using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const string NonExistentDomainError = "Non-Existent Domain";
        private const string ServerFailureError = "Server Failure";

        private readonly IDnsResolver _dnsResolver;
        private readonly ILogger<ReverseDnsLookup> _log;

        public ReverseDnsLookup(IDnsResolver dnsResolver, ILogger<ReverseDnsLookup> log)
        {
            _dnsResolver = dnsResolver;
            _log = log;
        }

        public async Task<ReverseDnsResult> Lookup(string ipAddress)
        {
            if(!IPAddress.TryParse(ipAddress, out IPAddress parsedIpAddress))
            {
                return ReverseDnsResult.Inconclusive(ipAddress);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                ReverseDnsQueryResponse ptrResponse = await _dnsResolver.QueryPtrAsync(parsedIpAddress);
                    
                if (ptrResponse.HasError)
                {
                    if (ptrResponse.ErrorMessage == ServerFailureError || ptrResponse.ErrorMessage == NonExistentDomainError)
                    {
                        _log.LogInformation($"PTR lookup for ip {ipAddress} resulted in {ptrResponse.ErrorMessage} error and took {stopwatch.ElapsedMilliseconds} ms.{Environment.NewLine}{ptrResponse.AuditTrail}");

                        return new ReverseDnsResult(ipAddress, null);
                    }

                    _log.LogWarning($"PTR lookup for ip {ipAddress} resulted in error and took {stopwatch.ElapsedMilliseconds} ms. Error: {ptrResponse.ErrorMessage ?? "Unknown Error"}.{Environment.NewLine}{ptrResponse.AuditTrail}");

                    return ReverseDnsResult.Inconclusive(ipAddress);
                }

                List<string> hosts = ptrResponse.Results;

                _log.LogInformation($"PTR lookup for ip {ipAddress} resulted in success returning {hosts.Count} hosts and took {stopwatch.ElapsedMilliseconds} ms.");

                List<ReverseDnsResponse> forwardResponses = new List<ReverseDnsResponse>();
     
                foreach (string host in hosts)
                {
                    Stopwatch forwardStopwatch = Stopwatch.StartNew();
                    if (!string.IsNullOrWhiteSpace(host))
                    {
                        ReverseDnsQueryResponse forwardResponse = parsedIpAddress.AddressFamily == AddressFamily.InterNetwork 
                            ? await _dnsResolver.QueryAddressAsync<ARecord>(host, QueryType.A)
                            : await _dnsResolver.QueryAddressAsync<AaaaRecord>(host, QueryType.AAAA);

                        if (forwardResponse.HasError)
                        {
                            if (forwardResponse.ErrorMessage == ServerFailureError || forwardResponse.ErrorMessage == NonExistentDomainError)
                            {
                                _log.LogInformation($"DNS lookup for host {host} resulted in {forwardResponse.ErrorMessage} error and took {stopwatch.ElapsedMilliseconds} ms.{Environment.NewLine}{forwardResponse.AuditTrail}");
                                forwardResponses.Add(new ReverseDnsResponse(host, new List<string>()));
                                continue;
                            }
                            _log.LogWarning($"DNS lookup for host {host} resulted in error: {forwardResponse.ErrorMessage ?? "Unknown error"}.{Environment.NewLine}{forwardResponse.AuditTrail}");
                            return ReverseDnsResult.Inconclusive(ipAddress);
                        }

                        forwardResponses.Add(new ReverseDnsResponse(host, forwardResponse.Results));
                        _log.LogInformation($"DNS lookup for host {host} resulted in success and took {forwardStopwatch.ElapsedMilliseconds} ms.");
                    }
                }

                return new ReverseDnsResult(ipAddress, forwardResponses);
            }
            catch (Exception e)
            {
                string errorMessage = $"Reverse DNS lookup for ip {ipAddress} resulted in exception and took {stopwatch.ElapsedMilliseconds} ms.";
                _log.LogWarning(e, errorMessage);
                return ReverseDnsResult.Inconclusive(ipAddress);
            }
        }
    }
}