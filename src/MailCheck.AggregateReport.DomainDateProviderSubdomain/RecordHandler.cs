using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.DomainDateProviderSubdomain.Dao;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.DomainDateProviderSubdomain
{
    public class RecordHandler : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IDateDomainProviderSubdomainAggregatorDao _aggregatorDao;

        public RecordHandler(IDateDomainProviderSubdomainAggregatorDao aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Handle(AggregateReportRecordEnriched aggregateReportRecord)
        {
            await _aggregatorDao.Save(aggregateReportRecord.ToDomainDateProviderSubdomainRecord());
        }
    }
}