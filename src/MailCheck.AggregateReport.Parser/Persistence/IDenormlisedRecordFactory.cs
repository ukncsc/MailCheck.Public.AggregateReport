using System.Collections.Generic;
using MailCheck.AggregateReport.Parser.Domain;

namespace MailCheck.AggregateReport.Parser.Persistence
{
    public interface IDenormlisedRecordFactory
    {
        List<DenormalisedRecord> Create(AggregateReportInfo aggregateReportInfo);
    }
}