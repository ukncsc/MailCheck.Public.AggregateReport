using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Common.Api.Authorisation.Service;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Integration
{
    [TestFixture(Category = "Auth")]
    [TestFixtureSource(typeof(EndpointTestFixtureSource<StartUp>), "FixtureParams")]
    public class EndpointAuthTests
    {
        private readonly IEndpointTestHelper<StartUp> _endpointHelper;
        private IMailCheckAuthorisationService _mailCheckAuthorisationService;
        private HttpClient _client;
        
        public EndpointAuthTests(IEndpointTestHelper<StartUp> endpointHelper)
        {
            _endpointHelper = endpointHelper;
            Environment.SetEnvironmentVariable("ConnectionString", "value");
            Environment.SetEnvironmentVariable("SnsTopicArn", "value");
            Environment.SetEnvironmentVariable("MicroserviceOutputSnsTopicArn", "value");
            Environment.SetEnvironmentVariable("AuthorisationServiceEndpoint", "value");
            Environment.SetEnvironmentVariable("DevMode", "value");
        }

        [SetUp]
        public void SetUp()
        {
            _endpointHelper.SetUp();

            _mailCheckAuthorisationService = A.Fake<IMailCheckAuthorisationService>();

            _client = _endpointHelper.Factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.Replace(new ServiceDescriptor(typeof(IMailCheckAuthorisationService), _mailCheckAuthorisationService));
                });
            }).CreateClient();
        }

        [TearDown]
        public void TearDown() 
        {
            _endpointHelper.TearDown();
        }

        [Test]
        public async Task AuthenticationRequiredForAllEndpoints()
        {
            Assert.NotZero(_endpointHelper.EndpointDescriptions.Count, "Endpoints are discovered");

            foreach (var endpoint in _endpointHelper.EndpointDescriptions)
            {
                HttpMethod httpMethod = new HttpMethod(endpoint.Method);
                string url = endpoint.UrlTemplate;
                HttpRequestMessage request = new HttpRequestMessage(httpMethod, url);

                HttpResponseMessage response = await _client.SendAsync(request);

                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                Assert.AreEqual(0, Fake.GetCalls(_mailCheckAuthorisationService).Count(), "The request should not try to authorise");
            }
        }

        [Test]
        public async Task AuthorisationRequiredForAllEndpoints()
        {
            _client.DefaultRequestHeaders.Add("oidc_claim_email", "value");
            _client.DefaultRequestHeaders.Add("oidc_claim_given_name", "value");
            _client.DefaultRequestHeaders.Add("oidc_claim_family_name", "value");

            Assert.NotZero(_endpointHelper.EndpointDescriptions.Count, "Endpoints are discovered");

            foreach (var endpoint in _endpointHelper.EndpointDescriptions)
            {
                HttpMethod httpMethod = new HttpMethod(endpoint.Method);
                string url = endpoint.UrlTemplate;
                HttpRequestMessage request = new HttpRequestMessage(httpMethod, url);

                HttpResponseMessage response = await _client.SendAsync(request);

                Assert.AreEqual(1, Fake.GetCalls(_mailCheckAuthorisationService).Count(), "The request should authorise once");
                Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, "The request should be forbidden");
                Fake.ClearRecordedCalls(_mailCheckAuthorisationService);
            }
        }
    }
}
