using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Api.V2.Dao;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;
using MailCheck.AggregateReport.Api.V2.Extensions;
using MailCheck.AggregateReport.Api.V2.Mappers;
using MailCheck.AggregateReport.Api.V2.Provider;
using Microsoft.AspNetCore.Mvc;

namespace MailCheck.AggregateReport.Api.V2.Services
{
    public interface IAggregateReportService
    {
        Task<List<DomainStats>> GetDomainStatsDto(string domain, DateTime startDate, DateTime endDate, bool rollup = false, string categoryFilter= null);

        Task<ProviderStatsDtoResult> GetProviderStatsDto(string domain, DateTime startDate, DateTime endDate,
            int page, int pageSize, bool rollup = false, string categoryFilter = null, string providerFilter = null);

        Task<SubdomainStatsDtoResult> GetSubdomainStatsDto(string domain, string provider, DateTime startDate, DateTime endDate, int page, int pageSize, string categoryFilter = null);

        Task<IpStatsDtoResult> GetIpStatsDto(string domain, DateTime startDate, DateTime endDate, string provider, int page, int pageSize, string ip = null, string hostFilter = null, string categoryFilter = null);

        Task<List<SpfDomainStatsDto>> GetSpfDomainStatsDto(string domain, string provider, string ip, DateTime startDate,
            DateTime endDate);

        Task<List<DkimDomainStatsDto>> GetDkimDomainStatsDto(string domain, string provider, string ip, DateTime startDate,
            DateTime endDate);

        Task<IEnumerable<AggregateReportExportStats>> GetAggregateReportExport(string domain, string provider, string ip, DateTime startDate,
            DateTime endDate, bool includeSubdomains);
    }

    public class AggregateReportService : IAggregateReportService
    {
        private readonly IAggregateReportApiDao _aggregateReportApiDao;
        private readonly IProviderDetailsProvider _providerDetailsProvider;
        private readonly ISubdomainStatsMapper _subdomainStatsMapper;
        private readonly IIpStatsMapper _ipStatsMapper;
        private readonly IProviderStatsMapper _providerStatsMapper;
        private readonly ISpfDomainStatsMapper _spfDomainStatsMapper;
        private readonly IDkimDomainStatsMapper _dkimDomainStatsMapper;

        public AggregateReportService(IAggregateReportApiDao aggregateReportApiDao, IProviderDetailsProvider providerDetailsProvider, ISubdomainStatsMapper subdomainStatsMapper, IProviderStatsMapper providerStatsMapper, IIpStatsMapper ipStatsMapper, ISpfDomainStatsMapper spfDomainStatsMapper, IDkimDomainStatsMapper dkimDomainStatsMapper)
        {
            _aggregateReportApiDao = aggregateReportApiDao;
            _providerDetailsProvider = providerDetailsProvider;
            _subdomainStatsMapper = subdomainStatsMapper;
            _providerStatsMapper = providerStatsMapper;
            _ipStatsMapper = ipStatsMapper;
            _spfDomainStatsMapper = spfDomainStatsMapper;
            _dkimDomainStatsMapper = dkimDomainStatsMapper;
        }

        public async Task<List<DomainStats>> GetDomainStatsDto(string domain, DateTime startDate, DateTime endDate, bool rollup = false, string categoryFilter = null)
        {
            List<DomainStats> domainStats =  await _aggregateReportApiDao.GetDomainStatsDense(domain, startDate, endDate, rollup, categoryFilter);
            return domainStats;
        }

        public async Task<ProviderStatsDtoResult> GetProviderStatsDto(string domain, DateTime startDate,
            DateTime endDate,
            int page, int pageSize, bool rollup = false, string categoryFilter = null, string providerFilter = null)
        {
            ProviderStatsResult providerStatsResult =
                await _aggregateReportApiDao.GetProviderStats(domain, startDate, endDate, page, pageSize, rollup, categoryFilter, providerFilter);

            List<ProviderStatsDto> list = new List<ProviderStatsDto>();

            foreach (ProviderStats providerStats in providerStatsResult.ProviderStats)
            {
                string providerAlias =
                    _providerDetailsProvider.GetProviderAliasOut(
                        _providerDetailsProvider.GetProviderAliasIn(providerStats.Provider));
                string providerMarkdown = _providerDetailsProvider.GetProviderMarkdown(providerStats.Provider);
                var ipDto = _providerStatsMapper.Map(providerStats, providerAlias, providerMarkdown);
                list.Add(ipDto);
            }

            ProviderStatsDtoResult providerStatsDtoResult = new ProviderStatsDtoResult(list,
                providerStatsResult.TotalCount, providerStatsResult.AllProviderTotalCount);
            return providerStatsDtoResult;
        }

