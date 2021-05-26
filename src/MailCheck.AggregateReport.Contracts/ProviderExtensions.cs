using System;
namespace MailCheck.AggregateReport.Contracts
{
    public static class ProviderExtensions
    {
        public static string GetProvider(this AggregateReportRecordEnriched aggregateReportRecord)
        {
            if (aggregateReportRecord.Arc == true)
            {
                return "ARC-Forwarded";
            }

            if (aggregateReportRecord.Dkim == DmarcResult.fail &&
              aggregateReportRecord.Spf == DmarcResult.fail &&
              aggregateReportRecord.ProxyBlockListCount + aggregateReportRecord.SuspiciousNetworkBlockListCount + aggregateReportRecord.HijackedNetworkBlockListCount + aggregateReportRecord.EndUserNetworkBlockListCount + aggregateReportRecord.SpamSourceBlockListCount + aggregateReportRecord.MalwareBlockListCount + aggregateReportRecord.EndUserBlockListCount + aggregateReportRecord.BounceReflectorBlockListCount > 0)
            {
                return "Blocklisted";
            }

            return aggregateReportRecord.HostProvider;
        }
    }
}
