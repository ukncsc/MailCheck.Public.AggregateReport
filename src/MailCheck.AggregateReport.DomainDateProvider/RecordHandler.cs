using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.DomainDateProvider.Dao;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.DomainDateProvider
{
    public class RecordHandler : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IDateDomainProviderAggregatorDao _aggregatorDao;

        public RecordHandler(IDateDomainProviderAggregatorDao aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Handle(AggregateReportRecordEnriched aggregateReportRecord)
        {
            await _aggregatorDao.Save(aggregateReportRecord.ToDomainDateProviderRecord());
        }
    }
}