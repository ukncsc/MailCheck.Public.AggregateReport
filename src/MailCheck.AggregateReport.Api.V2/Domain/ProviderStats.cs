using System.Collections.Generic;

namespace MailCheck.AggregateReport.Api.V2.Domain
{
    public class ProviderStatsResult
    {
        public ProviderStatsResult(List<ProviderStats> providerStats, int totalCount, int allProviderTotalCount)
        {
            ProviderStats = providerStats;
            TotalCount = totalCount;
            AllProviderTotalCount = allProviderTotalCount;
        }

        public List<ProviderStats> ProviderStats { get; }
        public int TotalCount { get; }
        public int AllProviderTotalCount { get; }
    }

    public class ProviderStats
    {
        public ProviderStats(
            string domain, 
            string provider, 
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
            long totalEmails,
            long failSpfTotal,
            long failDkimTotal)
        {
            Domain = domain;
            Provider = provider;
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
            TotalEmails = totalEmails;
            FailSpfTotal = failSpfTotal;
            FailDkimTotal = failDkimTotal;
        }

        public string Domain { get; }
        public string Provider { get; }
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
        public long TotalEmails { get; }
        public long FailSpfTotal { get; }
        public long FailDkimTotal { get; }
    }
}