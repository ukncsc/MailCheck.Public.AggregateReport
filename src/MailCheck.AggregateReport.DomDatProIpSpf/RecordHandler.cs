using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.DomDatProIpSpf.Dao;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.DomDatProIpSpf
{
    public class RecordHandler : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IDomDatProIpSpfAggregatorDao _aggregatorDao;

        public RecordHandler(IDomDatProIpSpfAggregatorDao aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Handle(AggregateReportRecordEnriched aggregateReportRecord)
        {
            await _aggregatorDao.Save(aggregateReportRecord.ToDomDatProIpSpfRecord());
        }
    }
}
