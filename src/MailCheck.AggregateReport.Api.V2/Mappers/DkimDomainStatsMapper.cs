using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;

namespace MailCheck.AggregateReport.Api.V2.Mappers
{
    public interface IDkimDomainStatsMapper
    {
        DkimDomainStatsDto Map(DkimDomainStats dkimDomainStats, string providerAlias, string providerMarkdown);
    }

    public class DkimDomainStatsMapper : IDkimDomainStatsMapper
    {
        public DkimDomainStatsDto Map(DkimDomainStats dkimDomainStats, string providerAlias,  string providerMarkdown)
        {
            return new DkimDomainStatsDto(
                dkimDomainStats.Domain, dkimDomainStats.Provider, dkimDomainStats.Ip, dkimDomainStats.DkimDomain, dkimDomainStats.DkimSelector,
                dkimDomainStats.DkimPass, dkimDomainStats.DkimFail, providerAlias, providerMarkdown);
        }
    }
}