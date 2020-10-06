using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.RollupDomainDate.Dao;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.RollupDomainDate
{
    public class RecordHandler : IHandle<AggregateReportRecordEnriched>
    {
        private readonly IDomainDateAggregatorDao _aggregatorDao;

        public RecordHandler(IDomainDateAggregatorDao aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Handle(AggregateReportRecordEnriched aggregateReportRecord)
        {
            await _aggregatorDao.Save(aggregateReportRecord.ToDomainDateRecords());
        }
    }
}