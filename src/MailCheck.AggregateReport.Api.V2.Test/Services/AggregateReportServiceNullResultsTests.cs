using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.AggregateReport.Api.V2.Controllers;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;
using MailCheck.AggregateReport.Api.V2.Services;
using MailCheck.Common.Api.Authorisation.Service;
using MailCheck.Common.Api.Authorisation.Service.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Services
{
    [TestFixture]
    public class AggregateReportServiceNullResultsTests
    {
        private IAggregateReportService _aggregateReportService;

        [SetUp]
        public void SetUp()
        {
            _aggregateReportService = A.Fake<IAggregateReportService>();
        }
        
        [Test]
        public async Task GetDomainStatsDtoReturnsNullWhenNoResults()
        {
            DomainDateRangeRequest request = new DomainDateRangeRequest();

            List<DomainStats> result = await _aggregateReportService.GetDomainStatsDto(request.Domain, request.StartDate, request.EndDate);

            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public async Task GetProviderStatsDtoReturnsNullWhenNoResults()
        {
            DomainDateRangeRequest request = new DomainDateRangeRequest();

            ProviderStatsDtoResult result = await _aggregateReportService.GetProviderStatsDto(request.Domain, request.StartDate, request.EndDate, 
                request.Page.Value, request.PageSize.Value);

            Assert.That(result.ProviderStats, Is.Empty);
            Assert.That(result.TotalCount, Is.EqualTo(0));
            Assert.That(result.AllProviderTotalCount, Is.EqualTo(0));
        }
        
        [Test]
        public async Task GetSubdomainStatsDtoReturnsNullWhenNoResults()
        {
            DomainProviderDateRangeRequest request = new DomainProviderDateRangeRequest();

           SubdomainStatsDtoResult result = await _aggregateReportService.GetSubdomainStatsDto(request.Domain, request.Provider, request.StartDate, 
               request.EndDate, request.Page.Value, request.PageSize.Value);

            Assert.That(result.SubdomainStatsDto, Is.Empty);
            Assert.That(result.TotalCount, Is.EqualTo(0));
            
            Assert.That(result.DomainStatsDto.Domain, Is.Empty);
            Assert.That(result.DomainStatsDto.Provider, Is.Empty);
            Assert.That(result.DomainStatsDto.Subdomain, Is.Empty);
            Assert.That(result.DomainStatsDto.ProviderAlias, Is.Empty);
            Assert.That(result.DomainStatsDto.ProviderMarkdown, Is.Empty);
            
            Assert.That(result.DomainStatsDto.TotalEmails, Is.EqualTo(0));
            Assert.That(result.DomainStatsDto.SpfFailDkimFailNone, Is.EqualTo(0));
            Assert.That(result.DomainStatsDto.SpfFailDkimFailQuarantine, Is.EqualTo(0));
            Assert.That(result.DomainStatsDto.SpfFailDkimFailReject, Is.EqualTo(0));
            
            Assert.That(result.DomainStatsDto.SpfFailDkimPassNone, Is.EqualTo(0));
            Assert.That(result.DomainStatsDto.SpfFailDkimPassQuarantine, Is.EqualTo(0));
            Assert.That(result.DomainStatsDto.SpfFailDkimPassReject, Is.EqualTo(0));
            
            Assert.That(result.DomainStatsDto.SpfPassDkimFailNone, Is.EqualTo(0));
            Assert.That(result.DomainStatsDto.SpfPassDkimFailQuarantine, Is.EqualTo(0));
            Assert.That(result.DomainStatsDto.SpfPassDkimFailReject, Is.EqualTo(0));
            
            Assert.That(result.DomainStatsDto.SpfPassDkimPassNone, Is.EqualTo(0));
            Assert.That(result.DomainStatsDto.SpfPassDkimPassQuarantine, Is.EqualTo(0));
            Assert.That(result.DomainStatsDto.SpfPassDkimPassReject, Is.EqualTo(0));
        }
        
        [Test]
        public async Task GetIpStatsDtoReturnsNullWhenNoResults()
        {
            DomainProviderIpDateRangeRequest request = new DomainProviderIpDateRangeRequest();

            IpStatsDtoResult result = await _aggregateReportService.GetIpStatsDto(request.Domain, 
                request.StartDate, request.EndDate, request.Provider, request.Page.Value, request.PageSize.Value);

            Assert.That(result.IpStats, Is.Empty);
            Assert.That(result.TotalHostnameCount, Is.EqualTo(0));
        }
        
        [Test]
        public async Task GetSpfDomainStatsDtoReturnsNullWhenNoResults()
        {
            DomainProviderIpDateRangeRequest request = new DomainProviderIpDateRangeRequest();

            List<SpfDomainStatsDto> result = await _aggregateReportService.GetSpfDomainStatsDto(request.Domain, request.Provider, request.Ip, 
                request.StartDate, request.EndDate);

            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public async Task GetDkimDomainStatsDtoStatsReturnsNullWhenNoResults()
        {
            DomainProviderIpDateRangeRequest request = new DomainProviderIpDateRangeRequest();

            List<DkimDomainStatsDto> result = await _aggregateReportService.GetDkimDomainStatsDto(request.Domain, request.Provider, request.Ip, 
                request.StartDate, request.EndDate);

            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public async Task GetExportStatsDtoStatsReturnsNullWhenNoResults()
        {
            DataExportRequest request = new DataExportRequest();

            IEnumerable<AggregateReportExportStats> result = await _aggregateReportService.GetAggregateReportExport(request.Domain, request.Provider, request.Ip, 
                request.StartDate, request.EndDate, request.IncludeSubdomains);

            Assert.That(result, Is.Empty);
        }
    }
}