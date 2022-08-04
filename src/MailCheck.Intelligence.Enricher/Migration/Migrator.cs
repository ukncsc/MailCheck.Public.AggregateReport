using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.AggregateReport.Contracts.Migration;
using MailCheck.Common.Data.Util;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Util;
using MailCheck.Intelligence.Enricher.Dao;
using MailCheck.Intelligence.Enricher.Config;
using Newtonsoft.Json;
using MySqlHelper = MailCheck.Common.Data.Util.MySqlHelper;
// ReSharper disable LocalizableElement

namespace MailCheck.Intelligence.Enricher.Migration
{
    public interface IMigrator
    {
        Task Migrate();
        Task BackfillAsn();
        Task BackfillReverseDns();
    }

    public class Migrator : IMigrator
    {
        private readonly IIpAddressDetailsDao _ipAddressDetailsDao;
        private readonly IMessagePublisher _publisher;
        private readonly IEnricherConfig _enricherConfig;

        public Migrator(IIpAddressDetailsDao ipAddressDetailsDao, IMessagePublisher publisher, IEnricherConfig enricherConfig)
        {
            _ipAddressDetailsDao = ipAddressDetailsDao;
            _publisher = publisher;
            _enricherConfig = enricherConfig;
        }

        public async Task Migrate()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine($"Getting all existing records");

            List<(DateTime, ReverseDnsResult)> existingReverseDnsRecords = await GetExistingReverseDnsResults(stopwatch);

            Console.WriteLine($"Grouping all existing records at {stopwatch.ElapsedMilliseconds} ms");

            IEnumerable<IGrouping<DateTime, ReverseDnsResult>> recordsGroupedByDate = existingReverseDnsRecords.GroupBy(x => x.Item1, y => y.Item2);

            double updateTally = 0;
            foreach (IGrouping<DateTime, ReverseDnsResult> recordGrouping in recordsGroupedByDate)
            {
                DateTime date = recordGrouping.Key;
                List<ReverseDnsResult> recordsToSaveForDate = recordGrouping.ToList();

                List<IpAddressDetails> ipAddressDetailsForDate = new List<IpAddressDetails>();

                foreach (ReverseDnsResult reverseDnsResult in recordsToSaveForDate)
                {
                    IpAddressDetails ipAddressDetails = new IpAddressDetails(reverseDnsResult.OriginalIpAddress, date, null, null, null, null, reverseDnsResult.ForwardResponses, null, null, date, false);

                    ipAddressDetailsForDate.Add(ipAddressDetails);
                }

                List<Task> insertsToDo = ipAddressDetailsForDate.Batch(1000).Select(batch => _ipAddressDetailsDao.SaveIpAddressDetails(batch.ToList())).ToList();

                await Task.WhenAll(insertsToDo);

                updateTally += ipAddressDetailsForDate.Count;
                double percentDone = updateTally / 4500000d;
                Console.Write($"\rUpdated {updateTally} = {percentDone:P2}% after {stopwatch.Elapsed.Minutes} mins {stopwatch.Elapsed.Seconds} secs");
            }

            Console.WriteLine($"\nDone updating at {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine("Press a key");
            Console.ReadLine();
        }

        public async Task BackfillAsn()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine($"Getting all records with no ASN details at {stopwatch.ElapsedMilliseconds} ms");

            List<string> ipAddressesToBeUpdated = new List<string>();
            using (DbDataReader reader = await PostgreSqlHelper.ExecuteReaderAsync(Environment.GetEnvironmentVariable("ConnectionString"), "SELECT distinct ip_address FROM public.ip_address_details where as_number is null"))
            {
                while (await reader.ReadAsync())
                {
                    int ipAddressOrdinal = reader.GetOrdinal("ip_address");
                    ipAddressesToBeUpdated.Add(reader.GetFieldValue<IPAddress>(ipAddressOrdinal).ToString());
                }
            }

            Console.WriteLine($"Publishing {ipAddressesToBeUpdated.Count} records at {stopwatch.ElapsedMilliseconds} ms");

