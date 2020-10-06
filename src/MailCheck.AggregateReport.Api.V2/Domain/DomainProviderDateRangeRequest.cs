using System;

namespace MailCheck.AggregateReport.Api.V2.Domain
{
    public class DomainProviderDateRangeRequest
    {
        public string Domain { get; set; }
        public string Provider { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CategoryFilter { get; set; }
        public string HostFilter { get; set; }
        public string IpFilter { get; set; }
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
