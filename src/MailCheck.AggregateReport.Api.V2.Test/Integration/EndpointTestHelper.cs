using System.Collections.Generic;
using System.Linq;
using MailCheck.Common.TestSupport;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MailCheck.AggregateReport.Api.V2.Test.Integration
{
    /// <remarks>This class exists to apply a helpful interface over BaseIntegrationTest</remarks>
    class EndpointTestHelper<TEntryPoint, TController> : BaseIntegrationTest<TEntryPoint, TController>, IEndpointTestHelper<TEntryPoint>
        where TEntryPoint : class
    {
        public IList<EndpointDescription> EndpointDescriptions => Endpoints
            .Select(e => new EndpointDescription { UrlTemplate = e.Item2, Method = e.Item1 })
            .ToList();

        WebApplicationFactory<TEntryPoint> IEndpointTestHelper<TEntryPoint>.Factory => Factory;

        public void SetUp()
        {
            BaseSetUp();
        }

        public void TearDown()
        {
            Factory?.Dispose();
        }
    }
}
