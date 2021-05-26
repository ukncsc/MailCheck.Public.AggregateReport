using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Util;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.Enricher.Blocklist
{
    public interface IBlocklistSourceProcessor
    {
        Task<List<BlocklistResult>> ProcessSource(List<string> ipAddresses);
    }

    public class BlocklistSourceProcessor : IBlocklistSourceProcessor
    {
        private readonly BlockListSource _source;
        private readonly ILookupClient _lookupClient;
        private readonly ILogger<BlocklistSourceProcessor> _log;

        private const int MaxRequestsInPeriod = 10;
        private const int RateLimitPeriod = 1000;

        public BlocklistSourceProcessor(BlockListSource source, ILookupClient lookupClient, ILogger<BlocklistSourceProcessor> log)
        {
            _source = source;
            _lookupClient = lookupClient;
            _log = log;
        }

        public async Task<List<BlocklistResult>> ProcessSource(List<string> ipAddresses)
        {
            List<BlocklistResult> results = new List<BlocklistResult>();

            IEnumerable<IEnumerable<string>> batches = ipAddresses.Batch(MaxRequestsInPeriod);

            foreach (IEnumerable<string> batch in batches)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                List<Task<BlocklistResult>> lookupTasks = batch
                    .Select(x => TryLookup(GetQuery(x, _source.Suffix))
                    .ContinueWith(y => MapToBlocklistResult(x, y.Result))).ToList();
                Task delayTask = Task.Delay(RateLimitPeriod);


                await Task.WhenAll(new List<Task>(lookupTasks) { delayTask }.ToArray());

                BlocklistResult[] batchResults = await Task.WhenAll(lookupTasks);

                results.AddRange(batchResults);
                _log.LogInformation($"Looked up batch of {batchResults.Length} from source {_source.Suffix} after {stopwatch.ElapsedMilliseconds} ms.");
            }

            return results;
        }

        private async Task<IDnsQueryResponse> TryLookup(string query)
        {
            IDnsQueryResponse result = null;

            int attempt = 0;
            int maxAttempts = 5;

            while (attempt < maxAttempts)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                await Task.Delay(attempt * attempt * 1000);
                attempt++;
                
                try
                {
                    result = await _lookupClient.QueryAsync(query, QueryType.A);
                    
                    if (!result.HasError)
                    {
                        _log.LogInformation(
                            $"Blocklist lookup attempt {attempt} of query {query} for source {_source.Suffix} resulted in success and took {stopwatch.ElapsedMilliseconds} ms.");
                        break;
                    }
                    
                    _log.LogInformation(
                        $"Blocklist lookup attempt {attempt} of query {query} for source {_source.Suffix} resulted in error and took {stopwatch.ElapsedMilliseconds} ms." +
                        $"Error: {result.ErrorMessage ?? "Unknown Error"}");
                }
                catch (Exception e)
                {
                    _log.LogInformation(e,$"Blocklist lookup attempt {attempt} of query {query} for source {_source.Suffix} resulted in exception and took {stopwatch.ElapsedMilliseconds} ms.");                
                }
            }

            return result;
        }

        private string GetQuery(string ipAddress, string suffix)
        {
            if (IPAddress.TryParse(ipAddress, out IPAddress parsedIpAddress))
            {
                switch (parsedIpAddress.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        string ipAddressReversed = string.Join(".", ipAddress.Trim().Split(".").Reverse());
                        return $"{ipAddressReversed}{suffix}";

                    case AddressFamily.InterNetworkV6:
                        string digits = UncompressIpV6(ipAddress).Replace(":", "");
                        return $"{string.Join('.', digits.Reverse())}{suffix}";
                }
            }

            throw new ArgumentException("Unexpected IPAddress format", "ipAddress");
        }

        private string UncompressIpV6(string input)
        {
            IPAddress ipAddress = IPAddress.Parse(input);

            byte[] bytes = ipAddress.GetAddressBytes();
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < 16; i += 2)
            {
                builder.AppendFormat("{0:x2}{1:x2}:", bytes[i], bytes[i + 1]);
            }

            builder.Length = builder.Length - 1;
            return builder.ToString();
        }

        private BlocklistResult MapToBlocklistResult(string ipAddress, IDnsQueryResponse dnsResponse)
        {
            if (dnsResponse is null)
            {
                return new BlocklistResult(ipAddress);
            }

            List<string> ipAddressesOfARecords = dnsResponse.Answers.OfType<ARecord>().Select(x => x.Address.ToString()).ToList();

            List<BlocklistAppearance> blockListAppearances = _source.Data
                .Where(x => ipAddressesOfARecords.Contains(x.IpAddress))
                .Select(y => new BlocklistAppearance(y.Flag, y.Source, y.Description))
                .ToList();

            return new BlocklistResult(ipAddress, blockListAppearances);
        }
    }
}