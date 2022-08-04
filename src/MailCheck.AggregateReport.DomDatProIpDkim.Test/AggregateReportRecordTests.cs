using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy.Sdk;
using MailCheck.AggregateReport.Contracts;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Host;
using NUnit.Framework;

namespace MailCheck.AggregateReport.DomDatProIpDkim.Test
{
    [TestFixture]
    public class AggregateReportRecordTests
    {
        [Test]
        public void DkimPassShouldConvertCorrectly()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched =
                CreateTestRecord(new List<string>() {"domain1:selector1:pass"}, count: 10);

            List<DomDatProIpDkimRecord>
                result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();
            
            Assert.AreEqual("domain1", result[0].DkimDomain);
            Assert.AreEqual("selector1", result[0].DkimSelector);
            Assert.AreEqual(10, result[0].DkimPass);
            Assert.AreEqual(0, result[0].DkimFail);
        }
        
        [Test]
        public void DkimFailShouldConvertCorrectly()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched =
                CreateTestRecord(new List<string>() {"domain11:selector1:fail"}, count: 12);

            List<DomDatProIpDkimRecord>
                result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();
            
            Assert.AreEqual("domain11", result[0].DkimDomain);
            Assert.AreEqual("selector1", result[0].DkimSelector);
            Assert.AreEqual(0, result[0].DkimPass);
            Assert.AreEqual(12, result[0].DkimFail);
        }
        
        [Test]
        public void DkimPassAndFailShouldConvertCorrectly()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched =
                CreateTestRecord(new List<string>() {"domain1:selector1:fail", "domain1:selector2:pass", "domain2:selector1:pass"}, count:13);

            List<DomDatProIpDkimRecord>
                result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();
            
            Assert.AreEqual("domain1", result[0].DkimDomain);
            Assert.AreEqual("selector1", result[0].DkimSelector);
            Assert.AreEqual(0, result[0].DkimPass);
            Assert.AreEqual(13, result[0].DkimFail);
            
            Assert.AreEqual("domain1", result[1].DkimDomain);
            Assert.AreEqual("selector2", result[1].DkimSelector);
            Assert.AreEqual(13, result[1].DkimPass);
            Assert.AreEqual(0, result[1].DkimFail);

            Assert.AreEqual("domain2", result[2].DkimDomain);
            Assert.AreEqual("selector1", result[2].DkimSelector);
            Assert.AreEqual(13, result[2].DkimPass);
            Assert.AreEqual(0, result[2].DkimFail);
        }

        [Test]
        public void ConvertingToRecordsShouldCreateAllProvidersRecord()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string>(){"domain1:selector1:pass"});

            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual("mail.host.provider", result[0].Provider);
            Assert.AreEqual("domain1", result[0].DkimDomain);

            Assert.AreEqual("All Providers", result[1].Provider);
            Assert.AreEqual("domain1", result[1].DkimDomain);
        }

        [TestCase(DmarcResult.fail, DmarcResult.fail, 1, "Blocklisted")]
        [TestCase(DmarcResult.fail, DmarcResult.fail, 0, "mail.host.provider")]
        [TestCase(DmarcResult.fail, DmarcResult.pass, 1, "mail.host.provider")]
        [TestCase(DmarcResult.fail, DmarcResult.pass, 0, "mail.host.provider")]
        [TestCase(DmarcResult.pass, DmarcResult.fail, 1, "mail.host.provider")]
        [TestCase(DmarcResult.pass, DmarcResult.fail, 0, "mail.host.provider")]
        [TestCase(DmarcResult.pass, DmarcResult.pass, 1, "mail.host.provider")]
        [TestCase(DmarcResult.pass, DmarcResult.pass, 0, "mail.host.provider")]
        public void ProviderShouldBeOverridenWhenSpfandDkimFailAndOnBlocklist(DmarcResult spfResult, DmarcResult dkimResult, int blocklistCount, string expectedProvider)
        {
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, spfResult, dkimResult, blockListCount: blocklistCount, hostProvider: hostProvider);
            var result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();
            Assert.AreEqual(expectedProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldBeOverridenWhenArcIsTrue()
        {
            string hostProvider = "mail.host.provider";
            string provider = "ARC-Forwarded";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, hostProvider: hostProvider, arc: true);
            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(provider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameUnknownAndDmarcPasses()
        {
            string hostName = "Unknown";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, hostProvider: hostProvider, hostName: hostName);
            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldBeOverridenWhenHostNameUnknownAndDmarcFails()
        {
            string hostName = "Unknown";
            string hostProvider = "mail.host.provider";
            string provider = "ReverseDnsFail";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, spfResult: DmarcResult.fail, dkimResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(provider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameMismatchedAndDmarcPasses()
        {
            string hostName = "Mismatch";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, hostProvider: hostProvider, hostName: hostName);
            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameMismatchedAndSpfPasses()
        {
            string hostName = "Mismatch";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, dkimResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameMismatchedAndDkimPasses()
        {
            string hostName = "Mismatch";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, spfResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameUnknownAndDkimPasses()
        {
            string hostName = "Unknown";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, spfResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameUnknownAndSpfPasses()
        {
            string hostName = "Unknown";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, dkimResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldBeOverridenWhenHostNameMismatchedAndDmarcFails()
        {
            string hostName = "Mismatch";
            string hostProvider = "mail.host.provider";
            string provider = "ReverseDnsFail";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:selector1:pass" }, spfResult: DmarcResult.fail, dkimResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomDatProIpDkimRecord> result = aggregateReportRecordEnriched.ToDomDatProIpDkimRecord();

            Assert.AreEqual(provider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        private AggregateReportRecordEnriched CreateTestRecord(List<string> dkimAuthResults, DmarcResult? spfResult = DmarcResult.pass,
            DmarcResult? dkimResult = DmarcResult.pass, Policy? disposition = Policy.none, int count = 0,
            string headerFrom = "digital.ncsc.gov.uk", string organisationDomain = "ncsc.gov.uk",
            int blockListCount = 0, string hostSourceIp = "192.168.1.1", string hostName = "hostName", string hostProvider = "mail.host.provider", bool arc = false)
        {
            return new AggregateReportRecordEnriched("id", "1", "correlationId", "causationId", "orgName", "reportId",
                DateTime.MaxValue, "domain", Alignment.r, Alignment.s, Policy.none, Policy.none, 1, "fo", hostSourceIp,
                count, disposition, dkimResult, spfResult, "envelopeTo", "envelopeFrom", headerFrom, organisationDomain,
                new List<string>(){$"{headerFrom}:{dkimResult}"}, 1, 1, dkimAuthResults, 1, 1, false, false, false, false, false, arc, false, hostName, "hostOrganisationDomain", hostProvider, 1,
                "asDescription", "country", blockListCount, blockListCount, blockListCount, blockListCount,
                blockListCount, blockListCount, blockListCount, blockListCount);
        }

        private long TallyAllCounts(DomDatProIpDkimRecord record)
        {
            return record.DkimPass + record.DkimFail;
        }

    }
}