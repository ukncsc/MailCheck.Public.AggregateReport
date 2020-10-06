using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Enricher.ProviderResolver.MailCheck.Intelligence.Enricher.ProviderResolver;
using NetTools;
using Newtonsoft.Json;

namespace MailCheck.Intelligence.Enricher.ProviderResolver
{
    public interface IProviderResolver
    {
        string GetProvider(IpAddressDetails ipAddressDetails);
    }

    public class ProviderResolver : IProviderResolver
    {
        private readonly Dictionary<string, string> _ipOverrides;
        private readonly Dictionary<string, string> _reverseDnsOverrides;

        public ProviderResolver()
        {
            string sources = File.ReadAllText("ProviderResolver/providerMappings.json");
            ProviderMappings providerMappings = JsonConvert.DeserializeObject<ProviderMappings>(sources);

            _ipOverrides = providerMappings.IpAddressProviderMappings
                .SelectMany(x => x.Mappings, (ipOverride, target) => new { target, ipOverride.Provider })
                .ToDictionary(x => x.target, y => y.Provider);

            _reverseDnsOverrides = providerMappings.OrgDomainProviderMappings
                .SelectMany(x => x.Mappings, (ipOverride, target) => new { target, ipOverride.Provider })
                .ToDictionary(x => x.target, y => y.Provider);
        }

        public string GetProvider(IpAddressDetails ipAddressDetails)
        {
            if (ipAddressDetails == null)
                throw new ArgumentException("IpAddressDetails required");

            string ipAddress = ipAddressDetails.IpAddress;

            string provider = TryGetProviderByIpAddress(ipAddress) ??
                              TryGetProviderByOrgDomain(ipAddressDetails) ??
                              ipAddressDetails.Description ??
                              "Unrouted";

            if (provider == null)
                throw new ApplicationException($"Unable to resolve provider{Environment.NewLine}{JsonConvert.SerializeObject(ipAddressDetails)}");

            return provider;
        }

        private string TryGetProviderByOrgDomain(IpAddressDetails ipAddressDetails)
        {
            string provider = null;
            string orgDomain = ipAddressDetails?.ReverseDnsResponses.Select(x => x.OrganisationalDomain)
                .FirstOrDefault();
            if (orgDomain != null && _reverseDnsOverrides.ContainsKey(orgDomain))
            {
                provider = _reverseDnsOverrides[orgDomain];
            }
            else if (ipAddressDetails?.ReverseDnsResponses != null &&
                     ipAddressDetails.ReverseDnsResponses.Any() &&
                     ipAddressDetails.ReverseDnsResponses[0].OrganisationalDomain != "Mismatch" &&
                     ipAddressDetails.ReverseDnsResponses[0].OrganisationalDomain != "Unknown")
            {
                provider = ipAddressDetails.ReverseDnsResponses[0].OrganisationalDomain;
            }

            return provider;
        }

        private string TryGetProviderByIpAddress(string ipAddressString)
        {
            string provider = null;

            IPAddress.TryParse(ipAddressString, out IPAddress ipAddress);

            if (ipAddress != null)
            {
                foreach (KeyValuePair<string, string> mapping in _ipOverrides)
                {
                    if (IPAddressRange.TryParse(mapping.Key, out var ipRange))
                    {
                        if (ipRange.Contains(ipAddress))
                        {
                            {
                                provider = mapping.Value;
                            }
                        }
                    }
                }
            }

            return provider;
        }
    }
}