using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.Common.Messaging.Abstractions;
using Microsoft.Extensions.Logging;

namespace MailCheck.AggregateReport.Common.Aggregators
{
    public class AggregateReportLoggingHandlerWrapper : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IHandle<AggregateReportRecordEnriched> _inner;
        private readonly ILogger<AggregateReportLoggingHandlerWrapper> _logger;

        public AggregateReportLoggingHandlerWrapper(IHandle<AggregateReportRecordEnriched> inner, ILogger<AggregateReportLoggingHandlerWrapper> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task Handle(AggregateReportRecordEnriched aggregateReportRecord)
        {
            string domain = aggregateReportRecord.HeaderFrom?.Trim().Trim('.').ToLower() ??
                            aggregateReportRecord.DomainFrom.ToLower();
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["AggregateReportInfoId"] = aggregateReportRecord.Id,
                ["AggregateReportId"] = aggregateReportRecord.ReportId,
                ["AggregateReportDomain"] = domain
            }))
            {
                _logger.LogInformation($"Saving aggregate report for domain: {domain} - report id: {aggregateReportRecord.ReportId}.");
                await _inner.Handle(aggregateReportRecord);
                _logger.LogInformation($"Saved aggregate report for domain: {domain} - report id: {aggregateReportRecord.ReportId}.");
            }
        }
    }
}
