using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Amazon.Auth.AccessControlPolicy;
using FakeItEasy;
using MailCheck.AggregateReport.Api.V2.Dao;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;
using MailCheck.AggregateReport.Api.V2.Mappers;
using MailCheck.AggregateReport.Api.V2.Provider;
using MailCheck.AggregateReport.Api.V2.Services;
using NUnit.Framework;
using Serilog.Parsing;
using MailCheck.AggregateReport.Contracts;
using MailCheck.Common.Util;
using Alignment = MailCheck.AggregateReport.Contracts.Alignment;
using Policy = MailCheck.AggregateReport.Contracts.Policy;
using System.Linq;

namespace MailCheck.AggregateReport.Api.V2.Test.Services
{
    [TestFixture]
    public class AggregateReportServiceTests
    {
        private IAggregateReportApiDao _aggregateReportApiDao;
        private IAggregateReportService _aggregateReportService;
        private IProviderDetailsProvider _providerDetailsProvider;
        private ISubdomainStatsMapper _subdomainStatsMapper;
        private IProviderStatsMapper _providerStatsMapper;
        private IIpStatsMapper _ipStatsMapper;
        private ISpfDomainStatsMapper _spfDomainStatsMapper;
        private IDkimDomainStatsMapper _dkimDomainStatsMapper;

        [SetUp]
        public void SetUp()
        {
            _aggregateReportApiDao = A.Fake<IAggregateReportApiDao>();
            _providerDetailsProvider = A.Fake<IProviderDetailsProvider>();
            _subdomainStatsMapper = A.Fake<ISubdomainStatsMapper>();
            _spfDomainStatsMapper = A.Fake<ISpfDomainStatsMapper>();
            _ipStatsMapper = A.Fake<IIpStatsMapper>();
            _dkimDomainStatsMapper = A.Fake<IDkimDomainStatsMapper>();
            _providerStatsMapper = A.Fake<IProviderStatsMapper>();
            _aggregateReportService = new AggregateReportService(_aggregateReportApiDao, _providerDetailsProvider, _subdomainStatsMapper, _providerStatsMapper, _ipStatsMapper, _spfDomainStatsMapper, _dkimDomainStatsMapper);
        }

        [Test]
        public async Task CheckGetDomainStatsDtoReturnsDomainStats()
        {
            string domain = "ncsc.gov.uk";
            DateTime startDate = new DateTime(2019, 8, 9);
            DateTime endDate = new DateTime(2019, 8, 15);
            DateTime now = new DateTime(2019, 8, 12);
            
            DomainStats domainStat = new DomainStats(domain, now, 0, 1, 2, 
                3, 4, 5, 6, 7,
                8, 9, 10, 11);
            
            A.CallTo(() => _aggregateReportApiDao.GetDomainStats(domain, A<DateTime>._, A<DateTime>._, false, null))
                .Returns(Task.FromResult(new List<DomainStats>() {domainStat}));
            
            List<DomainStats> domainStats = await _aggregateReportService.GetDomainStatsDto(domain, startDate, endDate);
            
            Assert.That(domainStats.Count, Is.EqualTo(7));
            Assert.That(domainStats[3], Is.SameAs(domainStat));
        }

        [Test]
        public async Task CheckGetProviderStatsDtoReturnsProviderStats()
        {
            string domain = "ncsc.gov.uk";
            string provider = "outlook.com";
            string alias = "Microsoft Outlook";
            string markdown = "testMarkdown";

            ProviderStats providerStat = new ProviderStats(
                domain, provider, 0, 1, 2,
                3, 4, 5, 6, 7,
                8, 9, 10, 11, 12, 13, 14, 15, 16, 20, 21, 22);
            ProviderStatsResult providerStatsResult = new ProviderStatsResult(new List<ProviderStats> { providerStat }, 20, 100);

            A.CallTo(() => _aggregateReportApiDao.GetProviderStats(domain, A<DateTime>._,
                    A<DateTime>._, A<int>._, A<int>._, false, null, null))
                .Returns(Task.FromResult(providerStatsResult));

            ProviderStatsDto providerStatDto = new ProviderStatsDto(domain, provider, 0, 1, 2,
                3, 4, 5, 6, 7,
                8, 9, 10, 11, 12, 13, 14, 15, 16, 20, 21, 22, alias, markdown);

            A.CallTo(() => _providerStatsMapper.Map(A<ProviderStats>._, A<string>._, A<string>._))
                .Returns(providerStatDto);

            ProviderStatsDtoResult results = await _aggregateReportService.GetProviderStatsDto(domain, DateTime.MinValue, DateTime.MaxValue, 0, 0);

            Assert.AreEqual(provider, results.ProviderStats[0].Provider);
            Assert.AreEqual(alias, results.ProviderStats[0].ProviderAlias);
            Assert.AreEqual(markdown, results.ProviderStats[0].ProviderMarkdown);
        }
        
