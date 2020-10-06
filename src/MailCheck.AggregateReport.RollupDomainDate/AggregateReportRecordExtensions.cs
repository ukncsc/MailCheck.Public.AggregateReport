using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;

namespace MailCheck.AggregateReport.RollupDomainDate
{
    public static class AggregateReportRecordExtensions
    {
        public static List<DomainDateRecord> ToDomainDateRecords(this AggregateReportRecordEnriched aggregateReportRecord)
        {
            long id = long.Parse(aggregateReportRecord.RecordId);
            string domain = aggregateReportRecord.HeaderFrom?.Trim().Trim('.').ToLower() ?? aggregateReportRecord.DomainFrom.ToLower();
            string orgDomain = aggregateReportRecord.OrganisationDomainFrom?.Trim().Trim('.').ToLower() ?? domain;
            DateTime date = aggregateReportRecord.EffectiveDate.Date;
            int count = aggregateReportRecord.Count;
            DmarcResult spfResult = aggregateReportRecord.Spf.GetValueOrDefault(DmarcResult.fail);
            DmarcResult dkimResult = aggregateReportRecord.Dkim.GetValueOrDefault(DmarcResult.fail);
            Policy disposition = aggregateReportRecord.Disposition.GetValueOrDefault(Policy.none);
            
            List<string> domains = new List<string>{domain};
            

            while (domain != orgDomain)
            {
                domain = domain.Substring(domain.IndexOf('.')+1);
                domains.Add(domain);
            };
            
            return domains.Select(_ => CreateDomainDateRecord(spfResult, dkimResult, disposition, id, _, date, count)).ToList();
        }

        private static DomainDateRecord CreateDomainDateRecord(DmarcResult spfResult, DmarcResult dkimResult,
            Policy disposition, long id, string domain, DateTime date, int count)
        {
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