using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;

namespace MailCheck.AggregateReport.Parser.Persistence
{
    internal class AggregateReportPersistorComposite : IAggregateReportPersistorComposite
    {
        private readonly IEnumerable<IAggregateReportPersistor> _pesistors;

        public AggregateReportPersistorComposite(IEnumerable<IAggregateReportPersistor> pesistors)
        {
            _pesistors = pesistors;
        }

        public async Task<AggregateReportInfo> Persist(AggregateReportInfo aggregateReportInfo)
        {
            foreach (IAggregateReportPersistor persistor in _pesistors)
            {
                aggregateReportInfo = await persistor.Persist(aggregateReportInfo);
            }

            return aggregateReportInfo;
        }
    }
}