using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Util;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace MailCheck.Intelligence.Api.Dao
{
    public interface IIpAddressDetailsApiDao
    {
        Task<List<IpAddressDetails>> GetIpAddressDetails(String ipAddress, DateTime startDate, DateTime endDate);
    }

    public class IpAddressDetailsApiDao : IIpAddressDetailsApiDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<IpAddressDetailsApiDao> _log;

        public IpAddressDetailsApiDao(IConnectionInfoAsync connectionInfo, ILogger<IpAddressDetailsApiDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public async Task<List<IpAddressDetails>> GetIpAddressDetails(String ipAddress, DateTime startDate, DateTime endDate)
        {
            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            string command = IpIntelligenceDao.SelectIpIntelligence;

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            parameters.Add(new NpgsqlParameter($"@ip_address", NpgsqlDbType.Inet) { Value = IPAddress.Parse(ipAddress) });
            parameters.Add(new NpgsqlParameter($"@start_date", NpgsqlDbType.Date) { Value = new NpgsqlDate(startDate) });
            parameters.Add(new NpgsqlParameter($"@end_date", NpgsqlDbType.Date) { Value = new NpgsqlDate(endDate) });

            List<IpAddressDetails> results = new List<IpAddressDetails>();

            using (DbDataReader reader = await PostgreSqlHelper.ExecuteReaderAsync(connectionString, command, parameters.ToArray()))
            {
                while (await reader.ReadAsync())
                {
                    int ipAddressOrdinal = reader.GetOrdinal("ip_address");
                    string ip = reader.GetFieldValue<IPAddress>(ipAddressOrdinal).ToString();

                    int asNumberOrdinal = reader.GetOrdinal("as_number");
                    int? asNumber = reader.IsDBNull(asNumberOrdinal) ? (int?)null : reader.GetInt32("as_number");

                    int descriptionOrdinal = reader.GetOrdinal("description");
                    string description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString("description");

                    int countryCodeOrdinal = reader.GetOrdinal("country_code");
                    string countryCode = reader.IsDBNull(countryCodeOrdinal) ? null : reader.GetString("country_code");

                    int asnUpdatedOrdinal = reader.GetOrdinal("asn_updated");
                    DateTime? asnUpdated = reader.IsDBNull(asnUpdatedOrdinal) ? (DateTime?)null : reader.GetDateTime("asn_updated");

                    int blocklistUpdatedOrdinal = reader.GetOrdinal("blocklist_updated");
                    DateTime? blocklistUpdated = reader.IsDBNull(blocklistUpdatedOrdinal) ? (DateTime?)null : reader.GetDateTime("blocklist_updated");

                    int reverseDnsUpdatedOrdinal = reader.GetOrdinal("reverse_dns_updated");
                    DateTime? reverseDnsUpdated = reader.IsDBNull(reverseDnsUpdatedOrdinal) ? (DateTime?)null : reader.GetDateTime("reverse_dns_updated");

                    string blocklistData = reader.GetString("blocklist_data");
                    string reverseDnsData = reader.GetString("reverse_dns_data");

                    IpAddressDetails ipAddressDetails = new IpAddressDetails(
                        ip,
                        reader.GetDateTime("date"),
                        asNumber,
                        description,
                        countryCode,
                        string.IsNullOrEmpty(blocklistData) ? null : JsonConvert.DeserializeObject<List<BlocklistAppearance>>(blocklistData),
                        string.IsNullOrEmpty(reverseDnsData) ? null : JsonConvert.DeserializeObject<List<ReverseDnsResponse>>(reverseDnsData),
                        asnUpdated,
                        blocklistUpdated,
                        reverseDnsUpdated);

                    results.Add(ipAddressDetails);
                }
            }

            return results;
        }
    }
}
