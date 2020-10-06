using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using FakeItEasy;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Logging;
using MailCheck.Common.Messaging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Intelligence.AsnInfoParser;
using MailCheck.Intelligence.AsnInfoParser.Dao;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace MailCheck.Intelligence.AsnToDescriptionAndCountryParser.Test
{

    [TestFixture(Category = "Integration")]
    public class SmokeTest
    {
        private const string ConnectionString = "User ID = dev_asntodescription; Pwd=<pwd>;Host=localhost;Port=5432;Database=ip_intelligence; Pooling=true;";

        private IProcess _processor;

        [SetUp]
        public void SetUp()
        {
            _processor = new ServiceCollection()
                .AddTransient<IProcess, AsnToDescriptionAndCountryProcessor>()
                .AddTransient<IAsnToDescriptionAndCountryFetcher, AsnToDescriptionAndCountryFetcher>()
                .AddTransient<IAsnToDescriptionAndCountryParser, AsnInfoParser.AsnToDescriptionAndCountryParser>()
                .AddTransient<IAsnToDescriptionAndCountryDao, AsnToDescriptionAndCountryDao>()
                .AddTransient<IConnectionInfoAsync>(_ =>  new StringConnectionInfoAsync(ConnectionString))
                .AddSerilogLogging()
                .BuildServiceProvider()
                .GetRequiredService<IProcess>();
        }

        [Test]
        public async Task Test()
        {
            await _processor.Process();
        }
    }

    [TestFixture(Category = "Integration")]
    public class SmokeTest2
    {
        private const string ConnectionString = "User ID = dev_asntodescription; Pwd=<pwd>;Host=localhost;Port=5432;Database=ip_intelligence; Pooling=true;";

        private LambdaEntryPoint _lambdaEntryPoint;

        [SetUp]
        public void SetUp()
        {
            Environment.SetEnvironmentVariable("RemainingTimeThresholdSeconds", "10");
            Environment.SetEnvironmentVariable(nameof(ConnectionString), ConnectionString);

            _lambdaEntryPoint = new LambdaEntryPoint();
        }

        [Test]
        public async Task Test()
        {
            await _lambdaEntryPoint.FunctionHandler(
                new ScheduledEvent(string.Empty, string.Empty, null, null, DateTime.UtcNow, Guid.NewGuid(),
                    new List<string>()),
                A.Fake<ILambdaContext>());
        }
    }
}