        public async Task<SubdomainStatsDtoResult> GetSubdomainStatsDto(string domain, string provider, DateTime startDate, DateTime endDate, int page, int pageSize, string categoryFilter = null)
        {
            provider = _providerDetailsProvider.GetProviderAliasIn(provider);

            string providerAlias = _providerDetailsProvider.GetProviderAliasOut(provider);
            string providerMarkdown = _providerDetailsProvider.GetProviderMarkdown(provider);

            SubdomainStatsResult subdomainStatsResult =  await _aggregateReportApiDao.GetSubdomainStats(domain, provider, startDate, endDate, page, pageSize, categoryFilter);
                        
            List<SubdomainStatsDto> subdomainStatsDto = new List<SubdomainStatsDto>();
            
            foreach (SubdomainStats subdomain in subdomainStatsResult.SubdomainStats)
            {
                SubdomainStatsDto subdomainDto = _subdomainStatsMapper.Map(subdomain, providerAlias, providerMarkdown);
                subdomainStatsDto.Add(subdomainDto);
            }
            
            
            SubdomainStatsDtoResult subdomainStatsDtoResult = new SubdomainStatsDtoResult(
                _subdomainStatsMapper.Map(subdomainStatsResult.DomainStats, providerAlias, providerMarkdown), 
                subdomainStatsDto, subdomainStatsResult.TotalCount);
            
            return subdomainStatsDtoResult;
        }

        public async Task<IpStatsDtoResult> GetIpStatsDto(string domain, DateTime startDate, DateTime endDate, string provider, int page, int pageSize, string ip = null, string hostFilter = null, string categoryFilter = null)
        {
            provider = _providerDetailsProvider.GetProviderAliasIn(provider);
            IpStatsResult ipStats =  await _aggregateReportApiDao.GetIpStats(domain, startDate, endDate, provider, page, pageSize, ip, hostFilter, categoryFilter);

            string providerAlias = _providerDetailsProvider.GetProviderAliasOut(provider);
            string providerMarkdown = _providerDetailsProvider.GetProviderMarkdown(provider);
            

            List<IpStatsDto> ipStatsDto = new List<IpStatsDto>();

            foreach (IpStats ipStat in ipStats.IpStats)
            {
                IpStatsDto ipDto = _ipStatsMapper.Map(ipStat, providerAlias, providerMarkdown);
                ipStatsDto.Add(ipDto);
            }


            IpStatsDtoResult ipStatsDtoResult = new IpStatsDtoResult(ipStatsDto, ipStats.TotalHostnameCount, ipStats.TotalEmails);
            return ipStatsDtoResult;
        }
        
        public async Task<List<SpfDomainStatsDto>> GetSpfDomainStatsDto(string domain, string provider, string ip, DateTime startDate,
            DateTime endDate)
        {
            provider = _providerDetailsProvider.GetProviderAliasIn(provider);
            List<SpfDomainStats> spfDomainStats =  await _aggregateReportApiDao.GetSpfDomainStats(domain, provider, ip, startDate, endDate);
            

            string providerAlias = _providerDetailsProvider.GetProviderAliasOut(provider);
            string providerMarkdown = _providerDetailsProvider.GetProviderMarkdown(provider);
            
            
            List<SpfDomainStatsDto> spfDomainStatsDto = new List<SpfDomainStatsDto>();

            foreach (SpfDomainStats spfDomainStat in spfDomainStats)
            {
                SpfDomainStatsDto spfDomainDto = _spfDomainStatsMapper.Map(spfDomainStat, providerAlias, providerMarkdown);
                spfDomainStatsDto.Add(spfDomainDto);
            }
            
            return spfDomainStatsDto;
        }
        
        public async Task<List<DkimDomainStatsDto>> GetDkimDomainStatsDto(string domain, string provider, string ip, DateTime startDate,
            DateTime endDate)
        {
            provider = _providerDetailsProvider.GetProviderAliasIn(provider);
            List<DkimDomainStats> dkimDomainStats =  await _aggregateReportApiDao.GetDkimDomainStats(domain, provider, ip, startDate, endDate);

            string providerAlias = _providerDetailsProvider.GetProviderAliasOut(provider);
            string providerMarkdown = _providerDetailsProvider.GetProviderMarkdown(provider);
            
            
            List<DkimDomainStatsDto> dkimDomainStatsDto = new List<DkimDomainStatsDto>();

            foreach (DkimDomainStats dkimDomainStat in dkimDomainStats)
            {
                DkimDomainStatsDto dkimDomainDto = _dkimDomainStatsMapper.Map(dkimDomainStat, providerAlias, providerMarkdown);
                dkimDomainStatsDto.Add(dkimDomainDto);
            }
            
            return dkimDomainStatsDto;
        }

        public async Task<IEnumerable<AggregateReportExportStats>> GetAggregateReportExport(string domain, string provider, string ip,
            DateTime startDate, DateTime endDate, bool includeSubdomains)
        {
            IEnumerable<AggregateReportExportStats> aggregateReportExportStats =
                await _aggregateReportApiDao.GetAggregateReportExport(domain, provider, ip, startDate, endDate,
                    includeSubdomains);

            return aggregateReportExportStats;
        }
    }
}