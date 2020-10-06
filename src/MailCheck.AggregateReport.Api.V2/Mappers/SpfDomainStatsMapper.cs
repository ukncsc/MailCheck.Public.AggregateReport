using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;

namespace MailCheck.AggregateReport.Api.V2.Mappers
{
    public interface ISpfDomainStatsMapper
    {
        SpfDomainStatsDto Map(SpfDomainStats spfDomainStats, string providerAlias, string providerMarkdown);
    }

    public class SpfDomainStatsMapper : ISpfDomainStatsMapper
    {
        public SpfDomainStatsDto Map(SpfDomainStats spfDomainStats, string providerAlias,  string providerMarkdown)
        {
            return new SpfDomainStatsDto(
                spfDomainStats.Domain, spfDomainStats.Provider, spfDomainStats.Ip, spfDomainStats.SpfDomain, 
                spfDomainStats.SpfPass, spfDomainStats.SpfFail, providerAlias, providerMarkdown);
        }
    }
}