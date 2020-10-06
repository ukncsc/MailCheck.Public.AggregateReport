using System.Collections.Generic;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.Contracts
{
    public class DkimSelectorsSeen :  Message
    {
        public DkimSelectorsSeen(string correlationId, string causationId,
            string id, List<string> selectors) : base(id)
        {
            CorrelationId = correlationId;
            CausationId = causationId;
            Selectors = selectors;
        }

        public List<string> Selectors { get; }
    }
}
