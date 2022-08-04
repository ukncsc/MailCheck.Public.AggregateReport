using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Intelligence.Enricher.Config;
using MailCheck.Intelligence.Enricher.ReverseDns.Domain;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.Enricher
{
    public class AggregateReportEnricher : IHandle<AggregateReportRecordBatch>
    {
        private static readonly List<IpAddressDetails> EmptyIpAddressDetailsList = new List<IpAddressDetails>();

        private readonly IIpAddressProcessor _ipAddressProcessor;
        private readonly IMessageDispatcher _dispatcher;
        private readonly IAggregateReportRecordEnrichedFactory _aggregateReportRecordEnrichedFactory;
        private readonly IEnricherConfig _enricherConfig;
        private readonly IOrganisationalDomainProvider _organisationalDomainProvider;
        private readonly ILogger<AggregateReportEnricher> _log;

        public AggregateReportEnricher(IIpAddressProcessor ipAddressProcessor, IMessageDispatcher dispatcher, IAggregateReportRecordEnrichedFactory aggregateReportRecordEnrichedFactory, IEnricherConfig enricherConfig, IOrganisationalDomainProvider organisationalDomainProvider, ILogger<AggregateReportEnricher> log)
        {
            _ipAddressProcessor = ipAddressProcessor;
            _dispatcher = dispatcher;
            _aggregateReportRecordEnrichedFactory = aggregateReportRecordEnrichedFactory;
            _enricherConfig = enricherConfig;
            _organisationalDomainProvider = organisationalDomainProvider;
            _log = log;
        }

        public async Task Handle(AggregateReportRecordBatch message)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _log.LogInformation($"Enricher received batch of {message.Records?.Count ?? 0} aggregate reports");

            if (message.Records == null || message.Records.Count == 0)
            {
                _log.LogInformation($"Enricher received batch containing no records - exiting");
                return;
            }

            var firstRecord = message.Records[0];

            using (_log.BeginScope(new Dictionary<string, object>
            {
                ["ReporterOrgName"] = firstRecord.ReporterOrgName,
                ["ReportId"] = firstRecord.ReportId,
                ["EffectiveDate"] = firstRecord.EffectiveDate,
                ["DomainFrom"] = firstRecord.DomainFrom
            }))
            {
                List<IpAddressDetailsRequest> requests = message
                    .Records.Select(x => new IpAddressDetailsRequest(x.HostSourceIp, x.EffectiveDate))
                    .ToList();

                List<IpAddressDetails> responses = await _ipAddressProcessor.Process(requests);

                var groupedResponses = responses
                    .GroupBy(x => x.ReverseDnsInconclusive)
                    .ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());

                groupedResponses.TryGetValue(false, out List<IpAddressDetails> validResponses);
                groupedResponses.TryGetValue(true, out List<IpAddressDetails> inconclustiveResponses);

                validResponses = validResponses ?? EmptyIpAddressDetailsList;
                inconclustiveResponses = inconclustiveResponses ?? EmptyIpAddressDetailsList;

                List<AggregateReportRecordEnriched> enrichedReports = new List<AggregateReportRecordEnriched>();

                foreach (AggregateReportRecord aggregateReportRecord in message.Records)
                {
                    IpAddressDetails ipAddressDetails = validResponses.FirstOrDefault(x => x.IpAddress == aggregateReportRecord.HostSourceIp);
                    if (ipAddressDetails is null)
                    {
                        _log.LogInformation($"Unable to enrich message for ip {aggregateReportRecord.HostSourceIp} and date {aggregateReportRecord.EffectiveDate}");
                        continue; // do not publish unenriched records
                    }

                    OrganisationalDomain organisationalDomain = await _organisationalDomainProvider.GetOrganisationalDomain(aggregateReportRecord.HeaderFrom.Trim().Trim('.').ToLower());

                    AggregateReportRecordEnriched aggregateReportRecordEnriched = _aggregateReportRecordEnrichedFactory.Create(ipAddressDetails, aggregateReportRecord, organisationalDomain.OrgDomain, message.CorrelationId, message.Id);
                    enrichedReports.Add(aggregateReportRecordEnriched);
                }

                foreach (AggregateReportRecordEnriched aggregateReportRecordEnriched in enrichedReports)
                {
                    _dispatcher.Dispatch(aggregateReportRecordEnriched, _enricherConfig.SnsTopicArn);
                }

                _log.LogInformation($"Enricher published batch of {enrichedReports.Count} enriched aggregate reports from request for {message.Records.Count} in {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Stop();

                if (inconclustiveResponses.Count > 0)
                {
                    _log.LogWarning($"Inconclusive reverse DNS lookup found in batch for IPs: {string.Join(", ", inconclustiveResponses.Select(response => response.IpAddress))}");
                    throw new Exception("Inconclusive reverse DNS lookup found in batch. All valid lookups have been published.");
                }
            }
        }
    }
}
