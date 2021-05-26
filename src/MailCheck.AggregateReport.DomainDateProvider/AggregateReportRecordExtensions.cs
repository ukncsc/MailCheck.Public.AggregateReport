using System;
using System.Collections.Generic;
using MailCheck.AggregateReport.Contracts;

namespace MailCheck.AggregateReport.DomainDateProvider
{
    public static class AggregateReportRecordExtensions
    {
        public static List<DomainDateProviderRecord> ToDomainDateProviderRecord(
            this AggregateReportRecordEnriched aggregateReportRecord)
        {
            DomainDateProviderRecord record = GetDomainDateProviderRecord(aggregateReportRecord);

            return new List<DomainDateProviderRecord> {record, record.CloneWithDifferentProvider("All Providers", null)};
        }

        private static DomainDateProviderRecord GetDomainDateProviderRecord(
            AggregateReportRecordEnriched aggregateReportRecord)
        {
            long id = long.Parse(aggregateReportRecord.RecordId);
            string domain = aggregateReportRecord.HeaderFrom.Trim().TrimEnd('.') ?? aggregateReportRecord.DomainFrom;
            string provider = aggregateReportRecord.GetProvider();
            string originalProvider = aggregateReportRecord.HostProvider;
            DateTime date = aggregateReportRecord.EffectiveDate.Date;
            int count = aggregateReportRecord.Count;
            DmarcResult spfResult = aggregateReportRecord.Spf.GetValueOrDefault(DmarcResult.fail);
            DmarcResult dkimResult = aggregateReportRecord.Dkim.GetValueOrDefault(DmarcResult.fail);
            Policy disposition = aggregateReportRecord.Disposition.GetValueOrDefault(Policy.none);

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.reject)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0);
            }

            return new DomainDateProviderRecord(id, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count);
        }
    }
}