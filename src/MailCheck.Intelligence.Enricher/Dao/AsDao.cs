using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Util;
using MailCheck.Common.Util;
using MailCheck.Intelligence.Enricher.Asn;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace MailCheck.Intelligence.Enricher.Dao
{
    public interface IAsnDao
    {
        Task<List<AsInfo>> GetIp4(List<string> ipAddresses);
        Task<List<AsInfo>> GetIp6(List<string> ipAddresses);
    }

    public class AsDao : IAsnDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<IAsnDao> _log;

        public AsDao(IConnectionInfoAsync connectionInfo, ILogger<IAsnDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public async Task<List<AsInfo>> GetIp4(List<string> ipAddresses)
        {
            return await GetIp(ipAddresses, AsDaoResources.SelectIp4AsNumber);
        }

        public async Task<List<AsInfo>> GetIp6(List<string> ipAddresses)
        {
            return await GetIp(ipAddresses, AsDaoResources.SelectIp6AsNumber);
        }

        public async Task<List<AsInfo>> GetIp(List<string> ipAddresses, string selectQuery)
        {
            List<AsInfo> asnInfos = new List<AsInfo>();

            if (!ipAddresses.Any())
            {
                return asnInfos;
            }

            _log.LogInformation($"Retrieving asn info for {ipAddresses.Count} ip addresses");

            Stopwatch stopwatch = Stopwatch.StartNew();

            if (ipAddresses.Any())
            {
                IEnumerable<IEnumerable<string>> batches = ipAddresses.Batch(10);

                foreach (IEnumerable<string> batch in batches)
                {
                    asnInfos.AddRange(await GetBatch(batch.ToList(), selectQuery));
                }
            }

            _log.LogInformation($"Retrieved {asnInfos.Count} asn infos of requested {ipAddresses.Count} in {stopwatch.ElapsedMilliseconds} ms.");

            return asnInfos;
        }

        private async Task<List<AsInfo>> GetBatch(List<string> ipAddresses, string selectQuery)
        {
            List<AsInfo> asnInfos = new List<AsInfo>();

            if (ipAddresses.Any())
            {
                string query = string.Join(" UNION ",
                    ipAddresses.Select((_, i) =>
                        $"({string.Format(selectQuery, $"@ip_address{i}", $"@original_ip_address{i}")})"));

                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
                for (int i = 0; i < ipAddresses.Count; i++)
                {
                    string ipAddress = ipAddresses[i];
                    parameters.Add(new NpgsqlParameter($"ip_address{i}", NpgsqlDbType.Inet)
                    {
                        Value = IPAddress.Parse(ipAddress)
                    });
                    parameters.Add(new NpgsqlParameter($"original_ip_address{i}", NpgsqlDbType.Varchar)
                    {
                        Value = ipAddress
                    });
                }

                string connectionString = await _connectionInfo.GetConnectionStringAsync();

                using (DbDataReader reader =
                    await PostgreSqlHelper.ExecuteReaderAsync(connectionString, query, parameters.ToArray()))
                {

                    while (await reader.ReadAsync())
                    {
                        asnInfos.Add(new AsInfo
                        {
                            AsNumber = reader.GetInt32("asn"),
                            IpAddress = reader.GetString("original_ip_address"),
                            Description = reader.GetString("description"),
                            CountryCode = reader.GetString("country")
                        });
                    }
                }
            }

            return asnInfos;
        }
    }
}