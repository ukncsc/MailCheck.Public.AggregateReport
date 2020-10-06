using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using FakeItEasy;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Enricher.ReverseDns.Dns;
using MailCheck.Intelligence.Enricher.ReverseDns.Processor;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Intelligence.Enricher.Test
{
    [TestFixture]
    public class ReverseDnsLookupTests
    {
        private IDnsResolver _dnsResolver;
        private TestLogger<ReverseDnsLookup> _log;
        private IOrganisationalDomainProvider _organisationalDomainProvider;

        [SetUp]
        public void SetUp()
        {
            _dnsResolver = A.Fake<IDnsResolver>();
            _organisationalDomainProvider = A.Fake<IOrganisationalDomainProvider>();
            _log = new TestLogger<ReverseDnsLookup>();
        }

        [TestCase("192.168.0.x1")]
        [TestCase(":x:ffff:c0a8:1")]
        public async Task ItShouldReturnEmptyListForBadIpAddress(string ipAddress)
        {
            var reverseDnsLookup = new ReverseDnsLookup(_dnsResolver, _log);
            ReverseDnsResult result = await reverseDnsLookup.Lookup(ipAddress);
            Assert.That(result.ForwardResponses.Count, Is.EqualTo(0));
            Assert.IsNull(result.OriginalIpAddress);
        }

        [TestCase("192.168.0.1")]
        [TestCase("::ffff:c0a8:1")]
        public async Task ItShouldLogErrorWhenUnableToDoLookupForPtr(string ipAddress)
        {
            ReverseDnsQueryResponse response = new ReverseDnsQueryResponse(true, "Something has gone wrong!", null);

            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(response);

            var reverseDnsLookup = new ReverseDnsLookup(_dnsResolver, _log);

            ReverseDnsResult result = await reverseDnsLookup.Lookup(ipAddress);

            Assert.That(_log.Warnings.Count, Is.EqualTo(1));
            Assert.That(result.ForwardResponses.Count, Is.EqualTo(0));
            Assert.IsNull(result.OriginalIpAddress);
        }

        [TestCase("192.168.0.1")]
        [TestCase("::ffff:c0a8:1")]
        public async Task ItShouldLogErrorWhenValidPtrButFailsOnForwardLookup(string ipAddress)
        {
            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(false, string.Empty, new List<string>() { "google.com" });
            ReverseDnsQueryResponse addressDnsQueryResponse = new ReverseDnsQueryResponse(true, "Something has gone wrong!", null);

            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>(A<string>._, QueryType.A)).Returns(addressDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>(A<string>._, QueryType.AAAA)).Returns(addressDnsQueryResponse);

            var reverseDnsLookup = new ReverseDnsLookup(_dnsResolver, _log);

            ReverseDnsResult reverseDnsResult = await reverseDnsLookup.Lookup(ipAddress);

            Assert.That(_log.Warnings.Count, Is.EqualTo(1));
            Assert.That(reverseDnsResult.ForwardResponses.Count, Is.EqualTo(0));
        }

        [TestCase("192.168.0.1")]
        [TestCase("::ffff:c0a8:1")]
        public async Task ItShouldOnlyProcessValidLookupsAndNotStopWhenInvalid(string ipAddress)
        {
            string googleHostName = "google.com";
            string googleIp1 = "192.168.0.1";
            string yahooHostName = "yahoo.com";

            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(false, string.Empty, new List<string>() { googleHostName, yahooHostName });
            ReverseDnsQueryResponse host1AddressDnsQueryResponse = new ReverseDnsQueryResponse(false, string.Empty, new List<string>() { googleIp1 });
            ReverseDnsQueryResponse host2AaddressDnsQueryResponse = new ReverseDnsQueryResponse(true, "Something has gone wrong!", null);

            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);

            //setup for only host1
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>(googleHostName, QueryType.A)).Returns(host1AddressDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>(googleHostName, QueryType.AAAA)).Returns(host1AddressDnsQueryResponse);

            //setup for only host1
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>(yahooHostName, QueryType.A)).Returns(host2AaddressDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>(yahooHostName, QueryType.AAAA)).Returns(host2AaddressDnsQueryResponse);

            var reverseDnsLookup = new ReverseDnsLookup(_dnsResolver, _log);

            ReverseDnsResult reverseDnsResults = await reverseDnsLookup.Lookup(ipAddress);

            Assert.That(_log.Warnings.Count, Is.EqualTo(1));

            Assert.That(reverseDnsResults.ForwardResponses.Count, Is.EqualTo(1));
            Assert.That(reverseDnsResults.ForwardResponses[0].Host, Is.EqualTo(googleHostName));
            Assert.That(reverseDnsResults.ForwardResponses[0].IpAddresses.Count, Is.EqualTo(1));
            Assert.That(reverseDnsResults.ForwardResponses[0].IpAddresses[0], Is.EqualTo(googleIp1));
        }

        [TestCase("192.168.0.1")]
        [TestCase("::ffff:c0a8:1")]
        public async Task ItShouldProcessAllValidLookups(string ipAddress)
        {
            string googleHostName = "google.com";
            string googleIp1 = "192.168.0.1";
            string googleIp2 = "192.168.0.2";

            string yahooHostName = "yahoo.com";
            string yahooIp1 = "192.168.1.1";

            ReverseDnsQueryResponse ptrDnsQueryResponse = new ReverseDnsQueryResponse(false, string.Empty, new List<string>() { googleHostName, yahooHostName });
            ReverseDnsQueryResponse googleAddressDnsQueryResponse = new ReverseDnsQueryResponse(false, string.Empty, new List<string>() { googleIp1, googleIp2 });
            ReverseDnsQueryResponse yahooAddressDnsQueryResponse = new ReverseDnsQueryResponse(false, string.Empty, new List<string>() { yahooIp1 });

            A.CallTo(() => _dnsResolver.QueryPtrAsync(A<IPAddress>._)).Returns(ptrDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>(googleHostName, QueryType.A)).Returns(googleAddressDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>(googleHostName, QueryType.AAAA)).Returns(googleAddressDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<ARecord>(yahooHostName, QueryType.A)).Returns(yahooAddressDnsQueryResponse);
            A.CallTo(() => _dnsResolver.QueryAddressAsync<AaaaRecord>(yahooHostName, QueryType.AAAA)).Returns(yahooAddressDnsQueryResponse);

            var reverseDnsLookup = new ReverseDnsLookup(_dnsResolver, _log);

            ReverseDnsResult reverseDnsResult = await reverseDnsLookup.Lookup(ipAddress);

            Assert.That(_log.Errors.Count, Is.EqualTo(0));
            Assert.That(_log.Warnings.Count, Is.EqualTo(0));

            Assert.That(reverseDnsResult.ForwardResponses.Count, Is.EqualTo(2));
            Assert.That(reverseDnsResult.ForwardResponses[0].Host, Is.EqualTo(googleHostName));
            Assert.That(reverseDnsResult.ForwardResponses[0].IpAddresses.Count, Is.EqualTo(2));
            Assert.That(reverseDnsResult.ForwardResponses[0].IpAddresses[0], Is.EqualTo(googleIp1));
            Assert.That(reverseDnsResult.ForwardResponses[0].IpAddresses[1], Is.EqualTo(googleIp2));
            Assert.That(reverseDnsResult.ForwardResponses[1].Host, Is.EqualTo(yahooHostName));
            Assert.That(reverseDnsResult.ForwardResponses[1].IpAddresses.Count, Is.EqualTo(1));
            Assert.That(reverseDnsResult.ForwardResponses[1].IpAddresses[0], Is.EqualTo(yahooIp1));
        }
    }

    #region test infrastructure
    public class TestLogger<T> : ILogger<T>
    {
        private readonly List<Tuple<LogLevel, string>> _logEntries = new List<Tuple<LogLevel, string>>();

        public List<string> Warnings => _logEntries.Where(_ => _.Item1 == LogLevel.Warning).Select(_ => _.Item2).ToList();
        public List<string> Information => _logEntries.Where(_ => _.Item1 == LogLevel.Information).Select(_ => _.Item2).ToList();
        public List<string> Errors => _logEntries.Where(_ => _.Item1 == LogLevel.Error).Select(_ => _.Item2).ToList();
        public List<string> Debug => _logEntries.Where(_ => _.Item1 == LogLevel.Debug).Select(_ => _.Item2).ToList();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logEntries.Add(Tuple.Create(logLevel, formatter(state, exception)));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
