using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.DomainDateProviderIp.Dao;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.DomainDateProviderIp
{
    public class RecordHandler : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IDateDomainProviderIpAggregatorDao _aggregatorDao;

        public RecordHandler(IDateDomainProviderIpAggregatorDao aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Handle(AggregateReportRecordEnriched aggregateReportRecord)
        {
            await _aggregatorDao.Save(aggregateReportRecord.ToDomainDateProviderIpRecord());
        }
    }
}