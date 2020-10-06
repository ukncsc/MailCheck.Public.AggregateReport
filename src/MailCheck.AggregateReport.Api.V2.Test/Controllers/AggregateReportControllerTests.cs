using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.AggregateReport.Api.V2.Controllers;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Controllers
{
    [TestFixture]
    public class AggregateReportControllerTests
    {
        private AggregateReportControllerV2 _domainStatusController;
        private IAggregateReportService _aggregateReportService;

        [SetUp]
        public void SetUp()
        {
            _aggregateReportService = A.Fake<IAggregateReportService>();
            _domainStatusController = new AggregateReportControllerV2(_aggregateReportService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = new ClaimsPrincipal(new ClaimsIdentity())}
                }
            };
        }

        // GetDomainDate
        [Test]
        public async Task GetDomainDateReturnsBadRequestWhenRequestIsInvalid()
        {
            DomainDateRangeRequest request = new DomainDateRangeRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetDomainDate(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        //GetDomainDateRollup
        [Test]
        public async Task GetDomainDateRollupReturnsBadRequestWhenRequestIsInvalid()
        {
            DomainDateRangeRequest request = new DomainDateRangeRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetDomainDateRollup(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        //GetProviderStats
        [Test]
        public async Task GetProviderStatsReturnsBadRequestWhenRequestIsInvalid()
        {
            DomainDateRangeRequest request = new DomainDateRangeRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetProviderStats(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        //GetProviderRollupStats
        [Test]
        public async Task GetProviderRollupStatsReturnsBadRequestWhenRequestIsInvalid()
        {
            DomainDateRangeRequest request = new DomainDateRangeRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetProviderRollupStats(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        //GetSubdomainStats
        [Test]
        public async Task GetSubdomainStatsReturnsBadRequestWhenRequestIsInvalid()
        {
            DomainProviderDateRangeRequest request = new DomainProviderDateRangeRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetSubdomainStats(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        // GetProviderIpStats
        [Test]
        public async Task GetProviderIpStatsReturnsBadRequestWhenRequestIsInvalid()
        {
            DomainProviderIpDateRangeRequest request = new DomainProviderIpDateRangeRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetProviderIpStats(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        // GetProviderIpStats2
        [Test]
        public async Task GetProviderIpStats2ReturnsBadRequestWhenRequestIsInvalid()
        {
            DomainProviderIpDateRangeRequest request = new DomainProviderIpDateRangeRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetProviderIpStats(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        //GetSpfDomainStats
        [Test]
        public async Task GetSpfDomainStatsReturnsBadRequestWhenRequestIsInvalid()
        {
            DomainProviderIpDateRangeRequest request = new DomainProviderIpDateRangeRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetSpfDomainStats(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        //getDkimDomainStats
        [Test]
        public async Task GetDkimDomainStatsReturnsBadRequestWhenRequestIsInvalid()
        {
            DomainProviderIpDateRangeRequest request = new DomainProviderIpDateRangeRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetDkimDomainStats(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        
        //getAggregateExportStats
        [Test]
        public async Task GetExportStatsReturnsBadRequestWhenRequestIsInvalid()
        {
            DataExportRequest request = new DataExportRequest();
            _domainStatusController.ModelState.AddModelError("testKey", "testErrorMessage");

            IActionResult result = await _domainStatusController.GetAggregateReportExport(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetAggregateReportExportReturnsStatsFromService()
        {
            DataExportRequest request = new DataExportRequest
            {
                CsvDownload = true, IncludeSubdomains = true, StartDate = new DateTime(2020, 06, 24),
                EndDate = new DateTime(2020, 07, 21)
            };

            AggregateReportExportStats[] reportExportStats =
            {
                new AggregateReportExportStats("2001-01-29T10:30:50", "", "", "", "", "",
                    0, "", "", "", "", "", "", "", "", 0, 0, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, "", "", "", 0, "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            };

            A.CallTo(() => _aggregateReportService.GetAggregateReportExport(A<string>._, A<string>._, A<string>._,
                A<DateTime>._, A<DateTime>._, A<bool>._)).Returns( reportExportStats );

            IActionResult result = await _domainStatusController.GetAggregateReportExport(request);
            
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.AreSame(((OkObjectResult)result).Value, reportExportStats);
        }
    }
}