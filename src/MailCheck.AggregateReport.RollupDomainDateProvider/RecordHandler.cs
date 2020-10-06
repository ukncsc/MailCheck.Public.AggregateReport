using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.RollupDomainDateProvider.Dao;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.RollupDomainDateProvider
{
    public class RecordHandler : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IDateDomainProviderRollUpAggregatorDao _aggregatorDao;

        public RecordHandler(IDateDomainProviderRollUpAggregatorDao aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Handle(AggregateReportRecordEnriched aggregateReportRecord)
        {
            await _aggregatorDao.Save(aggregateReportRecord.ToDomainDateProviderRecord());
        }
    }
}
