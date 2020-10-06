using MailCheck.AggregateReport.Api.V2.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailCheck.AggregateReport.Api.V2.Dao
{
    public static class AggregateReportApiDaoExtensions
    {
        public static async Task<List<DomainStats>> GetDomainStatsDense(this IAggregateReportApiDao dao, string domain, DateTime startDate, DateTime endDate, bool rollup = false, string categoryFilter = null)
        {
            List<DomainStats> domainStats = await dao.GetDomainStats(domain, startDate, endDate, rollup, categoryFilter);
            return domainStats
                .FillDateRange(startDate, endDate, ds => ds?.Date ?? DateTime.MinValue, d => DomainStats.CreateEmpty(domain, d))
                .ToList();
        }
    }
}
