using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Mapping;

namespace MailCheck.AggregateReport.Parser.Persistence
{
    internal class DenormalisedRecordPersistorAdaptor : IAggregateReportPersistor
    {
        private readonly IDenormalisedRecordPersistorComposite _composite;

        public DenormalisedRecordPersistorAdaptor(IDenormalisedRecordPersistorComposite composite)
        {
            _composite = composite;
        }

        public async Task<AggregateReportInfo> Persist(AggregateReportInfo aggregateReportInfo)
        {
            List<DenormalisedRecord> records = aggregateReportInfo.ToDenormalisedRecord();

            await _composite.Persist(records);

            return aggregateReportInfo;
        }
    }
}