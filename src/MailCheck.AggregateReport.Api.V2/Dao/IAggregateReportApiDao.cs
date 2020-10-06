using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Api.V2.Domain;

namespace MailCheck.AggregateReport.Api.V2.Dao
{
    public interface IAggregateReportApiDao
    {
        Task<List<DomainStats>> GetDomainStats(string domain, DateTime startDate, DateTime endDate, bool rollup = false, string categoryFilter = null);

        Task<StatsSummary> GetDomainStatsSummary(string domain, DateTime startDate, DateTime endDate, bool rollup = false, string categoryFilter = null);
        
        Task<ProviderStatsResult> GetProviderStats(string domain, DateTime startDate, DateTime endDate, int page, int pageSize, bool rollup = false, string categoryFilter = null, string providerFilter = null);

        Task<StatsSummary> GetProviderStatsSummary(string domain, DateTime startDate, DateTime endDate, int page, int pageSize, bool rollup = false, string categoryFilter = null, string providerFilter = null);

        Task<SubdomainStatsResult> GetSubdomainStats(string domain, string provider, DateTime startDate, DateTime endDate, int page, int pageSize, string categoryFilter = null);

        Task<IpStatsResult> GetIpStats(string domain, DateTime startDate, DateTime endDate, string provider, int page, int pageSize, string ipFilter = null, string hostFilter = null, string categoryFilter = null);
        
        Task<StatsSummary> GetIpStatsSummary(string domain, DateTime startDate, DateTime endDate, string provider, string categoryFilter = null, string hostFilter = null);
        
        Task<List<SpfDomainStats>> GetSpfDomainStats(string domain, string provider, string ip, DateTime startDate,
            DateTime endDate);
        
        Task<List<DkimDomainStats>> GetDkimDomainStats(string domain, string provider, string ip, DateTime startDate,
            DateTime endDate);
        Task<List<AggregateReportExportStats>> GetAggregateReportExport(string domain, string provider, string ip, DateTime startDate,
            DateTime endDate, bool includeSubdomains);
    }
}