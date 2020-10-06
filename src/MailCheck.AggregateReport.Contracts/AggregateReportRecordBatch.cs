using System;
using System.Collections.Generic;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.Contracts
{
    public class AggregateReportRecordBatch : Message
    {
        public AggregateReportRecordBatch(string id, List<AggregateReportRecord> records)
            : base(id)
        {
            Records = records;
            CorrelationId = Guid.NewGuid().ToString();
        }

        public List<AggregateReportRecord> Records { get; }
    }
}