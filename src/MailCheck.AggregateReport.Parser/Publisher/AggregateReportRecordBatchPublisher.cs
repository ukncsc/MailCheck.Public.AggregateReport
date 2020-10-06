using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.Parser.Config;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Mapping;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Util;
using Microsoft.Extensions.Logging;

namespace MailCheck.AggregateReport.Parser.Publisher
{
    internal class AggregateReportRecordBatchPublisher : IAggregateReportMessagePublisher
    {
        private const int Concurrency = 8;
        private const int MaxBatchSizeBytes = 200000;

        private readonly IMessagePublisher _messagePublisher;
        private readonly IAggregateReportConfig _config;
        private readonly ILogger<AggregateReportRecordBatchPublisher> _log;

        public AggregateReportRecordBatchPublisher(
            IMessagePublisher messagePublisher,
            IAggregateReportConfig config,
            ILogger<AggregateReportRecordBatchPublisher> log)
        {
            _messagePublisher = messagePublisher;
            _config = config;
            _log = log;
        }

        public async Task Publish(AggregateReportInfo aggregateReportInfo)
        {
            List<AggregateReportRecord> aggregateReportRecords = aggregateReportInfo.ToAggregateReportRecords();

            if (aggregateReportRecords.Any(x=> string.IsNullOrEmpty(x.HostSourceIp)))
            {
                List<AggregateReportRecord> records = aggregateReportRecords.Where(x => string.IsNullOrEmpty(x.HostSourceIp)).ToList();
                List<string> recordIds = records.Select(x => x.Id).ToList();
                _log.LogError($"Error validating AggregateReportInfo. {recordIds.Count} row(s) missing Host IP with Record Ids: {string.Join(", ", recordIds)}. Publish aborted.");
                return;
            }

            List<AggregateReportRecordBatch> aggregateReportRecordBatches = aggregateReportRecords
                .BatchByJsonBytes(MaxBatchSizeBytes)
                .Select(_ => new AggregateReportRecordBatch(Guid.NewGuid().ToString(), _.ToList()))
                .ToList();

            _log.LogDebug($"Publishing {aggregateReportRecordBatches.Count} aggregate report record processed batch messages.");

            foreach (IEnumerable<AggregateReportRecordBatch> reportRecordBatches in aggregateReportRecordBatches.Batch(Concurrency))
            {
                await Task.WhenAll(reportRecordBatches.Select(_ => _messagePublisher.Publish(_, _config.SnsTopicArn)));
            }
        }
    }
}
