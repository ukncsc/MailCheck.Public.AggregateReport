using System;
using System.Collections.Generic;
using MailCheck.AggregateReport.Contracts.IpIntelligence;

namespace MailCheck.Intelligence.Enricher.ReverseDns.Domain
{
    public class ReverseDnsInfo
    {
        public ReverseDnsInfo(string ipAddress, DateTime firstSeen, DateTime lastSeen, IList<ReverseDnsResponse> dnsResponses)
        {
            IpAddress = ipAddress;
            FirstSeen = firstSeen;
            LastSeen = lastSeen;
            DnsResponses = dnsResponses;
        }

        public string IpAddress { get; }
        public DateTime FirstSeen { get; }
        public DateTime LastSeen { get; }
        public IList<ReverseDnsResponse> DnsResponses { get; }
    }
}