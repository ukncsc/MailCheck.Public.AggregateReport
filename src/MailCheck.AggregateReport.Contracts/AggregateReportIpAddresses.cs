using System;
using System.Collections.Generic;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.Contracts
{
    public class AggregateReportIpAddresses : Message
    {
        public AggregateReportIpAddresses(string correlationId, string causationId, DateTime effectiveDate, List<string> ipAddresses) : base(Guid.NewGuid().ToString())
        {
            EffectiveDate = effectiveDate;
            IpAddresses = ipAddresses;
            CorrelationId = correlationId;
            CausationId = causationId;
            MessageId = Id;
        }

        public DateTime EffectiveDate { get; }
        public List<string> IpAddresses { get; }
    }
}
