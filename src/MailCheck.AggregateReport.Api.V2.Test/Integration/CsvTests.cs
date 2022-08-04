using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.AggregateReport.Api.V2.Dao;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.Common.Api.Authorisation.Service;
using MailCheck.Common.Api.Authorisation.Service.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Integration
{
    [TestFixture]
    public class CsvTests
    {
        private IMailCheckAuthorisationService _mailCheckAuthorisationService;
        private IAggregateReportApiDao _aggregateReportDao;
        private HttpClient _client;

        public CsvTests()
        {
            Environment.SetEnvironmentVariable("ConnectionString", "value");
            Environment.SetEnvironmentVariable("SnsTopicArn", "value");
            Environment.SetEnvironmentVariable("MicroserviceOutputSnsTopicArn", "value");
            Environment.SetEnvironmentVariable("AuthorisationServiceEndpoint", "value");
            Environment.SetEnvironmentVariable("DevMode", "value");
        }

        [SetUp]
        public void SetUp()
        {
            WebApplicationFactory<StartUp> factory = new WebApplicationFactory<StartUp>();

            _mailCheckAuthorisationService = A.Fake<IMailCheckAuthorisationService>();
            _aggregateReportDao = A.Fake<IAggregateReportApiDao>();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    
                    services.Replace(new ServiceDescriptor(typeof(IMailCheckAuthorisationService), _mailCheckAuthorisationService));
                    services.Replace(new ServiceDescriptor(typeof(IAggregateReportApiDao), _aggregateReportDao));
                });
            }).CreateClient();
        }

        [Test]
        public async Task DateTimeIsCorrectlyFormatted()
        {
            _client.DefaultRequestHeaders.Add("oidc_claim_email", "value");
            _client.DefaultRequestHeaders.Add("oidc_claim_given_name", "value");
            _client.DefaultRequestHeaders.Add("oidc_claim_family_name", "value");
            A.CallTo(() => _mailCheckAuthorisationService.IsAuthorised(A<Role>._))
                .Returns(new AuthorisationResult {Authorised = true});
            A.CallTo(() => _mailCheckAuthorisationService.IsAuthorised(A<Operation>._, A<ResourceType>._, A<string>._)).Returns(new AuthorisationResult { Authorised = true });

            AggregateReportExportStats aggregateReportExportStats = new AggregateReportExportStats("2001-01-29", "", "", "", "", "", 234, "", "", "", "", "", "",
                "", "", 0, 0, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, "", "", "", 0, "", "", 0, 0, 0, 0, 0, 0, 0, 0);

            A.CallTo(() => _aggregateReportDao.GetAggregateReportExport(A<string>._, A<string>._, A<string>._, A<DateTime>._, A<DateTime>._, A<bool>._))
                .Returns(new List<AggregateReportExportStats> { aggregateReportExportStats });

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/aggregatev2/export/gchq.gov.uk/2020-06-24/2020-07-21?includeSubdomains=true&csvDownload=true")
            {
                Headers = { { "accept", "text/csv" } }
            };

            HttpResponseMessage response = await _client.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            string date = content.Split(Environment.NewLine)[1].Split(",")[0];

            Assert.AreEqual("2001-01-29", date);
        }
    }
}
