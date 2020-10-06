using System;
using System.Collections.Generic;
using System.Text;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;

namespace MailCheck.AggregateReport.Api.V2.Mappers
{
    public interface IProviderStatsMapper
    {
        ProviderStatsDto Map(ProviderStats providerStats, string providerAlias, string providerMarkdown);

    }
    public class ProviderStatsMapper : IProviderStatsMapper
    {
        public ProviderStatsDto Map(ProviderStats providerStats, string providerAlias, string providerMarkdown)
        {
            return new ProviderStatsDto(providerStats.Domain, providerStats.Provider, providerStats.SpfPassDkimPassNone,
                providerStats.SpfPassDkimFailNone, providerStats.SpfFailDkimPassNone, providerStats.SpfFailDkimFailNone,
                providerStats.SpfPassDkimPassQuarantine,
                providerStats.SpfPassDkimFailQuarantine, providerStats.SpfFailDkimPassQuarantine,
                providerStats.SpfFailDkimFailQuarantine, providerStats.SpfPassDkimPassReject,
                providerStats.SpfPassDkimFailReject, providerStats.SpfFailDkimPassReject,
                providerStats.SpfFailDkimFailReject,
                providerStats.FullyTrusted, providerStats.PartiallyTrusted, providerStats.Untrusted, providerStats.Quarantined, providerStats.Rejected,
                providerStats.TotalEmails,
                providerStats.FailSpfTotal, providerStats.FailDkimTotal, 
                providerAlias, providerMarkdown);
        }
    }
}