        [Test]
        public async Task CheckGetSubdomainStatsDtoReturnsSubdomainStatsDto()
        {
            string domain = "ncsc.gov.uk";
            string subdomain = "digital.ncsc.gov.uk";
            int totalCount = 20;
            
            string provider = "testProvider";
            string alias = "testAlias";
            string markdown = "testMarkdown";
            
            A.CallTo(() => _providerDetailsProvider.GetProviderAliasIn(alias))
                .Returns(provider);
            A.CallTo(() => _providerDetailsProvider.GetProviderAliasOut(provider))
                .Returns(alias);
            A.CallTo(() => _providerDetailsProvider.GetProviderMarkdown(provider))
                .Returns(markdown);
            
            SubdomainStats domainStats = new SubdomainStats(domain, provider, subdomain, 0, 1, 2, 
                3, 4, 5, 6, 7,
                8, 9, 10, 11, 20, 21, 22);
            SubdomainStatsResult subdomainStatsResult = new SubdomainStatsResult(domainStats, new List<SubdomainStats>() {domainStats}, totalCount);
            
            A.CallTo(() => _aggregateReportApiDao.GetSubdomainStats(domain, provider, A<DateTime>._,
                    A<DateTime>._, A<int>._, A<int>._, null))
                .Returns(Task.FromResult(subdomainStatsResult));
            
            SubdomainStatsDto subdomainStatsDto = new SubdomainStatsDto(domain, provider, subdomain, 0, 1, 2,
                3, 4, 5, 6, 7,
                8, 9, 10, 11, 20, 21, 22,
                alias, markdown);
            
            A.CallTo(() => _subdomainStatsMapper.Map(domainStats, alias, markdown))
                .Returns(subdomainStatsDto);
            
            SubdomainStatsDtoResult subdomainStatsDtoResults = await _aggregateReportService.GetSubdomainStatsDto(domain, alias, DateTime.MinValue, DateTime.MaxValue, 0, 0);

            Assert.AreSame(subdomainStatsDto, subdomainStatsDtoResults.DomainStatsDto);
            Assert.AreSame(subdomainStatsDto, subdomainStatsDtoResults.SubdomainStatsDto[0]);
            Assert.AreEqual(totalCount, subdomainStatsDtoResults.TotalCount);
        }

        [Test]
        public async Task CheckGetIpStatsDtoReturnsIpStatsDto()
        {
            string domain = "ncsc.gov.uk";
            string ip = "192.168.72.11";
            string hostname = "hostName";
            int totalHostnameCount = 100;
            int totalEmails = 1111;
            
            string provider = "testProvider";
            string alias = "testAlias";
            string markdown = "testMarkdown";
            
            A.CallTo(() => _providerDetailsProvider.GetProviderAliasIn(alias))
                .Returns(provider);
            A.CallTo(() => _providerDetailsProvider.GetProviderAliasOut(provider))
                .Returns(alias);
            A.CallTo(() => _providerDetailsProvider.GetProviderMarkdown(provider))
                .Returns(markdown);
            
            IpStats ipStat = new IpStats(domain, provider, ip, hostname, 0, 1, 2, 
                3, 4, 5, 6, 7,
                8, 9, 10, 11, 30, 31, 32, 33, 34, 35, 100, 36, 37, 38, 39, 12, 
                13, 14, 15, 16, 17, 
                18, 19, 20, 21, 22, 23,
                24, 25, 26, 27, 28);

            IpStatsResult ipStatsResult = new IpStatsResult(new List<IpStats>() {ipStat}, totalHostnameCount, totalEmails);
            
            A.CallTo(() => _aggregateReportApiDao.GetIpStats(domain, A<DateTime>._, A<DateTime>._, provider, A<int>._, A<int>._, null, null, null))
                .Returns(Task.FromResult(ipStatsResult));
            
            IpStatsDto ipStatsDto = new IpStatsDto(domain, provider, ip, hostname, 0, 1, 2, 
                3, 4, 5, 6, 7,
                8, 9, 10, 11, 30, 31, 32, 33, 34, 35, 100, 36, 37, 38, 39, 12, 
                13, 14, 15, 16, 17, 
                18, 19, 20, 21, 22, 23,
                24, 25, 26, 27, 28, alias, markdown);

            A.CallTo(() => _ipStatsMapper.Map(ipStat, alias, markdown))
                .Returns(ipStatsDto);
            
            IpStatsDtoResult ipStatsDtoResult = await _aggregateReportService.GetIpStatsDto(domain, DateTime.MinValue, DateTime.MaxValue, alias, 0, 0);
            
            Assert.AreSame(ipStatsDto, ipStatsDtoResult.IpStats[0]);
            Assert.AreEqual(totalHostnameCount, ipStatsDtoResult.TotalHostnameCount);
            Assert.AreEqual(totalEmails, ipStatsDtoResult.TotalEmails);
        }

