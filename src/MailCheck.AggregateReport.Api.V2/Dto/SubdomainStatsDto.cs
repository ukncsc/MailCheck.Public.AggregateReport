using System.Collections.Generic;

namespace MailCheck.AggregateReport.Api.V2.Dto
{
    public class SubdomainStatsDtoResult
    {
        public SubdomainStatsDtoResult(SubdomainStatsDto domainStatsDto, List<SubdomainStatsDto> subdomainStatsDto, int totalCount)
        {
            DomainStatsDto = domainStatsDto;
            SubdomainStatsDto = subdomainStatsDto;
            TotalCount = totalCount;
        }

        public SubdomainStatsDto DomainStatsDto { get; }
        public List<SubdomainStatsDto> SubdomainStatsDto { get; }
        public int TotalCount { get; }
    }
    
    public class SubdomainStatsDto
    {
        public SubdomainStatsDto(
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
            long failDkimTotal,
            string providerAlias,
            string providerMarkdown)
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
            ProviderAlias = providerAlias;
            ProviderMarkdown = providerMarkdown;
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
        public string ProviderAlias { get; }
        public string ProviderMarkdown { get; }
    }
}