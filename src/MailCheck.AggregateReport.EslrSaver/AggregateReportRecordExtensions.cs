using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;
using MailCheck.Common.Util;

namespace MailCheck.AggregateReport.EslrSaver
{
    public static class AggregateReportRecordExtensions
    {
        public static EslrSaverRecord ToEslrSaverRecord(
            this AggregateReportRecordEnriched aggregateReportRecord)
        {
            long recordId = long.Parse(aggregateReportRecord.RecordId);
            DateTime effectiveDate = aggregateReportRecord.EffectiveDate.Date;
            string domain = DomainNameUtils.ToCanonicalDomainName(aggregateReportRecord.HeaderFrom ?? aggregateReportRecord.DomainFrom);
            string reverseDomain = DomainNameUtils.ReverseDomainName(domain);
            string provider = aggregateReportRecord.HostProvider;
            string originalProvider = null;
            string reporterOrgName = aggregateReportRecord.ReporterOrgName;
            string ip = aggregateReportRecord.HostSourceIp;
            int count = aggregateReportRecord.Count;
            string disposition = aggregateReportRecord.Disposition.GetValueOrDefault(Policy.none).ToString();
            string dkim = aggregateReportRecord.Dkim.GetValueOrDefault(DmarcResult.fail).ToString();
            string spf = aggregateReportRecord.Spf.GetValueOrDefault(DmarcResult.fail).ToString();
            string envelopeTo = aggregateReportRecord.EnvelopeTo;
            string envelopeFrom = aggregateReportRecord.EnvelopeFrom;
            string headerFrom = aggregateReportRecord.HeaderFrom;
            string organisationDomainFrom = aggregateReportRecord.OrganisationDomainFrom;
            string spfAuthResults = string.Join(',', aggregateReportRecord.SpfAuthResults);
            int spfPassCount = aggregateReportRecord.SpfPassCount;
            int spfFailCount = aggregateReportRecord.SpfFailCount;
            string dkimAuthResults = string.Join(',', aggregateReportRecord.DkimAuthResults);
            int dkimPassCount = aggregateReportRecord.DkimPassCount;
            int dkimFailCount = aggregateReportRecord.DkimFailCount;
            int forwarded = aggregateReportRecord.Forwarded ? count : 0;
            int sampledOut = aggregateReportRecord.SampledOut ? count : 0;
            int trustedForwarder = aggregateReportRecord.TrustedForwarder ? count : 0;
            int mailingList = aggregateReportRecord.MailingList ? count : 0;
            int localPolicy = aggregateReportRecord.LocalPolicy ? count : 0;
            int arc = aggregateReportRecord.Arc ? count : 0;
            int otherOverrideReason = aggregateReportRecord.OtherOverrideReason ? count : 0;
            string hostName = aggregateReportRecord.HostName;
            string hostOrgDomain = aggregateReportRecord.HostOrgDomain;
            string hostProvider = aggregateReportRecord.HostProvider;
            int hostAsNumber = aggregateReportRecord.HostAsNumber;
            string hostAsDescription = aggregateReportRecord.HostAsDescription;
            string hostCountry = aggregateReportRecord.HostCountry;
            int proxyBlockListCount = aggregateReportRecord.ProxyBlockListCount;
            int suspiciousNetworkBlockListCount = aggregateReportRecord.SuspiciousNetworkBlockListCount;
            int hijackedNetworkBlockListCount = aggregateReportRecord.HijackedNetworkBlockListCount;
            int endUserNetworkBlockListCount = aggregateReportRecord.EndUserNetworkBlockListCount;
            int spamSourceBlockListCount = aggregateReportRecord.SpamSourceBlockListCount;
            int malwareBlockListCount = aggregateReportRecord.MalwareBlockListCount;
            int endUserBlockListCount = aggregateReportRecord.EndUserBlockListCount;
            int bounceReflectorBlockListCount = aggregateReportRecord.BounceReflectorBlockListCount;
            
            if (aggregateReportRecord.Dkim == DmarcResult.fail &&
                aggregateReportRecord.Spf == DmarcResult.fail &&
                proxyBlockListCount + suspiciousNetworkBlockListCount + hijackedNetworkBlockListCount +
                endUserNetworkBlockListCount + spamSourceBlockListCount + malwareBlockListCount +
                endUserBlockListCount + bounceReflectorBlockListCount > 0)
            {
                originalProvider = provider;
                provider = "Blocklisted";
            }

            return new EslrSaverRecord(
                recordId, effectiveDate, domain, reverseDomain, provider, originalProvider, reporterOrgName, ip, count,
                disposition, dkim, spf, envelopeTo, envelopeFrom, headerFrom, organisationDomainFrom, 
                spfAuthResults, spfPassCount, spfFailCount, dkimAuthResults, dkimPassCount, dkimFailCount,
                forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason, 
                hostName, hostOrgDomain, hostProvider, hostAsNumber, hostAsDescription, hostCountry,
                proxyBlockListCount, suspiciousNetworkBlockListCount, hijackedNetworkBlockListCount,
                endUserNetworkBlockListCount, spamSourceBlockListCount, malwareBlockListCount, 
                endUserBlockListCount, bounceReflectorBlockListCount
                );
        }
    }
}