            List<AsnBackfillBatch> batchMessages = new List<AsnBackfillBatch>();

            foreach (IEnumerable<string> batch in ipAddressesToBeUpdated.Batch(ipAddressesToBeUpdated.Count))
            {
                batchMessages.Add(new AsnBackfillBatch(Guid.NewGuid().ToString(), batch.ToList()));
            }
    
           IEnumerable<Task> publishTasks = batchMessages.Select(batchMessage => _publisher.Publish(batchMessage, _enricherConfig.SnsTopicArn));


            var a = publishTasks.Count();
            Console.WriteLine($"Created {a} tasks");
            await Task.WhenAll(publishTasks);

            Console.WriteLine($"Done at {stopwatch.ElapsedMilliseconds} ms");
            
Console.WriteLine("Press a key");
            Console.ReadLine();
        }

        public async Task BackfillReverseDns()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<string> entriesToBeUpdated = new List<string>();

            Console.WriteLine($"Getting all records with no reverse dns entry at {stopwatch.ElapsedMilliseconds} ms");

            using (DbDataReader reader = await PostgreSqlHelper.ExecuteReaderAsync(Environment.GetEnvironmentVariable("ConnectionString"), "SELECT DISTINCT ip_address FROM public.ip_address_details where reverse_dns_data is null"))
            {
                while (await reader.ReadAsync())
                {
                    int ipAddressOrdinal = reader.GetOrdinal("ip_address");
                    entriesToBeUpdated.Add(reader.GetFieldValue<IPAddress>(ipAddressOrdinal).ToString());
                }
            }

            Console.WriteLine($"Publishing {entriesToBeUpdated.Count} records at {stopwatch.ElapsedMilliseconds} ms");
            List<ReverseDnsBackfillBatch> batchMessages = entriesToBeUpdated.Batch(500).Select(batch => new ReverseDnsBackfillBatch(Guid.NewGuid().ToString(), batch.ToList())).ToList();

            IEnumerable<Task> publishTasks = batchMessages.Select(batchMessage => _publisher.Publish(batchMessage, _enricherConfig.SnsTopicArn));

            await Task.WhenAll(publishTasks);

            Console.WriteLine($"Done at {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine("Press a key");
            Console.ReadLine();
        }

        private async Task<List<(DateTime, ReverseDnsResult)>> GetExistingReverseDnsResults(Stopwatch stopwatch)
        {
            Console.WriteLine($"Querying all reverse dns results");

            List<(DateTime, ReverseDnsResult)> results = new List<(DateTime, ReverseDnsResult)>();
            double tally = 0;
            using (DbDataReader reader = await MySqlHelper.ExecuteReaderAsync(Environment.GetEnvironmentVariable("ReverseDnsConnectionString"), "SELECT * FROM reverse_lookup_results"))
            {
                while (await reader.ReadAsync())
                {
                    ReverseDnsResponse data = JsonConvert.DeserializeObject<List<ReverseDnsResponse>>(reader.GetString("data")).FirstOrDefault();

                    List<ReverseDnsResponse> reverseDnsResponses = null;
                    if (data != null)
                    {
                        List<string> ipAddresses = data.IpAddresses;
                        string host = data.Host;
                        string organisationalDomain = data.OrganisationalDomain;

                        reverseDnsResponses = new List<ReverseDnsResponse>
                            {new ReverseDnsResponse(host, ipAddresses, organisationalDomain)};
                    }

                    DateTime date = reader.GetDateTime("date");
                    string originalIpAddress = reader.GetString("ip_address");

                    ReverseDnsResult reverseDnsResult = new ReverseDnsResult(originalIpAddress, reverseDnsResponses);
                    results.Add((date, reverseDnsResult));

                    tally++;
                    double percentDone = tally / 4500000d;
                    Console.Write($"\rLoaded {tally} = {percentDone:P2}% after {stopwatch.Elapsed.Minutes} mins {stopwatch.Elapsed.Seconds} secs");
                }
            }

            Console.WriteLine($"\nFinished querying all results");

            return results;
        }
    }
}
