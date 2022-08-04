using FakeItEasy;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Enricher.ReverseDns;
using MailCheck.Intelligence.Enricher.ReverseDns.Processor;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MailCheck.Intelligence.Enricher.Test.ReverseDns
{
    [TestFixture]
    public class ReverseDnsProviderTests
    {
        private ReverseDnsProvider _reverseDnsProvider;
        private IReverseDnsLookup _reverseDnsLookup;
        private IOrganisationalDomainProvider _organisationalDomainProvider;
        private ILogger<ReverseDnsProvider> _logger;

        [SetUp]
        public void SetUp()
        {
            _reverseDnsLookup = A.Fake<IReverseDnsLookup>();
            _organisationalDomainProvider = A.Fake<IOrganisationalDomainProvider>();
            _logger = A.Fake<ILogger<ReverseDnsProvider>>();
            _reverseDnsProvider = new ReverseDnsProvider(_reverseDnsLookup, _organisationalDomainProvider, _logger);
        }

        [Test]
        public async Task GetReverseDnsResultReturnsInconclusiveResult()
        {
            ReverseDnsResult inconclusiveResultFromLookup = ReverseDnsResult.Inconclusive("");
            A.CallTo(() => _reverseDnsLookup.Lookup(A<string>._)).Returns(inconclusiveResultFromLookup);

            List<ReverseDnsResult> result = await _reverseDnsProvider.GetReverseDnsResult(new List<string> { "" });

            Assert.AreSame(inconclusiveResultFromLookup, result[0]);
        }

        [Test]
        public async Task GetReverseDnsResultReturnsUnknownWhenForwardResponsesNull()
        {
            ReverseDnsResult reverseDnsResult = new ReverseDnsResult("", null);
            A.CallTo(() => _reverseDnsLookup.Lookup(A<string>._)).Returns(reverseDnsResult);

            List<ReverseDnsResult> result = await _reverseDnsProvider.GetReverseDnsResult(new List<string> { "" });

            Assert.AreEqual("Unknown", result[0].ForwardResponses[0].Host);
            Assert.AreEqual("Unknown", result[0].ForwardResponses[0].OrganisationalDomain);
            Assert.IsEmpty(result[0].ForwardResponses[0].IpAddresses);
        }

        [Test]
        public async Task GetReverseDnsResultReturnsUnknownWhenForwardResponsesEmpty()
        {
            ReverseDnsResult reverseDnsResult = new ReverseDnsResult("", new List<ReverseDnsResponse>());
            A.CallTo(() => _reverseDnsLookup.Lookup(A<string>._)).Returns(reverseDnsResult);

            List<ReverseDnsResult> result = await _reverseDnsProvider.GetReverseDnsResult(new List<string> { "" });

            Assert.AreEqual("Unknown", result[0].ForwardResponses[0].Host);
            Assert.AreEqual("Unknown", result[0].ForwardResponses[0].OrganisationalDomain);
            Assert.IsEmpty(result[0].ForwardResponses[0].IpAddresses);
        }

        [Test]
        public async Task GetReverseDnsResultReturnsUnknownWhenForwardResponsesPopulatedWithNullForwardIpAddresses()
        {
            ReverseDnsResult reverseDnsResult = new ReverseDnsResult("", new List<ReverseDnsResponse> { new ReverseDnsResponse("", null) });
            A.CallTo(() => _reverseDnsLookup.Lookup(A<string>._)).Returns(reverseDnsResult);

            List<ReverseDnsResult> result = await _reverseDnsProvider.GetReverseDnsResult(new List<string> { "" });

            Assert.AreEqual("Unknown", result[0].ForwardResponses[0].Host);
            Assert.AreEqual("Unknown", result[0].ForwardResponses[0].OrganisationalDomain);
            Assert.IsEmpty(result[0].ForwardResponses[0].IpAddresses);
        }

        [Test]
        public async Task GetReverseDnsResultReturnsUnknownWhenForwardResponsesPopulatedWithEmptyForwardIpAddresses()
        {
            ReverseDnsResult reverseDnsResult = new ReverseDnsResult("", new List<ReverseDnsResponse> { new ReverseDnsResponse("", new List<string>()) });
            A.CallTo(() => _reverseDnsLookup.Lookup(A<string>._)).Returns(reverseDnsResult);

            List<ReverseDnsResult> result = await _reverseDnsProvider.GetReverseDnsResult(new List<string> { "" });

            Assert.AreEqual("Unknown", result[0].ForwardResponses[0].Host);
            Assert.AreEqual("Unknown", result[0].ForwardResponses[0].OrganisationalDomain);
            Assert.IsEmpty(result[0].ForwardResponses[0].IpAddresses);
        }

        [Test]
        public async Task GetReverseDnsResultReturnsMismatch()
        {
            ReverseDnsResult reverseDnsResult = new ReverseDnsResult("1.1.1.1", new List<ReverseDnsResponse> { new ReverseDnsResponse("", new List<string> { "2.2.2.2" }) });
            A.CallTo(() => _reverseDnsLookup.Lookup("1.1.1.1")).Returns(reverseDnsResult);

            List<ReverseDnsResult> result = await _reverseDnsProvider.GetReverseDnsResult(new List<string> { "1.1.1.1" });

            Assert.AreEqual("Mismatch", result[0].ForwardResponses[0].Host);
            Assert.AreEqual("Mismatch", result[0].ForwardResponses[0].OrganisationalDomain);
            CollectionAssert.Contains(result[0].ForwardResponses[0].IpAddresses, "2.2.2.2");
        }
    }
}
