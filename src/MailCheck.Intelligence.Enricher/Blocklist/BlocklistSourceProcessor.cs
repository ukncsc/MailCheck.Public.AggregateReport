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
        IEnumerable<Task<BlocklistResult>> ProcessSource(List<string> ipAddresses);
    }

    public class BlocklistSourceProcessor : IBlocklistSourceProcessor
    {
        private const string NonExistentDomainError = "Non-Existent Domain";

        private readonly BlocklistSource _source;
        private readonly ILookupClient _lookupClient;
        private readonly ILogger<BlocklistSourceProcessor> _log;
        
        public BlocklistSourceProcessor(BlocklistSource source, ILookupClient lookupClient, ILogger<BlocklistSourceProcessor> log)
        {
        
            _source = source;
            _lookupClient = lookupClient;
            _log = log;
        }

        public IEnumerable<Task<BlocklistResult>> ProcessSource(List<string> ipAddresses)
        {
            var queries = ipAddresses
                .Select(ip => new { ip, query = GetQuery(ip, _source.Suffix) })
                .ToArray();

            var badIps = queries.Where(q => q.query == null).Select(q => q.ip).ToArray();
            if (badIps.Length > 0)
            {
                _log.LogInformation($"Found bad IPs in batch for lookup from source {_source.Suffix}.{Environment.NewLine}{string.Join(Environment.NewLine, badIps)}");
            }

            var goodIps = queries.Where(q => q.query != null).ToArray();

            return goodIps
                .Select(async q =>
                {
                    var response = await TryLookup(q.query);
                    return MapToBlocklistResult(q.ip, response);
                });
        }

        private async Task<IDnsQueryResponse> TryLookup(string query)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IDnsQueryResponse result = null;

            try
            {
                result = await _lookupClient.QueryAsync(query, QueryType.A);

                if (!result.HasError)
                {
                    _log.LogInformation(
                        $"Blocklist lookup of query {query} for source {_source.Suffix} resulted in success and took {stopwatch.ElapsedMilliseconds} ms.");
                }
                else
                {
                    _log.LogInformation(
                        $"Blocklist lookup of query {query} for source {_source.Suffix} resulted in error and took {stopwatch.ElapsedMilliseconds} ms." +
                        $"Error: {result.ErrorMessage ?? "Unknown Error"}{Environment.NewLine}{result.AuditTrail}");
                }
            }
            catch (Exception e)
            {
                _log.LogWarning(e, $"Blocklist lookup of query {query} for source {_source.Suffix} resulted in exception and took {stopwatch.ElapsedMilliseconds} ms.");                
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
                        string digits = UncompressIpV6(parsedIpAddress).Replace(":", "");
                        return $"{string.Join('.', digits.Reverse())}{suffix}";
                }
            }

            return null;
        }

        private string UncompressIpV6(IPAddress ipAddress)
        {
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
                return BlocklistResult.Inconclusive(ipAddress, "Exception occurred");
            }

            if (dnsResponse.HasError)
            {
                if (dnsResponse.ErrorMessage == NonExistentDomainError)
                    return new BlocklistResult(ipAddress);

                return BlocklistResult.Inconclusive(ipAddress, dnsResponse.ErrorMessage);
            }

            List<string> ipAddressesOfARecords = dnsResponse.Answers.OfType<ARecord>().Select(x => x.Address.ToString()).ToList();

            List<BlocklistAppearance> blocklistAppearances = _source.Data
                .Where(x => ipAddressesOfARecords.Contains(x.IpAddress))
                .Select(y => new BlocklistAppearance(y.Flag, y.Source, y.Description))
                .ToList();

            return new BlocklistResult(ipAddress, blocklistAppearances);
        }
    }
}