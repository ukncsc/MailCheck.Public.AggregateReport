using System;
using System.Collections.Generic;

namespace MailCheck.AggregateReport.Contracts.IpIntelligence
{
    public class IpAddressDetailsUpdateDto
    {
        public IpAddressDetailsUpdateDto(string ipAddress, DateTime date, List<ReverseDnsResponse> reverseDnsResponses, DateTime? reverseDnsLookupTimestamp)
        {
            Date = date;
            IpAddress = ipAddress;
            ReverseDnsResponses = reverseDnsResponses;
            ReverseDnsLookupTimestamp = reverseDnsLookupTimestamp;
        }

        public string IpAddress { get; }
        public DateTime Date { get; }
        public List<ReverseDnsResponse> ReverseDnsResponses { get; set; }
        public DateTime? ReverseDnsLookupTimestamp { get; set; }
    }
}