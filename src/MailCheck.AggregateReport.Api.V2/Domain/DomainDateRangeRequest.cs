using System;

namespace MailCheck.AggregateReport.Api.V2.Domain
{
    public class DomainDateRangeRequest
    {
        public string Domain { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string CategoryFilter { get; set; }
        public string ProviderFilter { get; set; }
    }
}