using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using FakeItEasy;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Enricher.ReverseDns.Dns;
using MailCheck.Intelligence.Enricher.ReverseDns.Processor;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Intelligence.Enricher.Test
{
    [TestFixture]
    public class ReverseDnsLookupTests
    {
        private IDnsResolver _dnsResolver;
        private ILogger<ReverseDnsLookup> _log;
        private ReverseDnsLookup _reverseDnsLookup;

        [SetUp]
        public void SetUp()
        {
            _dnsResolver = A.Fake<IDnsResolver>();
            _log = A.Fake<ILogger<ReverseDnsLookup>>();
            _reverseDnsLookup = new ReverseDnsLookup(_dnsResolver, _log);
        }

        [Test]
        public async Task LookupReturnsInconclusiveForBadlyFormattedIpAddress()
        {
            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup("not an ip address");

            Assert.True(reverseDnsResult.IsInconclusive);
            Assert.Null(reverseDnsResult.ForwardResponses);
        }

        [TestCase("192.168.0.1")]
        [TestCase("2001:1:2:3:4:5:6:7")]
        public async Task LookupReturnsEmptyForPtrServerFailure(string ipAddress)
        {
            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(true, "Server Failure", null, null);
            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);

            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup(ipAddress);

            Assert.False(reverseDnsResult.IsInconclusive);
            Assert.IsEmpty(reverseDnsResult.ForwardResponses);
        }

        [TestCase("192.168.0.1")]
        [TestCase("2001:1:2:3:4:5:6:7")]
        public async Task LookupReturnsEmptyForPtrNonExistentDomain(string ipAddress)
        {
            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(true, "Non-Existent Domain", null, null);
            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);

            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup(ipAddress);

            Assert.False(reverseDnsResult.IsInconclusive);
            Assert.IsEmpty(reverseDnsResult.ForwardResponses);
        }

        [TestCase("192.168.0.1")]
        [TestCase("2001:1:2:3:4:5:6:7")]
        public async Task LookupReturnsInconclusiveForPtrError(string ipAddress)
        {
            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(true, "Some other error", null, null);
            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);

            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup(ipAddress);

            Assert.True(reverseDnsResult.IsInconclusive);
            Assert.IsNull(reverseDnsResult.ForwardResponses);
        }

        [TestCase("192.168.0.1")]
        [TestCase("2001:1:2:3:4:5:6:7")]
        public async Task LookupReturnsEmptyResponseForForwardServerFailure(string ipAddress)
        {
            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(false, null, null, new List<string> { "testHost" });
            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);

            ReverseDnsQueryResponse forwardDnsQueryResponse = new ReverseDnsQueryResponse(true, "Server Failure", null, null);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>("testHost", QueryType.A)).Returns(forwardDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>("testHost", QueryType.AAAA)).Returns(forwardDnsQueryResponse);

            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup(ipAddress);

            Assert.False(reverseDnsResult.IsInconclusive);
            Assert.AreEqual(1, reverseDnsResult.ForwardResponses.Count);
            Assert.IsEmpty(reverseDnsResult.ForwardResponses[0].IpAddresses);
        }

        [TestCase("192.168.0.1")]
        [TestCase("2001:1:2:3:4:5:6:7")]
        public async Task LookupReturnsEmptyForForwardNonExistentDomain(string ipAddress)
        {
            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(false, null, null, new List<string> { "testHost" });
            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);

            ReverseDnsQueryResponse forwardDnsQueryResponse = new ReverseDnsQueryResponse(true, "Non-Existent Domain", null, null);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>("testHost", QueryType.A)).Returns(forwardDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>("testHost", QueryType.AAAA)).Returns(forwardDnsQueryResponse);

            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup(ipAddress);

            Assert.False(reverseDnsResult.IsInconclusive);
            Assert.AreEqual(1, reverseDnsResult.ForwardResponses.Count);
            Assert.IsEmpty(reverseDnsResult.ForwardResponses[0].IpAddresses);
        }

        [TestCase("192.168.0.1")]
        [TestCase("2001:1:2:3:4:5:6:7")]
        public async Task LookupReturnsInconclusiveForForwardError(string ipAddress)
        {
            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(false, null, null, new List<string> { "testHost" });
            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);

            ReverseDnsQueryResponse forwardDnsQueryResponse = new ReverseDnsQueryResponse(true, "Some other error", null, null);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>("testHost", QueryType.A)).Returns(forwardDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>("testHost", QueryType.AAAA)).Returns(forwardDnsQueryResponse);

            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup(ipAddress);

            Assert.True(reverseDnsResult.IsInconclusive);
            Assert.IsNull(reverseDnsResult.ForwardResponses);
        }

        [TestCase("192.168.0.1")]
        [TestCase("2001:1:2:3:4:5:6:7")]
        public async Task LookupReturnsInconclusiveOnException(string ipAddress)
        {
            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Throws(new Exception());

            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup(ipAddress);

            Assert.True(reverseDnsResult.IsInconclusive);
            Assert.IsNull(reverseDnsResult.ForwardResponses);
        }

        [TestCase("192.168.0.1")]
        [TestCase("2001:1:2:3:4:5:6:7")]
        public async Task LookupReturnsForwardResponses(string ipAddress)
        {
            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(false, null, null, new List<string> { "testHost1", "testHost2" });
            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);

            ReverseDnsQueryResponse forwardDnsQueryResponse1 = new ReverseDnsQueryResponse(false, null, null, new List<string> { "1.1.1.1", "2.2.2.2" });
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>("testHost1", QueryType.A)).Returns(forwardDnsQueryResponse1);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>("testHost1", QueryType.AAAA)).Returns(forwardDnsQueryResponse1);

            ReverseDnsQueryResponse forwardDnsQueryResponse2 = new ReverseDnsQueryResponse(false, null, null, new List<string> { "3.3.3.3", "4.4.4.4" });
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>("testHost2", QueryType.A)).Returns(forwardDnsQueryResponse2);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>("testHost2", QueryType.AAAA)).Returns(forwardDnsQueryResponse2);

            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup(ipAddress);

            Assert.False(reverseDnsResult.IsInconclusive);
            Assert.AreEqual(2, reverseDnsResult.ForwardResponses.Count);
            Assert.AreEqual("1.1.1.1", reverseDnsResult.ForwardResponses[0].IpAddresses[0]);
            Assert.AreEqual("2.2.2.2", reverseDnsResult.ForwardResponses[0].IpAddresses[1]);
            Assert.AreEqual("3.3.3.3", reverseDnsResult.ForwardResponses[1].IpAddresses[0]);
            Assert.AreEqual("4.4.4.4", reverseDnsResult.ForwardResponses[1].IpAddresses[1]);
        }
    }
}
