using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.EslrSaver.Dao;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.EslrSaver
{
    public class RecordHandler : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IEslrSaverDao _aggregatorDao;

        public RecordHandler(IEslrSaverDao aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Handle(AggregateReportRecordEnriched aggregateReportRecord)
        {
            await _aggregatorDao.Save(aggregateReportRecord.ToEslrSaverRecord());
        }
    }
}