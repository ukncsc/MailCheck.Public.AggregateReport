using System;
using System.Collections.Generic;
using MailCheck.AggregateReport.Contracts;

namespace MailCheck.AggregateReport.DomainDateProviderIp
{
    public static class AggregateReportRecordExtensions
    {
        public static List<DomainDateProviderIpRecord> ToDomainDateProviderIpRecord(
            this AggregateReportRecordEnriched aggregateReportRecord)
        {
            long id = long.Parse(aggregateReportRecord.RecordId);
            string domain = aggregateReportRecord.HeaderFrom?.Trim().Trim('.').ToLower() ?? aggregateReportRecord.DomainFrom.ToLower();
            string ip = aggregateReportRecord.HostSourceIp;
            string provider = aggregateReportRecord.GetProvider();
            string originalProvider = aggregateReportRecord.HostProvider;
            string hostname = aggregateReportRecord.HostName;
            DateTime date = aggregateReportRecord.EffectiveDate.Date;
            int count = aggregateReportRecord.Count;
            DmarcResult spfResult = aggregateReportRecord.Spf.GetValueOrDefault(DmarcResult.fail);
            DmarcResult dkimResult = aggregateReportRecord.Dkim.GetValueOrDefault(DmarcResult.fail);
            Policy disposition = aggregateReportRecord.Disposition.GetValueOrDefault(Policy.none);
            
            int spfMisalignedCount = (spfResult == DmarcResult.fail && aggregateReportRecord.SpfPassCount > 0 ? count : 0);
            int dkimMisAlignedCount = (dkimResult == DmarcResult.fail && aggregateReportRecord.DkimPassCount > 0 ? count : 0);

            int proxyBlockListCount = aggregateReportRecord.ProxyBlockListCount;
            int suspiciousNetworkBlockListCount = aggregateReportRecord.SuspiciousNetworkBlockListCount;
            int hijackedNetworkBlockListCount = aggregateReportRecord.HijackedNetworkBlockListCount;
            int endUserNetworkBlockListCount = aggregateReportRecord.EndUserNetworkBlockListCount;
            int spamSourceBlockListCount = aggregateReportRecord.SpamSourceBlockListCount;
            int malwareBlockListCount = aggregateReportRecord.MalwareBlockListCount;
            int endUserBlockListCount = aggregateReportRecord.EndUserBlockListCount;
            int bounceReflectorBlockListCount = aggregateReportRecord.BounceReflectorBlockListCount;
            
            int forwarded = aggregateReportRecord.Forwarded ? count : 0;
            int sampledOut = aggregateReportRecord.SampledOut ? count : 0;
            int trustedForwarder = aggregateReportRecord.TrustedForwarder ? count : 0;
            int mailingList = aggregateReportRecord.MailingList ? count : 0;
            int localPolicy = aggregateReportRecord.LocalPolicy ? count : 0;
            int arc = aggregateReportRecord.Arc ? count : 0;
            int otherOverrideReason = aggregateReportRecord.OtherOverrideReason ? count : 0;
            
            DomainDateProviderIpRecord record = CreateDomainDateProviderIp(spfResult, dkimResult, disposition, id, domain,
                date, provider, originalProvider, ip, hostname, count, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount,
                hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount,
                endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder,
                mailingList, localPolicy, arc, otherOverrideReason);
            
            return new List<DomainDateProviderIpRecord> {record, record.CloneWithDifferentProvider("All Providers", null)};
        }
        
        private static DomainDateProviderIpRecord CreateDomainDateProviderIp(DmarcResult spfResult, DmarcResult dkimResult,
            Policy disposition, long recordId, string domain, DateTime date, string provider, string originalProvider, string ip, string hostname, int count,
            int spfMisalignedCount, int dkimMisAlignedCount, int proxyBlockListCount, int suspiciousNetworkBlockListCount, int hijackedNetworkBlockListCount, 
            int endUserNetworkBlockListCount, int spamSourceBlockListCount, int malwareBlockListCount, int endUserBlockListCount, 
            int bounceReflectorBlockListCount, int forwarded, int sampledOut, int trustedForwarder, int mailingList, 
            int localPolicy, int arc, int otherOverrideReason)
        {
            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.none)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.none)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.quarantine)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.fail && disposition == Policy.quarantine)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.pass && dkimResult == DmarcResult.fail && disposition == Policy.reject)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            if (spfResult == DmarcResult.fail && dkimResult == DmarcResult.pass && disposition == Policy.reject)
            {
                return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, 0, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
            }

            return new DomainDateProviderIpRecord(recordId, domain, date, provider, originalProvider, ip, hostname, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, count, spfMisalignedCount, dkimMisAlignedCount, proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount, endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, endUserBlockListCount, bounceReflectorBlockListCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason);
        }
    }
}