        [Test]
        public async Task CheckGetSpfDomainStatsDtoReturnsSpfDomainStatsDto()
        {
            string domain = "ncsc.gov.uk";
            string ip = "192.168.72.11";
            string spfDomain = "ncsc.gov.uk";

            string provider = "testProvider";
            string alias = "testAlias";
            string markdown = "testMarkdown";

            A.CallTo(() => _providerDetailsProvider.GetProviderAliasIn(alias))
                .Returns(provider);
            A.CallTo(() => _providerDetailsProvider.GetProviderAliasOut(provider))
                .Returns(alias);
            A.CallTo(() => _providerDetailsProvider.GetProviderMarkdown(provider))
                .Returns(markdown);

            SpfDomainStats spfDomainStat = new SpfDomainStats(domain, provider, ip, spfDomain, 5, 10);

            List<SpfDomainStats> spfDomainStats = new List<SpfDomainStats>() {spfDomainStat};

            A.CallTo(() => _aggregateReportApiDao.GetSpfDomainStats(domain, provider, ip,
                    A<DateTime>._, A<DateTime>._))
                .Returns(Task.FromResult(spfDomainStats));

            SpfDomainStatsDto spfDomainStatsDto =
                new SpfDomainStatsDto(domain, provider, ip, spfDomain, 5, 10, alias, markdown);

            A.CallTo(() => _spfDomainStatsMapper.Map(spfDomainStat, alias, markdown))
                .Returns(spfDomainStatsDto);

            List<SpfDomainStatsDto> spfDomainStatsDtoResult =
                await _aggregateReportService.GetSpfDomainStatsDto(domain, alias, ip, DateTime.MinValue,
                    DateTime.MaxValue);
            
            Assert.AreSame(spfDomainStatsDto, spfDomainStatsDtoResult[0]);
        }

        [Test]
        public async Task CheckGetDkimDomainStatsDtoReturnsDkimDomainStatsDto()
        {
            string domain = "ncsc.gov.uk";
            string ip = "192.168.72.11";
            string dkimDomain = "ncsc.gov.uk";
            string dkimSelector = "selector";
            
            string provider = "testProvider";
            string alias = "testAlias";
            string markdown = "testMarkdown";

            A.CallTo(() => _providerDetailsProvider.GetProviderAliasIn(alias))
                .Returns(provider);
            A.CallTo(() => _providerDetailsProvider.GetProviderAliasOut(provider))
                .Returns(alias);
            A.CallTo(() => _providerDetailsProvider.GetProviderMarkdown(provider))
                .Returns(markdown);

            DkimDomainStats dkimDomainStat = new DkimDomainStats(domain, provider, ip, dkimDomain, dkimSelector, 5, 10);

            List<DkimDomainStats> dkimDomainStats = new List<DkimDomainStats>() {dkimDomainStat};
            
            A.CallTo(() => _aggregateReportApiDao.GetDkimDomainStats(domain, provider, ip, A<DateTime>._,
                    A<DateTime>._))
                .Returns(Task.FromResult(dkimDomainStats));
            
            DkimDomainStatsDto dkimDomainStatsDto = new DkimDomainStatsDto(domain, provider, ip, dkimDomain, dkimSelector, 5, 10, alias, markdown);

            A.CallTo(() => _dkimDomainStatsMapper.Map(dkimDomainStat, alias, markdown))
                .Returns(dkimDomainStatsDto);
            
            List<DkimDomainStatsDto> dkimDomainStatsDtoResult = await _aggregateReportService.GetDkimDomainStatsDto(domain, alias, ip, DateTime.MinValue, DateTime.MaxValue);
                        
            Assert.AreSame(dkimDomainStatsDto, dkimDomainStatsDtoResult[0]);
        }
        
        [Test]
        public async Task CheckGetAggregateExportStatsReturnsExportStats()
        {
            string effectiveDateIso8601 = "2001-01-29T10:30:50";
            string domain = "ncsc.gov.uk";
            string provider = "testProvider";
            string ip = "192.168.72.11";

            AggregateReportExportStats aggregateReportExportStat = new AggregateReportExportStats(
                effectiveDateIso8601, domain, provider, "", "reporterOrgName", ip, 100, 
                "disposition", "dkim", "spf", "envelopeTo", "envelopeFrom", 
                "headerFrom", "organisationDomainFrom", "spfAuthResults", 25, 75,
                "dkimAuthResults", 75, 25, 
                10, 5, 10, 5, 1, 0, 10, 
                "hostName", "hostOrgDomain", "hostProvider", 659562,
                "hostAsDescription", "hostCountry",
                10, 5, 1, 0, 
                5, 10, 5, 1);

            List<AggregateReportExportStats> aggregateReportExportStats = new List<AggregateReportExportStats>() { aggregateReportExportStat };
            
            A.CallTo(() => _aggregateReportApiDao.GetAggregateReportExport(domain, provider, ip, A<DateTime>._,
                    A<DateTime>._, A<bool>._))
                .Returns(Task.FromResult(aggregateReportExportStats));

            var result = await _aggregateReportService.GetAggregateReportExport(domain, provider, ip, new DateTime(), new DateTime(), true);

            Assert.AreSame(aggregateReportExportStat, result.ToList()[0]);
        }
    }
}