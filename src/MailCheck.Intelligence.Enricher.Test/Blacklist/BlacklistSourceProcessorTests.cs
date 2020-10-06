using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DnsClient;
using FakeItEasy;
using MailCheck.Intelligence.Enricher.Blocklist;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Intelligence.Enricher.Test.Blocklist
{
    [TestFixture]
    public class BlocklistSourceProcessorTests
    {
        private BlocklistSourceProcessor _blocklistSourceProcessor;
        private ILookupClient _lookupClient;
        private ILogger<BlocklistSourceProcessor> _logger;

        [SetUp]
        public void Setup()
        {
            _lookupClient = A.Fake<ILookupClient>();
            _logger = A.Fake<ILogger<BlocklistSourceProcessor>>();
            _blocklistSourceProcessor = new BlocklistSourceProcessor(
                new BlockListSource
                {
                    Suffix = ".testSuffix", Data = new List<BlockListAddressData>()
                }, _lookupClient,
                _logger);
        }

        [Test]
        public async Task Ipv4LookupQueryIsCorrectlyFormed()
        {
            await _blocklistSourceProcessor.ProcessSource(new List<string> {"192.168.0.1"});

            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Ipv6LookupQueryIsCorrectlyFormed()
        {
            await _blocklistSourceProcessor.ProcessSource(new List<string> { "2041:0:1::875B:131B" });

            A.CallTo(() => _lookupClient.QueryAsync("b.1.3.1.b.5.7.8.0.0.0.0.0.0.0.0.0.0.0.0.1.0.0.0.0.0.0.0.1.4.0.2.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Ipv6LookupQuesryIsCorrectlyFormed()
        {
            await _blocklistSourceProcessor.ProcessSource(new List<string> { "192.168.0.1", "2a01:111:f400:fe0a::60f" });

            A.CallTo(() => _lookupClient.QueryAsync(A<string>._, QueryType.A, QueryClass.IN, A<CancellationToken>._))
                .MustHaveHappenedTwiceExactly();
        }
    }
}
