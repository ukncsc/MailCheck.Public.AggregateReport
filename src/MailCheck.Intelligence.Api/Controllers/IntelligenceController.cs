using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Api.Authorisation.Filter;
using MailCheck.Common.Api.Authorisation.Service.Domain;
using MailCheck.Common.Api.Domain;
using MailCheck.Intelligence.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MailCheck.Intelligence.Api.Controllers
{
    [Route("api/intelligence")]
    [MailCheckAuthoriseRole(Role.Standard)]
    public class IntelligenceController : Controller
    {
        private readonly IIpAddressDetailsService _ipAddressDetailsService;

        public IntelligenceController(IIpAddressDetailsService ipAddressDetailsService)
        {
            _ipAddressDetailsService = ipAddressDetailsService;
        }
        
        [HttpGet("{ipAddress}")]
        public async Task<IActionResult> GetIpAddressDateRangeDetails(IpAddressDateRangeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ModelState.Values));
            }

            return Ok(await _ipAddressDetailsService.GetIpAddressDetails(request.IpAddress, request.StartDate, request.EndDate));
        }
    }
}
