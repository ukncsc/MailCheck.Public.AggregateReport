using System;
using System.Linq;
using MailCheck.AggregateReport.Contracts;

namespace MailCheck.AggregateReport.DomainDate
{
    public static class AggregateReportRecordExtensions
    {
        public static DomainDateRecord ToDomainDateRecord(this AggregateReportRecordEnriched aggregateReportRecord)
        {
            long id = long.Parse(aggregateReportRecord.RecordId);
            string domain = aggregateReportRecord.HeaderFrom?.Trim().Trim('.') ?? aggregateReportRecord.DomainFrom;
            DateTime date = aggregateReportRecord.EffectiveDate.Date;
            int count = aggregateReportRecord.Count;
            DmarcResult spfResult = aggregateReportRecord.Spf.GetValueOrDefault(DmarcResult.fail);
            DmarcResult dkimResult = aggregateReportRecord.Dkim.GetValueOrDefault(DmarcResult.fail);
            Policy disposition = aggregateReportRecord.Disposition.GetValueOrDefault(Policy.none);

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateRecord(id, domain, date, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateRecord(id, domain, date, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateRecord(id, domain, date, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateRecord(id, domain, date, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateRecord(id, domain, date, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateRecord(id, domain, date, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateRecord(id, domain, date, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateRecord(id, domain, date, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateRecord(id, domain, date, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.reject)
            {
                return new DomainDateRecord(id, domain, date, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateRecord(id, domain, date, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0);
            }

            return new DomainDateRecord(id, domain, date, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count);
        }
    }
}