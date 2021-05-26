using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.DomDatProIpDkim.Dao;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.DomDatProIpDkim
{
    public class RecordHandler : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IDomDatProIpDkimAggregatorDao _aggregatorDao;

        public RecordHandler(IDomDatProIpDkimAggregatorDao aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Handle(AggregateReportRecordEnriched aggregateReportRecord)
        {
            await _aggregatorDao.Save(aggregateReportRecord.ToDomDatProIpDkimRecord());
        }
    }
}