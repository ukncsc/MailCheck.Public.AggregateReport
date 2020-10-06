using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Logging;
using MailCheck.Common.Data.Implementations;
using MailCheck.Intelligence.IpToAsnParser.Dao;
using MailCheck.Intelligence.IpToAsnParser.Fetchers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace MailCheck.Intelligence.IpToAsnParser.Test
{
    [TestFixture(Category = "Integration")]
    public class SmokeTest
    {
        private const string ConnectionString = "User ID = dev_iptoasn; Pwd=<pwd>;Host=localhost;Port=5432;Database=ip_intelligence; Pooling=true;";

        private IpAddressToAsnProcessor _cloudWatchProcessor;

        [SetUp]
        public void SetUp()
        {
            _cloudWatchProcessor = new ServiceCollection()
                .AddTransient<IpAddressToAsnProcessor>()
                .AddTransient<IAsnToIp4AddressFetcher, AsnToIp4AddressFetcher>()
                .AddTransient<IAsnToIpAddressParser, AsnToIpAddressParser>()
                .AddTransient<IIp4ToAsnDao, Ip4ToAsnDao>()
                .AddTransient<IConnectionInfoAsync>(_ => new StringConnectionInfoAsync(ConnectionString))
                .AddSerilogLogging()
                .BuildServiceProvider()
                .GetRequiredService<IpAddressToAsnProcessor>();
        }

        [Test]
        public async Task Test()
        {
            await _cloudWatchProcessor.Process();
        }
    }
}
