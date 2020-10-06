using System;
using System.Collections.Generic;

namespace MailCheck.ReverseDns.Client.Domain
{
    public class ReverseDnsInfo
    {
        public ReverseDnsInfo(string ipAddress, DateTime date, IList<DnsInfo> dnsResponses, IList<string> forwardLookupMatches)
        {
            IpAddress = ipAddress;
            Date = date;
            DnsResponses = dnsResponses;
            ForwardLookupMatches = forwardLookupMatches;
        }

        public string IpAddress { get; }

        public DateTime Date { get; }

        public IList<DnsInfo> DnsResponses { get; }

        public IList<string> ForwardLookupMatches { get; }
    }
}