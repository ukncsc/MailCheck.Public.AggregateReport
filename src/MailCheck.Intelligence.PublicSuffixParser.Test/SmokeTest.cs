using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Logging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Intelligence.PublicSuffixParser.Dao;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace MailCheck.Intelligence.PublicSuffixParser.Test
{
    [TestFixture(Category = "Integration")]
    public class SmokeTest
    {
        private const string ConnectionString = "User ID = dev_pubsuf; Pwd=<pwd>;Host=localhost;Port=5432;Database=ip_intelligence; Pooling=true;";

        private IProcess _process;

        [SetUp]
        public void SetUp()
        {
            _process = new ServiceCollection()
                .AddTransient<IProcess, PublicSuffixProcessor>()
                .AddTransient<IPublicSuffixFetcher, PublicSuffixFetcher>()
                .AddTransient<IPublicSuffixParser, PublicSuffixParser>()
                .AddTransient<IPublicSuffixDao, PublicSuffixDao>()
                .AddTransient<IConnectionInfoAsync>(_ => new StringConnectionInfoAsync(ConnectionString))
                .AddSerilogLogging()
                .BuildServiceProvider()
                .GetRequiredService<IProcess>();
        }

        [Test]
        public async Task Test()
        {
            await _process.Process();
        }
    }
}
