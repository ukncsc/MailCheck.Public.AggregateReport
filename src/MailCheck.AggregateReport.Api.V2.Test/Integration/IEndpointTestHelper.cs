using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MailCheck.AggregateReport.Api.V2.Test.Integration
{
    public interface IEndpointTestHelper<TEntryPoint>
        where TEntryPoint : class
    {
        IList<EndpointDescription> EndpointDescriptions { get; }

        WebApplicationFactory<TEntryPoint> Factory { get; }

        void SetUp();

        void TearDown();
    }
}
