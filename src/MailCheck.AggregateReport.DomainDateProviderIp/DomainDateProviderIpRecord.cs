using MailCheck.AggregateReport.Common.Aggregators;
using System;

namespace MailCheck.AggregateReport.DomainDateProviderIp
{
    public class DomainDateProviderIpRecord : IRecord
    {
        public DomainDateProviderIpRecord(
            long id,
            string domain,
            DateTime date,
            string provider,
            string originalProvider,
            string ip,
            string hostname,
            long spfPassDkimPassNone,
            long spfPassDkimFailNone,
            long spfFailDkimPassNone,
            long spfFailDkimFailNone,
            long spfPassDkimPassQuarantine,
            long spfPassDkimFailQuarantine,
            long spfFailDkimPassQuarantine,
            long spfFailDkimFailQuarantine,
            long spfPassDkimPassReject,
            long spfPassDkimFailReject,
            long spfFailDkimPassReject,
            long spfFailDkimFailReject,
            long spfMisaligned,
            long dkimMisaligned,
            int proxyBlockListCount,
            int suspiciousNetworkBlockListCount,
            int hijackedNetworkBlockListCount,
            int endUserNetworkBlockListCount,
            int spamSourceBlockListCount,
            int malwareBlockListCount,
            int endUserBlockListCount,
            int bounceReflectorBlockListCount,
            long forwarded,
            long sampledOut,
            long trustedForwarder,
            long mailingList,
            long localPolicy,
            long arc,
            long otherOverrideReason)
        {
            Id = id;
            Domain = domain;
            Date = date;
            Provider = provider;
            OriginalProvider = originalProvider;
            Ip = ip;
            Hostname = hostname;
            SpfPassDkimPassNone = spfPassDkimPassNone;
            SpfPassDkimFailNone = spfPassDkimFailNone;
            SpfFailDkimPassNone = spfFailDkimPassNone;
            SpfFailDkimFailNone = spfFailDkimFailNone;
            SpfPassDkimPassQuarantine = spfPassDkimPassQuarantine;
            SpfPassDkimFailQuarantine = spfPassDkimFailQuarantine;
            SpfFailDkimPassQuarantine = spfFailDkimPassQuarantine;
            SpfFailDkimFailQuarantine = spfFailDkimFailQuarantine;
            SpfPassDkimPassReject = spfPassDkimPassReject;
            SpfPassDkimFailReject = spfPassDkimFailReject;
            SpfFailDkimPassReject = spfFailDkimPassReject;
            SpfFailDkimFailReject = spfFailDkimFailReject;

            SpfMisaligned = spfMisaligned;
            DkimMisaligned = dkimMisaligned;
            
            ProxyBlockListCount = proxyBlockListCount;
            SuspiciousNetworkBlockListCount = suspiciousNetworkBlockListCount;
            HijackedNetworkBlockListCount = hijackedNetworkBlockListCount;
            EndUserNetworkBlockListCount = endUserNetworkBlockListCount;
            SpamSourceBlockListCount = spamSourceBlockListCount;
            MalwareBlockListCount = malwareBlockListCount;
            EndUserBlockListCount = endUserBlockListCount;
            BounceReflectorBlockListCount = bounceReflectorBlockListCount;
            
            Forwarded = forwarded;
            SampledOut = sampledOut;
            TrustedForwarder = trustedForwarder;
            MailingList = mailingList;
            LocalPolicy = localPolicy;
            Arc = arc;
            OtherOverrideReason = otherOverrideReason;
        }
        public DateTime Date { get; }
        public string Provider { get; }
        public string OriginalProvider { get; }
        public long Id { get; }
        public string Domain { get; }
        public string Ip { get; }
        public string Hostname { get; }
        public long SpfPassDkimPassNone { get; }
        public long SpfPassDkimFailNone { get; }
        public long SpfFailDkimPassNone { get; }
        public long SpfFailDkimFailNone { get; }
        public long SpfPassDkimPassQuarantine { get; }
        public long SpfPassDkimFailQuarantine { get; }
        public long SpfFailDkimPassQuarantine { get; }
        public long SpfFailDkimFailQuarantine { get; }
        public long SpfPassDkimPassReject { get; }
        public long SpfPassDkimFailReject { get; }
        public long SpfFailDkimPassReject { get; }
        public long SpfFailDkimFailReject { get; }
        public long SpfMisaligned { get; }
        public long DkimMisaligned { get; }
        public int ProxyBlockListCount { get; }
        public int SuspiciousNetworkBlockListCount { get; }
        public int HijackedNetworkBlockListCount { get; }
        public int EndUserNetworkBlockListCount { get; }
        public int SpamSourceBlockListCount { get; }
        public int MalwareBlockListCount { get; }
        public int EndUserBlockListCount { get; }
        public int BounceReflectorBlockListCount { get; }
        public long Forwarded { get; }
        public long SampledOut { get; }
        public long TrustedForwarder { get; }
        public long MailingList { get; }
        public long LocalPolicy { get; }
        public long Arc { get; }
        public long OtherOverrideReason { get; }
    }
}
