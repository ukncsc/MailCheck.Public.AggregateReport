using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Integration
{
    class EndpointTestFixtureSource<TEntryPoint>
    {
        static readonly Type EntryPointType = typeof(TEntryPoint);

        /// <summary>
        /// Gets a TestFixtureData containing an instance of EndpointTestHelper
        /// for each controller class found in <typeparamref name="TEntryPoint"/>
        /// </summary>
        public static IEnumerable<TestFixtureData> FixtureParams
        {
            get
            {
                return EntryPointType.Assembly
                    .GetTypes()
                    .Where(t => typeof(Controller).IsAssignableFrom(t))
                    .Select(c =>
                    {
                        // Here we're creating a closed generic type for the entrypoint type and controller type...
                        var testerType = typeof(EndpointTestHelper<,>).MakeGenericType(EntryPointType, c);
                        // ... and then instantiating that new type
                        var tester = Activator.CreateInstance(testerType);
                        var tfd = new TestFixtureData(tester);
                        tfd.SetArgDisplayNames(c.Name);
                        return tfd;
                    });
            }
        }
    }
}
