using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;

namespace MailCheck.AggregateReport.DomainDateProviderSubdomain
{
    public static class AggregateReportRecordExtensions
    {
        public static List<DomainDateProviderSubdomainRecord> ToDomainDateProviderSubdomainRecord(
            this AggregateReportRecordEnriched aggregateReportRecord)
        {
            long id = long.Parse(aggregateReportRecord.RecordId);
            string domain = aggregateReportRecord.HeaderFrom?.Trim().Trim('.').ToLower() ?? aggregateReportRecord.DomainFrom.ToLower();
            string orgDomain = aggregateReportRecord.OrganisationDomainFrom?.Trim().Trim('.').ToLower() ?? domain;
            string provider = aggregateReportRecord.GetProvider();
            string originalProvider = aggregateReportRecord.HostProvider;

            string subdomain = domain;
            DateTime date = aggregateReportRecord.EffectiveDate.Date;
            int count = aggregateReportRecord.Count;
            DmarcResult spfResult = aggregateReportRecord.Spf.GetValueOrDefault(DmarcResult.fail);
            DmarcResult dkimResult = aggregateReportRecord.Dkim.GetValueOrDefault(DmarcResult.fail);
            Policy disposition = aggregateReportRecord.Disposition.GetValueOrDefault(Policy.none);

            // Don't add subdomain==domain (this data comes from MailCheck.AggregateReport.DomainDateProvider)
            List<Tuple<string, string>> subdomainParentDomainPairs = new List<Tuple<string, string>> {};

            // Add {subdomain, parentDomain} where the parent is not higher than the org domain
            // e.g. a.b.c.gov.uk => {a.b.c.gov.uk, b.c.gov.uk}, {b.c.gov.uk, c.gov.uk}
            while (domain != orgDomain)
            {
                domain = domain.Substring(domain.IndexOf('.') + 1);
                subdomainParentDomainPairs.Add(Tuple.Create(subdomain,domain));
                subdomain = subdomain.Substring(subdomain.IndexOf('.') + 1);
            }

            List<DomainDateProviderSubdomainRecord> resultSets = subdomainParentDomainPairs.Select(x =>
                    CreateDomainDateProvider(spfResult, dkimResult, disposition, id, x.Item2, date, provider, originalProvider, x.Item1, count))
                .ToList();

            List<DomainDateProviderSubdomainRecord> allProviderResultSets = resultSets.Select(x => x.CloneWithDifferentProvider("All Providers", null)).ToList();
            resultSets.AddRange(allProviderResultSets);

            return resultSets;
        }
        
        private static DomainDateProviderSubdomainRecord CreateDomainDateProvider(DmarcResult spfResult, DmarcResult dkimResult,
            Policy disposition, long recordId, string domain, DateTime date, string provider, string originalProvider, string subdomain, int count)
        {
            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.reject)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0);
            }

            return new DomainDateProviderSubdomainRecord(recordId, domain, date, provider, originalProvider, subdomain, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count);
        }
    }
}