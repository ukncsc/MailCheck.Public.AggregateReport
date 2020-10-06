﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Enricher.Blocklist;
using NUnit.Framework;

namespace MailCheck.Intelligence.Enricher.Test.Blocklist
{
    [TestFixture]
    public class BlocklistProviderTests
    {
        private BlocklistProvider _blocklistProvider;
        private IBlocklistSourceProcessor _sourceProcessor1;
        private IBlocklistSourceProcessor _sourceProcessor2;

        [SetUp]
        public void Setup()
        {
            _sourceProcessor1 = A.Fake<IBlocklistSourceProcessor>();
            _sourceProcessor2 = A.Fake<IBlocklistSourceProcessor>();

            _blocklistProvider = new BlocklistProvider(new[] { _sourceProcessor1, _sourceProcessor2 });
        }

        [Test]
        public async Task SourceProcessorResultsAreGroupedByIpAddress()
        {
            List<string> ipAddresses = new List<string> { "ipAddress1", "ipAddress2" };

            A.CallTo(() => _sourceProcessor1.ProcessSource(ipAddresses))
                .Returns(Task.FromResult(ipAddresses.Select(x => GetResult("source1", x)).ToList()));
            A.CallTo(() => _sourceProcessor2.ProcessSource(ipAddresses))
                .Returns(Task.FromResult(ipAddresses.Select(x => GetResult("source2", x)).ToList()));

            List<BlocklistResult> result = await _blocklistProvider.GetBlocklistAppearances(ipAddresses);

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual("ipAddress1", result[0].IpAddress);
            Assert.AreEqual(2, result[0].BlocklistAppearances.Count);
            Assert.AreEqual("source1", result[0].BlocklistAppearances[0].Source);
            Assert.AreEqual("source2", result[0].BlocklistAppearances[1].Source);

            Assert.AreEqual("ipAddress2", result[1].IpAddress);
            Assert.AreEqual(2, result[1].BlocklistAppearances.Count);
            Assert.AreEqual("source1", result[1].BlocklistAppearances[0].Source);
            Assert.AreEqual("source2", result[1].BlocklistAppearances[1].Source);
        }

        private BlocklistResult GetResult(string source, string ipAddress)
        {
            return new BlocklistResult(ipAddress, new List<BlocklistAppearance>
            {
                new BlocklistAppearance("flag", source, "description")
            });
        }
    }
}
