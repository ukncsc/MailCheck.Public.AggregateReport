using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MailCheck.AggregateReport.Contracts.IpIntelligence
{
    public class IpAddressDetails
    {
        public IpAddressDetails(string ipAddress, DateTime date)
         : this(ipAddress, date, null, null, null, null, null, date, date, date)
        {
        }

        [JsonConstructor]
        public IpAddressDetails(string ipAddress, DateTime date, int? asNumber, string description, string countryCode, List<BlocklistAppearance> blockListOccurrences, List<ReverseDnsResponse> reverseDnsResponses, DateTime? asnLookupTimestamp, DateTime? blocklistLookupTimestamp, DateTime? reverseDnsLookupTimestamp)
        {
            Description = description;
            AsNumber = asNumber;
            BlockListOccurrences = blockListOccurrences ??  new List<BlocklistAppearance>();
            CountryCode = countryCode;
            Date = date;
            IpAddress = ipAddress;
            ReverseDnsResponses = reverseDnsResponses ?? new List<ReverseDnsResponse>();
            AsnLookupTimestamp = asnLookupTimestamp;
            BlocklistLookupTimestamp = blocklistLookupTimestamp;
            ReverseDnsLookupTimestamp = reverseDnsLookupTimestamp;
        }

        public string IpAddress { get; }
        public DateTime Date { get; }
        public int? AsNumber { get; }
        public string Description { get; }
        public string CountryCode { get; }
        public List<BlocklistAppearance> BlockListOccurrences { get; }
        public List<ReverseDnsResponse> ReverseDnsResponses { get; }
        public DateTime? AsnLookupTimestamp { get; }
        public DateTime? BlocklistLookupTimestamp { get; }
        public DateTime? ReverseDnsLookupTimestamp { get; }
    }
}