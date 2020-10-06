using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;

namespace MailCheck.AggregateReport.Parser.Persistence
{
    internal class DenormalisedRecordPersistorComposite : IDenormalisedRecordPersistorComposite
    {
        private readonly IEnumerable<IDenormalisedRecordPersistor> _persistors;

        public DenormalisedRecordPersistorComposite(IEnumerable<IDenormalisedRecordPersistor> persistors)
        {
            _persistors = persistors;
        }

        public Task Persist(List<DenormalisedRecord> records)
        {
            return Task.WhenAll(_persistors.Select(_ => _.Persist(records)));
        }
    }
}