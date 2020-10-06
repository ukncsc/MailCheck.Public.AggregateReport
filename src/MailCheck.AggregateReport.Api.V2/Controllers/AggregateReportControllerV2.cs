using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Services;
using MailCheck.Common.Api.Authorisation.Filter;
using MailCheck.Common.Api.Authorisation.Service.Domain;
using MailCheck.Common.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace MailCheck.AggregateReport.Api.V2.Controllers
{
    [Route("api/aggregateV2")]
    [MailCheckAuthoriseResource(Operation.Read, ResourceType.AggregateReport, "domain")]
    public class AggregateReportControllerV2 : Controller
    {
        private readonly IAggregateReportService _aggregateReportService;

        public AggregateReportControllerV2(IAggregateReportService aggregateReportService)
        {
            _aggregateReportService = aggregateReportService;
        }

        [HttpGet("{domain}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetDomainDate(DomainDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportService.GetDomainStatsDto(request.Domain, request.StartDate, request.EndDate, false, request.CategoryFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/rollup")]
        public async Task<IActionResult> GetDomainDateRollup(DomainDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportService.GetDomainStatsDto(request.Domain, request.StartDate, request.EndDate,
                true, request.CategoryFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/providers")]
        public async Task<IActionResult> GetProviderStats(DomainDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportService.GetProviderStatsDto(request.Domain, request.StartDate, request.EndDate,
                request.Page.Value, request.PageSize.Value, false, request.CategoryFilter, request.ProviderFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/providers/rollup")]
        public async Task<IActionResult> GetProviderRollupStats(DomainDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportService.GetProviderStatsDto(request.Domain, request.StartDate, request.EndDate,
                request.Page.Value, request.PageSize.Value, true, request.CategoryFilter, request.ProviderFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/providers/{provider}/subdomains")]
        public async Task<IActionResult> GetSubdomainStats(DomainProviderDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportService.GetSubdomainStatsDto(request.Domain, request.Provider,
                request.StartDate, request.EndDate, request.Page.Value, request.PageSize.Value, request.CategoryFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/providers/{provider}/ips")]
        public async Task<IActionResult> GetProviderIpStats(DomainProviderDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportService.GetIpStatsDto(request.Domain, request.StartDate, request.EndDate, request.Provider, request.Page.Value, request.PageSize.Value, request.IpFilter, request.HostFilter, request.CategoryFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/providers/{provider}/ips/{ip}")]
        public async Task<IActionResult> GetProviderIpStats(DomainProviderIpDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportService.GetIpStatsDto(request.Domain, request.StartDate, request.EndDate, request.Provider, request.Page.Value, request.PageSize.Value, request.Ip));
        }
        
        [HttpGet("{domain}/{startDate}/{endDate}/providers/{provider}/ips/{ip}/spfdomains")]
        public async Task<IActionResult> GetSpfDomainStats(DomainProviderIpDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportService.GetSpfDomainStatsDto(request.Domain, request.Provider, request.Ip, request.StartDate, request.EndDate));
        }
        
        [HttpGet("{domain}/{startDate}/{endDate}/providers/{provider}/ips/{ip}/dkimdomains")]
        public async Task<IActionResult> GetDkimDomainStats(DomainProviderIpDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportService.GetDkimDomainStatsDto(request.Domain, request.Provider, request.Ip, request.StartDate, request.EndDate));
        }
        
        [HttpGet("export/{domain}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetAggregateReportExport(DataExportRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }
            
            IEnumerable<AggregateReportExportStats> aggregateReportExportStats = await _aggregateReportService.GetAggregateReportExport(request.Domain, 
                request.Provider, request.Ip, request.StartDate, request.EndDate, request.IncludeSubdomains);

            Response.Headers[HeaderNames.ContentDisposition] = request.CsvDownload
                ? $"attachment; filename=\"AggregateSummaryReportExport_{request.Domain}_{request.StartDate:yyyy-MM-dd}_{request.EndDate:yyyy-MM-dd}.csv\""
                : request.JsonDownload
                    ? $"attachment; filename=\"AggregateSummaryReportExport_{request.Domain}_{request.StartDate:yyyy-MM-dd}_{request.EndDate:yyyy-MM-dd}.json\""
                    : request.JsonRender
                        ? "inline"
                        : "";

            return Ok(aggregateReportExportStats);
        }
    }
}
