﻿using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;

namespace MailCheck.AggregateReport.RollupDomainDateProvider
{
    public static class AggregateReportRecordExtensions
    {
        public static List<DomainDateProviderRecord> ToDomainDateProviderRecord(
            this AggregateReportRecordEnriched aggregateReportRecord)
        {
            long id = long.Parse(aggregateReportRecord.RecordId);
            string domain = aggregateReportRecord.HeaderFrom?.Trim().Trim('.').ToLower() ??
                            aggregateReportRecord.DomainFrom.ToLower();
            string orgDomain = aggregateReportRecord.OrganisationDomainFrom?.Trim().Trim('.').ToLower() ?? domain;
            string provider = aggregateReportRecord.GetProvider();
            string originalProvider = aggregateReportRecord.HostProvider;

            DateTime date = aggregateReportRecord.EffectiveDate.Date;
            int count = aggregateReportRecord.Count;
            DmarcResult spfResult = aggregateReportRecord.Spf.GetValueOrDefault(DmarcResult.fail);
            DmarcResult dkimResult = aggregateReportRecord.Dkim.GetValueOrDefault(DmarcResult.fail);
            Policy disposition = aggregateReportRecord.Disposition.GetValueOrDefault(Policy.none);

            List<string> domainNames = new List<string> {domain};

            while (domain != orgDomain)
            {
                domain = domain.Substring(domain.IndexOf('.') + 1);
                domainNames.Add(domain);
            }

            List<DomainDateProviderRecord> resultSets = domainNames.Select(x =>
                    CreateDomainDateProvider(spfResult, dkimResult, disposition, id, x, date, provider, originalProvider, count))
                .ToList();

            List<DomainDateProviderRecord> allProviders =
                resultSets.Select(_ => _.CloneWithDifferentProvider("All Providers", null)).ToList();

            resultSets.AddRange(allProviders);

            return resultSets;
        }

        private static DomainDateProviderRecord CreateDomainDateProvider(DmarcResult spfResult, DmarcResult dkimResult,
            Policy disposition, long recordId, string domain, DateTime date, string provider, string originalProvider, int count)
        {
            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.reject)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0);
            }

            return new DomainDateProviderRecord(recordId, domain, date, provider, originalProvider, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count);
        }
    }
}