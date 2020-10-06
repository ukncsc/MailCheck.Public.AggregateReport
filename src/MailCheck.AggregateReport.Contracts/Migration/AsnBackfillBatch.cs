using System.Collections.Generic;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.Contracts.Migration
{
    public class AsnBackfillBatch : Message
    {
        public AsnBackfillBatch(string id, List<string> ipsToBackfill) : base(id)
        {
            IpsToBackfill = ipsToBackfill;
        }

        public List<string> IpsToBackfill { get; }
    }
}