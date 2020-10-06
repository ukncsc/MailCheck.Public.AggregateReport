using System;
using System.Collections.Generic;

namespace MailCheck.AggregateReport.EslrSaver
{
    public class EslrSaverRecord
    {
        public EslrSaverRecord(
            long recordId,
            DateTime effectiveDate,
            string domain,
            string reverseDomain,
            string provider,
            string originalProvider,
            string reporterOrgName,
            string ip,
            int count,
            string disposition,
            string dkim,
            string spf,
            string envelopeTo,
            string envelopeFrom,
            string headerFrom,
            string organisationDomainFrom,
            string spfAuthResults,
            int spfPassCount,
            int spfFailCount,
            string dkimAuthResults,
            int dkimPassCount,
            int dkimFailCount,
            int forwarded,
            int sampledOut,
            int trustedForwarder,
            int mailingList,
            int localPolicy,
            int arc,
            int otherOverrideReason,
            string hostName,
            string hostOrgDomain,
            string hostProvider,
            int hostAsNumber,
            string hostAsDescription,
            string hostCountry,
            int proxyBlockListCount,
            int suspiciousNetworkBlockListCount,
            int hijackedNetworkBlockListCount,
            int endUserNetworkBlockListCount,
            int spamSourceBlockListCount,
            int malwareBlockListCount,
            int endUserBlockListCount,
            int bounceReflectorBlockListCount
        )

        {
            RecordId = recordId;
            EffectiveDate = effectiveDate;
            Domain = domain;
            ReverseDomain = reverseDomain;
            Provider = provider;
            OriginalProvider = originalProvider;
            ReporterOrgName = reporterOrgName;
            Ip = ip;
            Count = count;
            Disposition = disposition;
            Dkim = dkim;
            Spf = spf;
            EnvelopeTo = envelopeTo;
            EnvelopeFrom = envelopeFrom;
            HeaderFrom = headerFrom;
            OrganisationDomainFrom = organisationDomainFrom;
            SpfAuthResults = spfAuthResults;
            SpfPassCount = spfPassCount;
            SpfFailCount = spfFailCount;
            DkimAuthResults = dkimAuthResults;
            DkimPassCount = dkimPassCount;
            DkimFailCount = dkimFailCount;
            Forwarded = forwarded;
            SampledOut = sampledOut;
            TrustedForwarder = trustedForwarder;
            MailingList = mailingList;
            LocalPolicy = localPolicy;
            Arc = arc;
            OtherOverrideReason = otherOverrideReason;
            HostName = hostName;
            HostOrgDomain = hostOrgDomain;
            HostProvider = hostProvider;
            HostAsNumber = hostAsNumber;
            HostAsDescription = hostAsDescription;
            HostCountry = hostCountry;
            ProxyBlockListCount = proxyBlockListCount;
            SuspiciousNetworkBlockListCount = suspiciousNetworkBlockListCount;
            HijackedNetworkBlockListCount = hijackedNetworkBlockListCount;
            EndUserNetworkBlockListCount = endUserNetworkBlockListCount;
            SpamSourceBlockListCount = spamSourceBlockListCount;
            MalwareBlockListCount = malwareBlockListCount;
            EndUserBlockListCount = endUserBlockListCount;
            BounceReflectorBlockListCount = bounceReflectorBlockListCount;
        }
        
        public long RecordId { get; }
        public DateTime EffectiveDate { get; }
        public string Domain { get; }
        public string ReverseDomain { get; }
        public string Provider { get; }
        public string OriginalProvider { get; }
        public string ReporterOrgName { get; }
        public string Ip { get; }
        public int Count { get; }
        public string Disposition { get; }
        public string Dkim { get; }
        public string Spf { get; }
        public string EnvelopeTo { get; }
        public string EnvelopeFrom { get; }
        public string HeaderFrom { get; }
        public string OrganisationDomainFrom { get; }
        public string SpfAuthResults { get; }
        public int SpfPassCount { get; }
        public int SpfFailCount { get; }
        public string DkimAuthResults { get; }
        public int DkimPassCount { get; }
        public int DkimFailCount { get; }
        public int Forwarded { get; }
        public int SampledOut { get; }
        public int TrustedForwarder { get; }
        public int MailingList { get; }
        public int LocalPolicy { get; }
        public int Arc { get; }
        public int OtherOverrideReason { get; }
        public string HostName { get; }
        public string HostOrgDomain { get; }
        public string HostProvider { get; }
        public int HostAsNumber { get; }
        public string HostAsDescription { get; }
        public string HostCountry { get; }
        public int ProxyBlockListCount { get; }
        public int SuspiciousNetworkBlockListCount { get; }
        public int HijackedNetworkBlockListCount { get; }
        public int EndUserNetworkBlockListCount { get; }
        public int SpamSourceBlockListCount { get; }
        public int MalwareBlockListCount { get; }
        public int EndUserBlockListCount { get; }
        public int BounceReflectorBlockListCount { get; }
    }
}
