using System;
using System.Collections.Generic;

namespace MailCheck.AggregateReport.Contracts
{
    public class AggregateReportRecordEnriched : AggregateReportRecord
    {
        public AggregateReportRecordEnriched(
            string id,
            string recordId,
            string correlationId,
            string causationId,
            string reporterOrgName,
            string reportId,
            DateTime effectiveDate,
            string domainFrom,
            Alignment? adkim,
            Alignment? aspf,
            Policy p,
            Policy? sp,
            int? pct,
            string fo,
            string hostSourceIp,
            int count,
            Policy? disposition,
            DmarcResult? dkim,
            DmarcResult? spf,
            string envelopeTo,
            string envelopeFrom,
            string headerFrom,
            string organisationDomainFrom,
            List<string> spfAuthResults,
            int spfPassCount,
            int spfFailCount,
            List<string> dkimAuthResults,
            int dkimPassCount,
            int dkimFailCount,
            bool forwarded, //policy override reason could be a flags type
            bool sampledOut,
            bool trustedForwarder,
            bool mailingList,
            bool localPolicy,
            bool arc,
            bool otherOverrideReason,
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
        ) : base(id, reporterOrgName, reportId, effectiveDate, domainFrom, adkim, aspf, p, sp, pct, fo, hostSourceIp, count, disposition, dkim, spf, envelopeTo, envelopeFrom, headerFrom, spfAuthResults, spfPassCount, spfFailCount, dkimAuthResults, dkimPassCount, dkimFailCount, forwarded, sampledOut, trustedForwarder, mailingList, localPolicy, arc, otherOverrideReason)

        {
            CausationId = causationId;
            CorrelationId = correlationId;
            RecordId = recordId;
            OrganisationDomainFrom = organisationDomainFrom;
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

        public string RecordId { get; }
        public string OrganisationDomainFrom { get; }
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