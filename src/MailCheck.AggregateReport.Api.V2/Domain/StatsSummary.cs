namespace MailCheck.AggregateReport.Api.V2.Domain
{
    public class StatsSummary
    {
        public StatsSummary(int totalEmails, int totalFullyTrusted, int totalPartiallyTrusted, int totalQuarantined, int totalRejected, int totalUntrusted)
        {
            TotalEmails = totalEmails;
            TotalFullyTrusted = totalFullyTrusted;
            TotalPartiallyTrusted = totalPartiallyTrusted;
            TotalQuarantined = totalQuarantined;
            TotalRejected = totalRejected;
            TotalUntrusted = totalUntrusted;
        }

        public int TotalEmails { get; set; }
        public int TotalFullyTrusted { get; set; }
        public int TotalPartiallyTrusted { get; set; }
        public int TotalQuarantined { get; set; }
        public int TotalRejected { get; set; }
        public int TotalUntrusted { get; set; }
    }
}
