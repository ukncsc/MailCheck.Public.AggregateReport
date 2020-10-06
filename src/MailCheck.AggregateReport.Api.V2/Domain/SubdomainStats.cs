using System.Collections.Generic;

namespace MailCheck.AggregateReport.Api.V2.Domain
{
    public class SubdomainStatsResult
    {
        public SubdomainStatsResult(SubdomainStats domainStats, List<SubdomainStats> subdomainStats, int totalCount)
        {
            DomainStats = domainStats;
            SubdomainStats = subdomainStats;
            TotalCount = totalCount;
        }

        public SubdomainStats DomainStats { get; }
        public List<SubdomainStats> SubdomainStats { get; }
        public int TotalCount { get; }
    }


    public class SubdomainStats
    {
        public SubdomainStats(
            string domain, 
            string provider, 
            string subdomain, 
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
            long totalEmails,
            long failSpfTotal,
            long failDkimTotal)
        {
            Domain = domain;
            Provider = provider;
            Subdomain = subdomain;
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
            TotalEmails = totalEmails;
            FailSpfTotal = failSpfTotal;
            FailDkimTotal = failDkimTotal;
        }

        public string Domain { get; }
        public string Provider { get; }
        public string Subdomain { get; }
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
        public long TotalEmails { get; }
        public long FailSpfTotal { get; }
        public long FailDkimTotal { get; }
    }
}