using System;
using System.Linq;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Enricher.ProviderResolver;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;

namespace MailCheck.Intelligence.Enricher
{
    public interface IAggregateReportRecordEnrichedFactory
    {
        AggregateReportRecordEnriched Create(IpAddressDetails response, AggregateReportRecord source, string orgDomain,
            string correlationId, string causationId);
    }

    public class AggregateReportRecordEnrichedFactory : IAggregateReportRecordEnrichedFactory
    {
        private readonly IProviderResolver _providerResolver;

        public AggregateReportRecordEnrichedFactory(IProviderResolver providerResolver)
        {
            _providerResolver = providerResolver;
        }

        public AggregateReportRecordEnriched Create(IpAddressDetails response, AggregateReportRecord source, string orgDomain,
            string correlationId, string causationId)
        {
            return new AggregateReportRecordEnriched(
                Guid.NewGuid().ToString(),
                source.Id,
                correlationId,
                causationId,
                source.ReporterOrgName,
                source.ReportId,
                source.EffectiveDate,
                source.DomainFrom,
                source.Adkim,
                source.Aspf,
                source.P,
                source.Sp,
                source.Pct,
                source.Fo,
                source.HostSourceIp,
                source.Count,
                source.Disposition,
                source.Dkim,
                source.Spf,
                source.EnvelopeTo,
                source.EnvelopeFrom,
                source.HeaderFrom,
                orgDomain,
                source.SpfAuthResults,
                source.SpfPassCount,
                source.SpfFailCount,
                source.DkimAuthResults,
                source.DkimPassCount,
                source.DkimFailCount,
                source.Forwarded,
                source.SampledOut,
                source.TrustedForwarder,
                source.MailingList,
                source.LocalPolicy,
                source.Arc,
                source.OtherOverrideReason,
                response?.ReverseDnsResponses != null && response.ReverseDnsResponses.Any() ? response.ReverseDnsResponses.OrderBy(x => x.Host).First().Host : "",
                response?.ReverseDnsResponses != null && response.ReverseDnsResponses.Any() ? response.ReverseDnsResponses.OrderBy(x => x.Host).First().OrganisationalDomain : "",
                _providerResolver.GetProvider(response),
                response?.AsNumber ?? 0,
                response?.Description,
                response?.CountryCode,
                response?.BlockListOccurrences?.Count(x => x.Flag == "proxy") ?? 0,
                response?.BlockListOccurrences?.Count(x => x.Flag == "suspiciousnetwork") ?? 0,
                response?.BlockListOccurrences?.Count(x => x.Flag == "highjackednetwork") ?? 0,
                response?.BlockListOccurrences?.Count(x => x.Flag == "endusernetwork") ?? 0,
                response?.BlockListOccurrences?.Count(x => x.Flag == "spamsource") ?? 0,
                response?.BlockListOccurrences?.Count(x => x.Flag == "malware") ?? 0,
                response?.BlockListOccurrences?.Count(x => x.Flag == "enduser") ?? 0,
                response?.BlockListOccurrences?.Count(x => x.Flag == "bouncereflector") ?? 0);
        }
    }
}