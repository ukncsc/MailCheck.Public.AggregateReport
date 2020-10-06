using System.Threading.Tasks;
using MailCheck.AggregateReport.Api.V2.Dao;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.Common.Api.Authorisation.Filter;
using MailCheck.Common.Api.Authorisation.Service.Domain;
using MailCheck.Common.Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MailCheck.AggregateReport.Api.V2.Controllers
{
    [Route("api/aggregateV2/summary/")]
    [MailCheckAuthoriseResource(Operation.Read, ResourceType.AggregateReport, "domain")]
    public class AggregateReportSummaryControllerV2 : Controller
    {
        private readonly IAggregateReportApiDao _aggregateReportApiDao;

        public AggregateReportSummaryControllerV2(IAggregateReportApiDao aggregateReportApiDao)
        {
            _aggregateReportApiDao = aggregateReportApiDao;
        }

        [HttpGet("{domain}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetDomainDate(DomainDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }
            
            return Ok(await _aggregateReportApiDao.GetDomainStatsSummary(
                request.Domain, 
                request.StartDate, 
                request.EndDate, 
                false, 
                request.CategoryFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/rollup")]
        public async Task<IActionResult> GetDomainDateRollup(DomainDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportApiDao.GetDomainStatsSummary(
                request.Domain, 
                request.StartDate, 
                request.EndDate,
                true, 
                request.CategoryFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/providers")]
        public async Task<IActionResult> GetProviderStats(DomainDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportApiDao.GetProviderStatsSummary(request.Domain, request.StartDate, request.EndDate,
                request.Page.Value, request.PageSize.Value, false, request.CategoryFilter, request.ProviderFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/providers/rollup")]
        public async Task<IActionResult> GetProviderStatsRollup(DomainDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportApiDao.GetProviderStatsSummary(request.Domain, request.StartDate, request.EndDate,
                request.Page.Value, request.PageSize.Value, true, request.CategoryFilter, request.ProviderFilter));
        }

        [HttpGet("{domain}/{startDate}/{endDate}/providers/{provider}/ips")]
        public async Task<IActionResult> GetProviderIpStats(DomainProviderDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _aggregateReportApiDao.GetIpStatsSummary(request.Domain, request.StartDate, request.EndDate, 
                request.Provider, request.CategoryFilter, request.HostFilter));
        }
    }
}