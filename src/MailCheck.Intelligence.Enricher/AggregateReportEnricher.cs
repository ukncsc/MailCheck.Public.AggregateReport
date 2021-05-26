using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Util;
using MailCheck.Intelligence.Enricher.Config;
using MailCheck.Intelligence.Enricher.ReverseDns.Domain;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.Enricher
{
    public class AggregateReportEnricher : IHandle<AggregateReportRecordBatch>
    {
        private readonly IIpAddressProcessor _ipAddressProcessor;
        private readonly IMessagePublisher _publisher;
        private readonly IAggregateReportRecordEnrichedFactory _aggregateReportRecordEnrichedFactory;
        private readonly IEnricherConfig _enricherConfig;
        private readonly IOrganisationalDomainProvider _organisationalDomainProvider;
        private readonly ILogger<AggregateReportEnricher> _log;

        public AggregateReportEnricher(IIpAddressProcessor ipAddressProcessor, IMessagePublisher publisher, IAggregateReportRecordEnrichedFactory aggregateReportRecordEnrichedFactory, IEnricherConfig enricherConfig, IOrganisationalDomainProvider organisationalDomainProvider, ILogger<AggregateReportEnricher> log)
        {
            _ipAddressProcessor = ipAddressProcessor;
            _publisher = publisher;
            _aggregateReportRecordEnrichedFactory = aggregateReportRecordEnrichedFactory;
            _enricherConfig = enricherConfig;
            _organisationalDomainProvider = organisationalDomainProvider;
            _log = log;
        }

        public async Task Handle(AggregateReportRecordBatch message)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _log.LogInformation($"Enricher received batch of {message.Records?.Count ?? 0} aggregate reports");
            List<IpAddressDetailsRequest> requests = message
                .Records.Select(x => new IpAddressDetailsRequest(x.HostSourceIp, x.EffectiveDate))
                .ToList();

            List<IpAddressDetails> responses = await _ipAddressProcessor.Process(requests);

            List<AggregateReportRecordEnriched> enrichedReports = new List<AggregateReportRecordEnriched>();
            foreach (AggregateReportRecord aggregateReportRecord in message.Records)
            {
                IpAddressDetails ipAddressDetails = responses.FirstOrDefault(x => x.IpAddress == aggregateReportRecord.HostSourceIp);
                if (ipAddressDetails is null)
                {
                    _log.LogInformation($"Unable to enrich message for ip {aggregateReportRecord.HostSourceIp} and date {aggregateReportRecord.EffectiveDate}");
                }

                OrganisationalDomain organisationalDomain  = await _organisationalDomainProvider.GetOrganisationalDomain(aggregateReportRecord.HeaderFrom.Trim().Trim('.').ToLower());

                AggregateReportRecordEnriched aggregateReportRecordEnriched = _aggregateReportRecordEnrichedFactory.Create(ipAddressDetails, aggregateReportRecord, organisationalDomain.OrgDomain, message.CorrelationId, message.Id);
                enrichedReports.Add(aggregateReportRecordEnriched);
            }

            foreach (AggregateReportRecordEnriched aggregateReportRecordEnriched in enrichedReports)
            {
                await _publisher.Publish(aggregateReportRecordEnriched, _enricherConfig.SnsTopicArn);
            }

            _log.LogInformation($"Enricher published batch of {enrichedReports.Count} enriched aggregate reports from request for {message.Records.Count} in {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Stop();
        }
    }
}
