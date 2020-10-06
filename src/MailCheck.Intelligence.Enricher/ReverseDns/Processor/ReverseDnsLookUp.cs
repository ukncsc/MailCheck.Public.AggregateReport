using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Enricher.ReverseDns.Dns;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.Enricher.ReverseDns.Processor
{
    public interface IReverseDnsLookup
    {
        Task<ReverseDnsResult> Lookup(string ipAddresses);
    }

    public class ReverseDnsLookup : IReverseDnsLookup
    {
        private readonly IDnsResolver _dnsResolver;
        private readonly ILogger<ReverseDnsLookup> _log;

        public ReverseDnsLookup(IDnsResolver dnsResolver, ILogger<ReverseDnsLookup> log)
        {
            _dnsResolver = dnsResolver;
            _log = log;
        }

        public async Task<ReverseDnsResult> Lookup(string ipAddress)
        {
            return IPAddress.TryParse(ipAddress, out IPAddress originalAddress)
                ? await GetReverseDnsResult(originalAddress)
                : ReverseDnsResult.InvalidReverseDnsResult;
        }

        private async Task<ReverseDnsResult> GetReverseDnsResult(IPAddress originalAddress)
        {
            int attempt = 0;
            int maxAttempts = 5;

            while (attempt < maxAttempts)
            {
                await Task.Delay(attempt * attempt * 1000);
                attempt++;

                try
                {
                    ReverseDnsQueryResponse response = await _dnsResolver.QueryPtrAsync(originalAddress);

                    if (response.HasError && attempt < maxAttempts) continue;

                    if (response.HasError)
                    {
                        _log.LogWarning("Failed to do PTR look up for {IpAddress} with error: {Error}.", originalAddress, response.ErrorMessage ?? "Unknown error");
                        return ReverseDnsResult.InvalidReverseDnsResult;
                    }

                    List<string> hosts = response.Results;

                    List<ReverseDnsResponse> forwardResponses = originalAddress.AddressFamily == AddressFamily.InterNetwork
                        ? await GetDnsResponses<ARecord>(hosts, QueryType.A)
                        : await GetDnsResponses<AaaaRecord>(hosts, QueryType.AAAA);

                    return new ReverseDnsResult(originalAddress.ToString(), forwardResponses);
                }
                catch (Exception e)
                {
                    string errorMessage = $"Error occured performing PTR lookup for {originalAddress} (attempt {attempt} of {maxAttempts})";

                    if (attempt == maxAttempts)
                    {
                        _log.LogError(e, errorMessage);
                    }
                    else
                    {
                        _log.LogWarning(e, errorMessage);
                    }
                }
            }
            return ReverseDnsResult.InvalidReverseDnsResult;
        }

        private async Task<List<ReverseDnsResponse>> GetDnsResponses<T>(List<string> hosts, QueryType queryType)
            where T : AddressRecord
        {
            List<ReverseDnsResponse> responses = new List<ReverseDnsResponse>();
            foreach (var host in hosts)
            {
                List<string> ipAddresses = new List<string>();

                if (!string.IsNullOrWhiteSpace(host))
                {
                    ReverseDnsQueryResponse forward = await _dnsResolver.QueryAddressAsync<T>(host, queryType);

                    if (!forward.HasError)
                    {
                        ipAddresses.AddRange(forward.Results);
                        responses.Add(new ReverseDnsResponse(host, ipAddresses));
                    }
                    else
                    {
                        _log.LogWarning("Failed to do dns query type: {QueryType} for {Host} with error: {Error}.",
                            queryType, host, forward.ErrorMessage ?? "Unknown error");
                    }
                }
            }

            return responses;
        }
    }
}