using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;

namespace MailCheck.AggregateReport.Api.V2.Mappers
{
    public interface ISubdomainStatsMapper
    {
        SubdomainStatsDto Map(SubdomainStats subdomainStats, string providerAlias, string providerMarkdown);
    }

    public class SubdomainStatsMapper : ISubdomainStatsMapper
    {
        public SubdomainStatsDto Map(SubdomainStats subdomainStats, string providerAlias,  string providerMarkdown)
        {
            return new SubdomainStatsDto(
                subdomainStats.Domain, subdomainStats.Provider, subdomainStats.Subdomain, 
                subdomainStats.SpfPassDkimPassNone, subdomainStats.SpfPassDkimFailNone, subdomainStats.SpfFailDkimPassNone, subdomainStats.SpfFailDkimFailNone,
                subdomainStats.SpfPassDkimPassQuarantine, subdomainStats.SpfPassDkimFailQuarantine, subdomainStats.SpfFailDkimPassQuarantine, subdomainStats.SpfFailDkimFailQuarantine, 
                subdomainStats.SpfPassDkimPassReject, subdomainStats.SpfPassDkimFailReject, subdomainStats.SpfFailDkimPassReject, subdomainStats.SpfFailDkimFailReject, 
                subdomainStats.TotalEmails, subdomainStats.FailSpfTotal, subdomainStats.FailDkimTotal, 
                providerAlias, providerMarkdown);
        }
    }
}