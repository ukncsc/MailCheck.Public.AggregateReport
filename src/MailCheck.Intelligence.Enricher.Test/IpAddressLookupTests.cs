using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Util;
using MailCheck.Intelligence.Enricher.Asn;
using MailCheck.Intelligence.Enricher.Blocklist;
using MailCheck.Intelligence.Enricher.ReverseDns;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Intelligence.Enricher.Test
{
    public class IpAddressLookupTests
    {
        private IpAddressLookup _ipAddressLookup;
        private IAsInfoProvider _asInfoProvider;
        private IBlocklistProvider _blocklistProvider;
        private IReverseDnsProvider _reverseDnsProvider;
        private ILogger<IpAddressLookup> _log;
        private IClock _clock;

        [SetUp]
        public void Setup()
        {
            _asInfoProvider = A.Fake<IAsInfoProvider>();
            _blocklistProvider = A.Fake<IBlocklistProvider>();
            _reverseDnsProvider = A.Fake<IReverseDnsProvider>();
            _log = A.Fake<ILogger<IpAddressLookup>>();
            _clock = A.Fake<IClock>();

            _ipAddressLookup = new IpAddressLookup(_asInfoProvider, _blocklistProvider, _reverseDnsProvider, _clock, _log);

            A.CallTo(() => _blocklistProvider.GetBlocklistAppearances(A<List<string>>._)).ReturnsLazily((List<string> arguments) =>
                {
                    return Task.FromResult(arguments.Select(x => new BlocklistResult(x, new List<BlocklistAppearance> { new BlocklistAppearance("flag", "source", Guid.NewGuid().ToString()) })
                ).ToList());
                });

            A.CallTo(() => _reverseDnsProvider.GetReverseDnsResult(A<List<string>>._)).ReturnsLazily((List<string> arguments) =>
                {
                    return Task.FromResult(arguments.Select(x => new ReverseDnsResult(x, new List<ReverseDnsResponse>
                        { new ReverseDnsResponse(Guid.NewGuid().ToString(), new List<string>())})).ToList());
                });

            A.CallTo(() => _asInfoProvider.GetAsInfo(A<List<string>>._)).ReturnsLazily((List<string> arguments) =>
                {
                    return Task.FromResult(arguments.Select(x => new AsInfo { IpAddress = x, Description = Guid.NewGuid().ToString() }).ToList());
                });

            A.CallTo(() => _clock.GetDateTimeUtc()).Returns(new DateTime(1999, 01, 01));
        }

        [Test]
        public async Task LookupForDifferentIpOnSameDateReturnsSameLookupForEachRequest()
        {
            List<IpAddressDetailsRequest> request = new List<IpAddressDetailsRequest>()
            {
                new IpAddressDetailsRequest("1", new DateTime(2000,01,01)),
                new IpAddressDetailsRequest("2", new DateTime(2000,01,01))
            };

            List<IpAddressDetails> result = await _ipAddressLookup.Lookup(request);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(result[0].Date, request[0].Date);
            Assert.AreEqual(result[0].IpAddress, request[0].IpAddress);
            Assert.AreEqual(result[1].Date, request[1].Date);
            Assert.AreEqual(result[1].IpAddress, request[1].IpAddress);

            Assert.AreNotEqual(result[0].Description, result[1].Description);
            Assert.AreNotEqual(result[0].BlockListOccurrences[0].Description, result[1].BlockListOccurrences[0].Description);
            Assert.AreNotEqual(result[0].ReverseDnsResponses[0].Host, result[1].ReverseDnsResponses[0].Host);
        }

        [Test]
        public async Task LookupForDifferentIpOnDifferentDateSameLookupForEachRequest()
        {
            List<IpAddressDetailsRequest> request = new List<IpAddressDetailsRequest>()
            {
                new IpAddressDetailsRequest("1", new DateTime(2000,01,01)),
                new IpAddressDetailsRequest("2", new DateTime(2000,01,02))
            };

            List<IpAddressDetails> result = await _ipAddressLookup.Lookup(request);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(result[0].Date, request[0].Date);
            Assert.AreEqual(result[0].IpAddress, request[0].IpAddress);
            Assert.AreEqual(result[1].Date, request[1].Date);
            Assert.AreEqual(result[1].IpAddress, request[1].IpAddress);

            Assert.AreNotEqual(result[0].Description, result[1].Description);
            Assert.AreNotEqual(result[0].BlockListOccurrences[0].Description, result[1].BlockListOccurrences[0].Description);
            Assert.AreNotEqual(result[0].ReverseDnsResponses[0].Host, result[1].ReverseDnsResponses[0].Host);
        }


        [Test]
        public async Task LookupForSameIpOnSameDateReturnsDifferentLookupForEachRequest()
        {
            List<IpAddressDetailsRequest> request = new List<IpAddressDetailsRequest>()
            {
                new IpAddressDetailsRequest("1", new DateTime(2000,01,01)),
                new IpAddressDetailsRequest("1", new DateTime(2000,01,01))
            };

            List<IpAddressDetails> result = await _ipAddressLookup.Lookup(request);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(result[0].Date, request[0].Date);
            Assert.AreEqual(result[0].IpAddress, request[0].IpAddress);
            Assert.AreEqual(result[1].Date, request[1].Date);
            Assert.AreEqual(result[1].IpAddress, request[1].IpAddress);

            Assert.AreEqual(result[0].Description, result[1].Description);
            Assert.AreEqual(result[0].BlockListOccurrences[0].Description, result[1].BlockListOccurrences[0].Description);
            Assert.AreEqual(result[0].ReverseDnsResponses[0].Host, result[1].ReverseDnsResponses[0].Host);
        }

        [Test]
        public async Task LookupForSameIpOnDifferentDateReturnsDifferentLookupForEachRequest()
        {
            List<IpAddressDetailsRequest> request = new List<IpAddressDetailsRequest>()
            {
                new IpAddressDetailsRequest("1", new DateTime(2000,01,01)),
                new IpAddressDetailsRequest("1", new DateTime(2000,01,02))
            };

            List<IpAddressDetails> result = await _ipAddressLookup.Lookup(request);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(result[0].Date, request[0].Date);
            Assert.AreEqual(result[0].IpAddress, request[0].IpAddress);
            Assert.AreEqual(result[1].Date, request[1].Date);
            Assert.AreEqual(result[1].IpAddress, request[1].IpAddress);

            Assert.AreEqual(result[0].Description, result[1].Description);
            Assert.AreEqual(result[0].BlockListOccurrences[0].Description, result[1].BlockListOccurrences[0].Description);
            Assert.AreEqual(result[0].ReverseDnsResponses[0].Host, result[1].ReverseDnsResponses[0].Host);
        }
    }
}