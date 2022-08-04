using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using FakeItEasy;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
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

            var source = new BlocklistSource
            {
                Suffix = ".testSuffix",
                Data = new List<BlocklistAddressData>
                    {
                        new BlocklistAddressData
                        {
                            IpAddress = "127.0.0.2",
                            Source = "blocklist.org",
                            Description = "Spam",
                            Flag = "spamsource"
                        },
                        new BlocklistAddressData
                        {
                            IpAddress = "127.0.0.3",
                            Source = "blocklist.org",
                            Description = "Malware",
                            Flag = "malware"
                        },
                        new BlocklistAddressData
                        {
                            IpAddress = "127.0.0.4",
                            Source = "blocklist.org",
                            Description = "Other",
                            Flag = "enduser"
                        }
                    }
            };

            _blocklistSourceProcessor = new BlocklistSourceProcessor(source, _lookupClient, _logger);
        }

        [Test]
        public async Task Ipv4LookupQueryIsCorrectlyFormed()
        {
            await _blocklistSourceProcessor.ProcessSource(new List<string> { "192.168.0.1" }).Single();

            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Ipv6LookupQueryIsCorrectlyFormed()
        {
            await _blocklistSourceProcessor.ProcessSource(new List<string> { "2041:0:1::875B:131B" }).Single();

            A.CallTo(() => _lookupClient.QueryAsync("b.1.3.1.b.5.7.8.0.0.0.0.0.0.0.0.0.0.0.0.1.0.0.0.0.0.0.0.1.4.0.2.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task MixedIpv4Ipv6LookupQueryIsCorrectlyFormed()
        {
            await Task.WhenAll(_blocklistSourceProcessor.ProcessSource(new List<string> { "192.168.0.1", "2a01:111:f400:fe0a::60f" }));

            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
               .MustHaveHappenedOnceExactly();
            A.CallTo(() => _lookupClient.QueryAsync("f.0.6.0.0.0.0.0.0.0.0.0.0.0.0.0.a.0.e.f.0.0.4.f.1.1.1.0.1.0.a.2.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ProcessSource_WhenEmptyDnsResponse_ReturnsEmptyResult()
        {
            var response = A.Fake<IDnsQueryResponse>();

            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
               .Returns(Task.FromResult(response));

            var result = await _blocklistSourceProcessor.ProcessSource(new List<string> { "192.168.0.1" }).Single();

            Assert.That(result.IpAddress, Is.EqualTo("192.168.0.1"));
            Assert.That(result.BlocklistAppearances, Is.Empty);
        }

        [Test]
        public async Task ProcessSource_WhenErrorDnsResponse_ReturnsEmptyResult()
        {
            var response = A.Fake<IDnsQueryResponse>();
            A.CallTo(() => response.HasError).Returns(true);
            A.CallTo(() => response.ErrorMessage).Returns("Some error");

            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
               .Returns(Task.FromResult(response));

            var result = await _blocklistSourceProcessor.ProcessSource(new List<string> { "192.168.0.1" }).Single();

            Assert.That(result.IpAddress, Is.EqualTo("192.168.0.1"));
            Assert.That(result.BlocklistAppearances, Is.Empty);

            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ProcessSource_WhenDnsException_ReturnsEmptyResult()
        {
            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
               .Throws<Exception>();

            var result = await _blocklistSourceProcessor.ProcessSource(new List<string> { "192.168.0.1" }).Single();

            Assert.That(result.IpAddress, Is.EqualTo("192.168.0.1"));
            Assert.That(result.BlocklistAppearances, Is.Empty);

            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ProcessSource_WhenPopulatedDnsResponse_ReturnsResult()
        {
            var response = A.Fake<IDnsQueryResponse>();
            A.CallTo(() => response.Answers).Returns(new[] { new ARecord(new ResourceRecordInfo("blah", ResourceRecordType.A, QueryClass.IN, 50, 10), IPAddress.Parse("127.0.0.3")) });

            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
               .Returns(Task.FromResult(response));

            var result = await _blocklistSourceProcessor.ProcessSource(new List<string> { "192.168.0.1" }).Single();

            Assert.That(result.IpAddress, Is.EqualTo("192.168.0.1"));
            Assert.That(result.BlocklistAppearances, Has.Count.EqualTo(1));

            var appearance = result.BlocklistAppearances[0];
            Assert.That(appearance.Description, Is.EqualTo("Malware"));
            Assert.That(appearance.Flag, Is.EqualTo("malware"));
            Assert.That(appearance.Source, Is.EqualTo("blocklist.org"));
        }

        [Test]
        public async Task ProcessSource_WhenMixedResponses_ReturnsResult()
        {
            var emptyResponse = A.Fake<IDnsQueryResponse>();

            var malwareResponse = A.Fake<IDnsQueryResponse>();
            A.CallTo(() => malwareResponse.Answers).Returns(new[] { 
                new ARecord(new ResourceRecordInfo("blah", ResourceRecordType.A, QueryClass.IN, 50, 10), IPAddress.Parse("127.0.0.3")) 
            });

            var otherResponse = A.Fake<IDnsQueryResponse>();
            A.CallTo(() => otherResponse.Answers).Returns(new[] {
                new ARecord(new ResourceRecordInfo("blah", ResourceRecordType.A, QueryClass.IN, 50, 10), IPAddress.Parse("127.0.0.3")),
                new ARecord(new ResourceRecordInfo("blah", ResourceRecordType.A, QueryClass.IN, 50, 10), IPAddress.Parse("127.0.0.4")) 
            });

            var errorResponse = A.Fake<IDnsQueryResponse>();
            A.CallTo(() => errorResponse.HasError).Returns(true);
            A.CallTo(() => errorResponse.ErrorMessage).Returns("Some error");

            A.CallTo(() => _lookupClient.QueryAsync("1.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
               .Returns(Task.FromResult(emptyResponse));

            A.CallTo(() => _lookupClient.QueryAsync("2.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
               .Returns(Task.FromResult(malwareResponse));

            A.CallTo(() => _lookupClient.QueryAsync("3.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
               .Returns(Task.FromResult(otherResponse));

            A.CallTo(() => _lookupClient.QueryAsync("4.0.168.192.testSuffix", QueryType.A, QueryClass.IN, A<CancellationToken>._))
               .Returns(Task.FromResult(errorResponse));

            var results = await Task.WhenAll(_blocklistSourceProcessor.ProcessSource(new List<string> { "192.168.0.1", "192.168.0.2", "192.168.0.3", "192.168.0.4" }));

            Assert.That(results, Has.Length.EqualTo(4));

            BlocklistResult result;
            BlocklistAppearance appearance;

            result = results[0];
            Assert.That(result.IpAddress, Is.EqualTo("192.168.0.1"));
            Assert.That(result.BlocklistAppearances, Is.Empty);

            result = results[1];
            Assert.That(result.IpAddress, Is.EqualTo("192.168.0.2"));
            Assert.That(result.BlocklistAppearances, Has.Count.EqualTo(1));

            appearance = result.BlocklistAppearances[0];
            Assert.That(appearance.Description, Is.EqualTo("Malware"));
            Assert.That(appearance.Flag, Is.EqualTo("malware"));
            Assert.That(appearance.Source, Is.EqualTo("blocklist.org"));

            result = results[2];
            Assert.That(result.IpAddress, Is.EqualTo("192.168.0.3"));
            Assert.That(result.BlocklistAppearances, Has.Count.EqualTo(2));

            appearance = result.BlocklistAppearances[0];
            Assert.That(appearance.Description, Is.EqualTo("Malware"));
            Assert.That(appearance.Flag, Is.EqualTo("malware"));
            Assert.That(appearance.Source, Is.EqualTo("blocklist.org"));

            appearance = result.BlocklistAppearances[1];
            Assert.That(appearance.Description, Is.EqualTo("Other"));
            Assert.That(appearance.Flag, Is.EqualTo("enduser"));
            Assert.That(appearance.Source, Is.EqualTo("blocklist.org"));

            result = results[3];
            Assert.That(result.IpAddress, Is.EqualTo("192.168.0.4"));
            Assert.That(result.BlocklistAppearances, Is.Empty);
        }
    }
}
