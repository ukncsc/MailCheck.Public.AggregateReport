using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Util;
using MailCheck.Intelligence.Enricher.Asn;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;

namespace MailCheck.Intelligence.Enricher.Dao
{
    public interface IIpAddressDetailsDao
    {
        Task<List<IpAddressDetails>> GetIpAddressDetails(List<IpAddressDetailsRequest> ipAddressDetailsRequests);
        Task SaveIpAddressDetails(List<IpAddressDetails> ipAddressDetailsResponse);
        Task UpdateAsnInfo(List<AsInfo> requests);
        Task<List<IpAddressDetails>> GetIpAddressDetails(string ipAddress);
        Task UpdateReverseDns(List<IpAddressDetailsUpdateDto> entriesToUpdate);
    }

    public class IpAddressDetailsDao : IIpAddressDetailsDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<IpAddressDetailsDao> _log;

        public IpAddressDetailsDao(IConnectionInfoAsync connectionInfo, ILogger<IpAddressDetailsDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public async Task<List<IpAddressDetails>> GetIpAddressDetails(List<IpAddressDetailsRequest> ipAddressDetailsRequests)
        {
            List<IpAddressDetailsRequest> distinctDetailsRequests = ipAddressDetailsRequests
                .GroupBy(x => new { x.Date, x.IpAddress })
                .Select(x => x.First())
                .ToList();

            _log.LogInformation($"Retrieving address details for {distinctDetailsRequests.Count} distinct requests from {ipAddressDetailsRequests.Count}");

            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            List<IpAddressDetails> result = new List<IpAddressDetails>();

            List<IGrouping<DateTime, IpAddressDetailsRequest>> requestsByDate = ipAddressDetailsRequests
                .GroupBy(x => x.Date)
                .ToList();

            List<string> queries = new List<string>();
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

            for (int i = 0; i < requestsByDate.Count; i++)
            {
                IEnumerable<NpgsqlParameter> ipAddressParams = requestsByDate[i].Select((request, j) =>
                {
                    bool ipAddressValue = IPAddress.TryParse(request.IpAddress, out IPAddress ipAddress);
                    if (!ipAddressValue)
                    {
                        _log.LogInformation($"Unable to parse IP address {request.IpAddress}");
                        throw new ArgumentException($"Unable to parse IP address {request.IpAddress}");
                    }

                    NpgsqlParameter parameter = new NpgsqlParameter($"ip_address_{i}_{j}", NpgsqlDbType.Inet)
                    {
                        Value = ipAddress
                    };
                    return parameter;
                });

                NpgsqlParameter dateParams = new NpgsqlParameter($"date_{i}", NpgsqlDbType.Date)
                {
                    Value = requestsByDate[i].Key
                };

                parameters.AddRange(ipAddressParams);
                parameters.Add(dateParams);

                string queryForDate = string.Format(IpAddressIntelligenceDaoResources.SelectIpAddressDetails, string.Join(',', requestsByDate[i].Select((_, j) => $"@ip_address_{i}_{j}")), $"@date_{i}");
                queries.Add(queryForDate);
            }

            string query = string.Join(" union all ", queries);

            using (DbDataReader reader = await PostgreSqlHelper.ExecuteReaderAsync(connectionString, query, parameters.ToArray()))
            {
                while (await reader.ReadAsync())
                {
                    int ipAddressOrdinal = reader.GetOrdinal("ip_address");
                    string ipAddress = reader.GetFieldValue<IPAddress>(ipAddressOrdinal).ToString();
                    DateTime date = reader.GetDateTime("date");

                    int asNumberOrdinal = reader.GetOrdinal("as_number");
                    int? asNumber = reader.IsDBNull(asNumberOrdinal) ? (int?)null : reader.GetInt32("as_number");

                    int descriptionOrdinal = reader.GetOrdinal("description");
                    string description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString("description");

                    int countryCodeOrdinal = reader.GetOrdinal("country_code");
                    string countryCode = reader.IsDBNull(countryCodeOrdinal) ? null : reader.GetString("country_code");

                    int blocklistDataOrdinal = reader.GetOrdinal("blocklist_data");
                    List<BlocklistAppearance> blocklistData = reader.IsDBNull(blocklistDataOrdinal) ? null : JsonConvert.DeserializeObject<List<BlocklistAppearance>>(reader.GetString("blocklist_data"));

                    int reverseDnsDataOrdinal = reader.GetOrdinal("reverse_dns_data");
                    List<ReverseDnsResponse> reverseDnsData = reader.IsDBNull(reverseDnsDataOrdinal) ? null : JsonConvert.DeserializeObject<List<ReverseDnsResponse>>(reader.GetString("reverse_dns_data"));

                    int asnUpdatedOrdinal = reader.GetOrdinal("asn_updated");
                    DateTime? asnUpdated = reader.IsDBNull(asnUpdatedOrdinal) ? (DateTime?)null : reader.GetDateTime("asn_updated");

                    int blocklistUpdatedOrdinal = reader.GetOrdinal("blocklist_updated");
                    DateTime? blocklistUpdated = reader.IsDBNull(blocklistUpdatedOrdinal) ? (DateTime?)null : reader.GetDateTime("blocklist_updated");

                    int reverseDnsUpdatedOrdinal = reader.GetOrdinal("reverse_dns_updated");
                    DateTime? reverseDnsUpdated = reader.IsDBNull(reverseDnsUpdatedOrdinal) ? (DateTime?)null : reader.GetDateTime("reverse_dns_updated");

                    IpAddressDetails ipAddressDetails = new IpAddressDetails(
                        ipAddress, date, asNumber, description, countryCode, blocklistData, reverseDnsData, asnUpdated,
                        blocklistUpdated, reverseDnsUpdated, false);

                    result.Add(ipAddressDetails);
                }
            }

            _log.LogInformation($"Found {result.Count} IpAddressDetails in database from request for {ipAddressDetailsRequests.Count}");

            return result;
        }

        public async Task UpdateAsnInfo(List<AsInfo> requests)
        {
            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            string command = "UPDATE public.ip_address_details SET as_number = @as_number, description = @description, country_code = @country_code, asn_updated = @asn_updated WHERE ip_address = @ip_address";

            DateTime now = DateTime.UtcNow;
            foreach (var asInfo in requests)
            {
                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
                parameters.Add(new NpgsqlParameter($"@ip_address", NpgsqlDbType.Inet) { Value = IPAddress.Parse(asInfo.IpAddress) });

                parameters.Add(new NpgsqlParameter($"@as_number", NpgsqlDbType.Numeric) { Value = asInfo.AsNumber });
                parameters.Add(new NpgsqlParameter($"@description", NpgsqlDbType.Varchar, 250) { Value = asInfo.Description });
                parameters.Add(new NpgsqlParameter($"@country_code", NpgsqlDbType.Char, 2) { Value = asInfo.CountryCode });
                parameters.Add(new NpgsqlParameter($"@asn_updated", NpgsqlDbType.Timestamp) { Value = new NpgsqlDateTime(now) });
                await PostgreSqlHelper.ExecuteNonQueryAsync(connectionString, command, parameters.ToArray());
            }
        }

        public async Task<List<IpAddressDetails>> GetIpAddressDetails(string ipAddress)
        {
            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            string command = "SELECT * from public.ip_address_details WHERE ip_address = @ip_address";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            parameters.Add(new NpgsqlParameter($"@ip_address", NpgsqlDbType.Inet) { Value = IPAddress.Parse(ipAddress) });

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
                        reverseDnsUpdated,
                        false);

                    results.Add(ipAddressDetails);
                }
            }

            return results;
        }

        public async Task UpdateReverseDns(List<IpAddressDetailsUpdateDto> entriesToUpdate)
        {
            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            string command = "UPDATE public.ip_address_details SET reverse_dns_data = @reverse_dns_data, reverse_dns_updated = @reverse_dns_updated WHERE ip_address = @ip_address AND date = @date";

            DateTime now = DateTime.UtcNow;

            foreach (IpAddressDetailsUpdateDto entryToUpdate in entriesToUpdate)
            {
                List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
                parameters.Add(new NpgsqlParameter($"@date", NpgsqlDbType.Date) { Value = new NpgsqlDate(entryToUpdate.Date) });
                parameters.Add(new NpgsqlParameter($"@ip_address", NpgsqlDbType.Inet) { Value = IPAddress.Parse(entryToUpdate.IpAddress) });
                parameters.Add(new NpgsqlParameter($"@reverse_dns_data", NpgsqlDbType.Json) { Value = JsonConvert.SerializeObject(entryToUpdate.ReverseDnsResponses) });
                parameters.Add(new NpgsqlParameter($"@reverse_dns_updated", NpgsqlDbType.Timestamp) { Value = new NpgsqlDateTime(now) });

                await PostgreSqlHelper.ExecuteNonQueryAsync(connectionString, command, parameters.ToArray());
            }
        }

        public async Task SaveIpAddressDetails(List<IpAddressDetails> ipAddressDetailsResponse)
        {
            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            List<string> values = new List<string>();
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            for (int i = 0; i < ipAddressDetailsResponse.Count; i++)
            {
                parameters.Add(new NpgsqlParameter($"@date_{i}", NpgsqlDbType.Date) { Value = new NpgsqlDate(ipAddressDetailsResponse[i].Date) });
                parameters.Add(new NpgsqlParameter($"@ip_address_{i}", NpgsqlDbType.Inet) { Value = IPAddress.Parse(ipAddressDetailsResponse[i].IpAddress) });

                parameters.Add(ipAddressDetailsResponse[i].AsNumber.HasValue
                    ? new NpgsqlParameter($"@as_number_{i}", NpgsqlDbType.Integer) { Value = ipAddressDetailsResponse[i].AsNumber.Value }
                    : new NpgsqlParameter($"@as_number_{i}", NpgsqlDbType.Integer) { Value = DBNull.Value });

                parameters.Add(!string.IsNullOrEmpty(ipAddressDetailsResponse[i].Description)
                    ? new NpgsqlParameter($"@description_{i}", NpgsqlDbType.Varchar, 250) { Value = ipAddressDetailsResponse[i].Description }
                    : new NpgsqlParameter($"@description_{i}", NpgsqlDbType.Varchar) { Value = DBNull.Value });

                parameters.Add(!string.IsNullOrEmpty(ipAddressDetailsResponse[i].CountryCode)
                    ? new NpgsqlParameter($"@country_code_{i}", NpgsqlDbType.Char, 2) { Value = ipAddressDetailsResponse[i].CountryCode }
                    : new NpgsqlParameter($"@country_code_{i}", NpgsqlDbType.Char, 2) { Value = DBNull.Value });

                parameters.Add(ipAddressDetailsResponse[i].ReverseDnsResponses != null && ipAddressDetailsResponse[i].ReverseDnsResponses.Count > 0
                    ? new NpgsqlParameter($"@reverse_dns_data_{i}", NpgsqlDbType.Json) { Value = JsonConvert.SerializeObject(ipAddressDetailsResponse[i].ReverseDnsResponses) }
                    : new NpgsqlParameter($"@reverse_dns_data_{i}", NpgsqlDbType.Json) { Value = DBNull.Value });

                parameters.Add(ipAddressDetailsResponse[i].BlockListOccurrences != null && ipAddressDetailsResponse[i].BlockListOccurrences.Count > 0
                    ? new NpgsqlParameter($"@blocklist_data_{i}", NpgsqlDbType.Json) { Value = JsonConvert.SerializeObject(ipAddressDetailsResponse[i].BlockListOccurrences) }
                    : new NpgsqlParameter($"@blocklist_data_{i}", NpgsqlDbType.Json) { Value = DBNull.Value });

                parameters.Add(ipAddressDetailsResponse[i].AsnLookupTimestamp.HasValue
                    ? new NpgsqlParameter($"@asn_updated_{i}", NpgsqlDbType.Timestamp) { Value = new NpgsqlDateTime(ipAddressDetailsResponse[i].AsnLookupTimestamp.Value) }
                    : new NpgsqlParameter($"@asn_updated_{i}", NpgsqlDbType.Timestamp) { Value = DBNull.Value });

                parameters.Add(ipAddressDetailsResponse[i].BlocklistLookupTimestamp.HasValue
                    ? new NpgsqlParameter($"@blocklist_updated_{i}", NpgsqlDbType.Timestamp) { Value = new NpgsqlDateTime(ipAddressDetailsResponse[i].BlocklistLookupTimestamp.Value) }
                    : new NpgsqlParameter($"@blocklist_updated_{i}", NpgsqlDbType.Timestamp) { Value = DBNull.Value });

                parameters.Add(ipAddressDetailsResponse[i].ReverseDnsLookupTimestamp.HasValue
                    ? new NpgsqlParameter($"@reverse_dns_updated_{i}", NpgsqlDbType.Timestamp) { Value = new NpgsqlDateTime(ipAddressDetailsResponse[i].ReverseDnsLookupTimestamp.Value) }
                    : new NpgsqlParameter($"@reverse_dns_updated_{i}", NpgsqlDbType.Timestamp) { Value = DBNull.Value });

                values.Add($"(@date_{i}, @ip_address_{i}, @as_number_{i}, @description_{i}, @country_code_{i}, @reverse_dns_data_{i}, @blocklist_data_{i}, @asn_updated_{i}, @blocklist_updated_{i}, @reverse_dns_updated_{i})");
            }

            string command = string.Format(IpAddressIntelligenceDaoResources.InsertIpAddressDetails, string.Join(",", values));

            await PostgreSqlHelper.ExecuteNonQueryAsync(connectionString, command, parameters.ToArray());
        }
    }
}
