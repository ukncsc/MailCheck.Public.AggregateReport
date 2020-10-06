using System.Collections.Generic;

namespace MailCheck.AggregateReport.Api.V2.Dto
{
    public class IpStatsDtoResult
    {
        public IpStatsDtoResult(List<IpStatsDto> ipStatsDto, long totalHostnameCount, long totalEmails)
        {
            IpStats = ipStatsDto;
            TotalHostnameCount = totalHostnameCount;
            TotalEmails = totalEmails;
        }

        public List<IpStatsDto> IpStats { get; }
        public long TotalHostnameCount { get; }
        public long TotalEmails { get; }
    }
    
    public class IpStatsDto
    {
        public IpStatsDto(
            string domain, 
            string provider, 
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
            long fullyTrusted,
            long partiallyTrusted,
            long untrusted,
            long quarantined,
            long rejected,
            long delivered,
            long totalEmails,
            long passSpfTotal,
            long passDkimTotal,
            long failSpfTotal,
            long failDkimTotal,
            long spfMisaligned,
            long dkimMisaligned,
            int proxyBlockListCount,
            int suspiciousNetworkBlockListCount,
            int hijackedetworkBlockListCount,
            int endUserNetworkBlockListCount,
            int spamSourceBlockListCount,
            int malwareBlockListCount,
            int endUserBlockListCount,
            int bounceReflectorBlockListCount,
            int forwarded,
            int sampledOut,
            int trustedForwarder,
            int mailingList,
            int localPolicy,
            int arc,
            int otherOverrideReason,
            string providerAlias,
            string providerMarkdown
            )
        {
            Domain = domain;
            Provider = provider;
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
            FullyTrusted = fullyTrusted;
            PartiallyTrusted = partiallyTrusted;
            Untrusted = untrusted;
            Quarantined = quarantined;
            Rejected = rejected;
            Delivered = delivered;
            TotalEmails = totalEmails;
            PassSpfTotal = passSpfTotal;
            PassDkimTotal = passDkimTotal;
            FailSpfTotal = failSpfTotal;
            FailDkimTotal = failDkimTotal;
            SpfMisaligned = spfMisaligned;
            DkimMisaligned = dkimMisaligned;
            
            ProxyBlockListCount = proxyBlockListCount;
            SuspiciousNetworkBlockListCount=suspiciousNetworkBlockListCount;
            HijackedetworkBlockListCount = hijackedetworkBlockListCount;
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
            ProviderAlias = providerAlias;
            ProviderMarkdown = providerMarkdown;
        }

        public string Domain { get; }
        public string Provider { get; }
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
        public long FullyTrusted { get; }
        public long PartiallyTrusted { get; }
        public long Untrusted { get; }
        public long Quarantined { get; }
        public long Rejected { get; }
        public long Delivered { get; }
        public long TotalEmails { get; }
        public long PassSpfTotal { get; }
        public long PassDkimTotal { get; }
        public long FailSpfTotal { get; }
        public long FailDkimTotal { get; }
        public long SpfMisaligned { get; }
        public long DkimMisaligned { get; }
        public int ProxyBlockListCount { get; }
        public int SuspiciousNetworkBlockListCount { get; }
        public int HijackedetworkBlockListCount { get; }
        public int EndUserNetworkBlockListCount { get; }
        public int SpamSourceBlockListCount { get; }
        public int MalwareBlockListCount { get; }
        public int EndUserBlockListCount { get; }
        public int BounceReflectorBlockListCount { get; }
        public int Forwarded { get; }
        public int SampledOut { get; }
        public int TrustedForwarder { get; }
        public int MailingList { get; }
        public int LocalPolicy { get; }
        public int Arc { get; }
        public int OtherOverrideReason { get; }
        public string ProviderAlias { get; }
        public string ProviderMarkdown { get; }
    }
}