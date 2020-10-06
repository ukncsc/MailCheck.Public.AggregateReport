using System.Collections.Generic;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using Newtonsoft.Json;

namespace MailCheck.Intelligence.Api.Domain
{
    public class IpAddressDateRangeDetails
    {
        public IpAddressDateRangeDetails()
        {
            ReverseDnsDetails = new List<ReverseDnsDetail>();
            AsInfo = new List<AsInfo>();
            BlockListDetails = new List<BlockListDetail>();
        }

        public string IpAddress { get; set; }
        public List<ReverseDnsDetail> ReverseDnsDetails { get; set; }
        public List<AsInfo> AsInfo { get; set; }
        public List<BlockListDetail> BlockListDetails { get; set; }
    }
}