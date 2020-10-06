using System;
using System.Collections.Generic;
using FakeItEasy;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Enricher.ProviderResolver;
using NUnit.Framework;

namespace MailCheck.Intelligence.Enricher.Test
{
    [TestFixture]
    public class AggregateReportRecordEnrichedFactoryTests
    {
        private IProviderResolver _providerResolver;
        private AggregateReportRecordEnrichedFactory _aggregateReportRecordEnrichedFactory;

        [SetUp]
        public void SetUp()
        {
            _providerResolver = A.Fake<IProviderResolver>();
            _aggregateReportRecordEnrichedFactory = new AggregateReportRecordEnrichedFactory(_providerResolver);
        }

        [TestCase("proxy", "ProxyBlockListCount")]
        [TestCase("suspiciousnetwork", "SuspiciousNetworkBlockListCount")]
        [TestCase("highjackednetwork", "HijackedNetworkBlockListCount")]
        [TestCase("endusernetwork", "EndUserNetworkBlockListCount")]
        [TestCase("spamsource", "SpamSourceBlockListCount")]
        [TestCase("malware", "MalwareBlockListCount")]
        [TestCase("enduser", "EndUserBlockListCount")]
        [TestCase("bouncereflector", "BounceReflectorBlockListCount")]
        public void BlocklistFlagsSetCorrectly(string flagName, string countField)
        {
            IpAddressDetails response = CreateResponse();
            response.BlockListOccurrences.Add(new BlocklistAppearance(flagName, "source", "description"));
            AggregateReportRecord source = CreateSource();

            AggregateReportRecordEnriched result = _aggregateReportRecordEnrichedFactory.Create(response, source, "", "", "");

            Assert.AreEqual(1, typeof(AggregateReportRecordEnriched).GetProperty(countField)?.GetValue(result, null));
        }

        private IpAddressDetails CreateResponse(string asDescription = "")
        {
            return new IpAddressDetails("", DateTime.MaxValue, 12, asDescription,
                "", new List<BlocklistAppearance>(), new List<ReverseDnsResponse>(), DateTime.MaxValue, DateTime.MaxValue, DateTime.MaxValue);
        }

        private AggregateReportRecord CreateSource(DmarcResult dkimResult = DmarcResult.pass, DmarcResult spfResult = DmarcResult.pass)
        {
            return new AggregateReportRecord("", "", "", DateTime.MinValue, "",
                Alignment.r, Alignment.r, Policy.none, Policy.none, 0, "", "", 0, Policy.none,
                dkimResult, spfResult, "", "", "", new List<string> { "" }, 0, 0, new List<string>() { "" }, 0, 0, false, false,
                false, false, false, false, false);
        }
    }
}
