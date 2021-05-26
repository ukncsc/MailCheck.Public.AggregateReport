using System;
using System.Collections.Generic;
using MailCheck.AggregateReport.Contracts;
using NUnit.Framework;

namespace MailCheck.AggregateReport.RollupDomainDateProvider.Test
{
    [TestFixture]
    public class AggregateReportRecordTests
    {
        [TestCase(DmarcResult.pass, DmarcResult.pass, Policy.none, "SpfPassDkimPassNone")]
        [TestCase(DmarcResult.pass, DmarcResult.fail, Policy.none, "SpfPassDkimFailNone")]
        [TestCase(DmarcResult.fail, DmarcResult.pass, Policy.none, "SpfFailDkimPassNone")]
        [TestCase(DmarcResult.fail, DmarcResult.fail, Policy.none, "SpfFailDkimFailNone")]
        [TestCase(DmarcResult.pass, DmarcResult.pass, Policy.quarantine, "SpfPassDkimPassQuarantine")]
        [TestCase(DmarcResult.pass, DmarcResult.fail, Policy.quarantine, "SpfPassDkimFailQuarantine")]
        [TestCase(DmarcResult.fail, DmarcResult.pass, Policy.quarantine, "SpfFailDkimPassQuarantine")]
        [TestCase(DmarcResult.fail, DmarcResult.fail, Policy.quarantine, "SpfFailDkimFailQuarantine")]
        [TestCase(DmarcResult.pass, DmarcResult.pass, Policy.reject, "SpfPassDkimPassReject")]
        [TestCase(DmarcResult.pass, DmarcResult.fail, Policy.reject, "SpfPassDkimFailReject")]
        [TestCase(DmarcResult.fail, DmarcResult.pass, Policy.reject, "SpfFailDkimPassReject")]
        [TestCase(DmarcResult.fail, DmarcResult.fail, Policy.reject, "SpfFailDkimFailReject")]
        public void ConvertingToRecordShouldTallyCorrectly(DmarcResult? spfResult, DmarcResult? dkimResult, Policy? disposition, string aggregationField)
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(spfResult, dkimResult, disposition, 10000);

            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual("hostProvider", result[0].Provider);
            Assert.AreEqual("digital.ncsc.gov.uk", result[0].Domain);
            Assert.AreEqual(10000, TallyAllCounts(result[0]));
            Assert.AreEqual(10000, typeof(DomainDateProviderRecord).GetProperty(aggregationField)?.GetValue(result[0], null));

            Assert.AreEqual("hostProvider", result[1].Provider);
            Assert.AreEqual("ncsc.gov.uk", result[1].Domain);
            Assert.AreEqual(10000, TallyAllCounts(result[1]));
            Assert.AreEqual(10000, typeof(DomainDateProviderRecord).GetProperty(aggregationField)?.GetValue(result[0], null));

            Assert.AreEqual("All Providers", result[2].Provider);
            Assert.AreEqual("digital.ncsc.gov.uk", result[2].Domain);
            Assert.AreEqual(10000, TallyAllCounts(result[2]));
            Assert.AreEqual(10000, typeof(DomainDateProviderRecord).GetProperty(aggregationField)?.GetValue(result[1], null));

            Assert.AreEqual("All Providers", result[3].Provider);
            Assert.AreEqual("ncsc.gov.uk", result[3].Domain);
            Assert.AreEqual(10000, TallyAllCounts(result[3]));
            Assert.AreEqual(10000, typeof(DomainDateProviderRecord).GetProperty(aggregationField)?.GetValue(result[1], null));
        }

        [Test]
        public void ConvertingToRecordsShouldSplitIntoSubdomainsAndAddAllProvider()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(headerFrom: "a.b.c.d.e.gov.uk", organisationDomain: "e.gov.uk");

            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(10, result.Count);

            Assert.AreEqual("hostProvider", result[0].Provider);
            Assert.AreEqual("hostProvider", result[1].Provider);
            Assert.AreEqual("hostProvider", result[2].Provider);
            Assert.AreEqual("hostProvider", result[3].Provider);
            Assert.AreEqual("hostProvider", result[4].Provider);
            Assert.AreEqual("All Providers", result[5].Provider);
            Assert.AreEqual("All Providers", result[6].Provider);
            Assert.AreEqual("All Providers", result[7].Provider);
            Assert.AreEqual("All Providers", result[8].Provider);
            Assert.AreEqual("All Providers", result[9].Provider);

            Assert.AreEqual("a.b.c.d.e.gov.uk", result[0].Domain);
            Assert.AreEqual("b.c.d.e.gov.uk", result[1].Domain);
            Assert.AreEqual("c.d.e.gov.uk", result[2].Domain);
            Assert.AreEqual("d.e.gov.uk", result[3].Domain);
            Assert.AreEqual("e.gov.uk", result[4].Domain);
            Assert.AreEqual("a.b.c.d.e.gov.uk", result[5].Domain);
            Assert.AreEqual("b.c.d.e.gov.uk", result[6].Domain);
            Assert.AreEqual("c.d.e.gov.uk", result[7].Domain);
            Assert.AreEqual("d.e.gov.uk", result[8].Domain);
            Assert.AreEqual("e.gov.uk", result[9].Domain);
        }

        [TestCase(DmarcResult.fail, DmarcResult.fail, 1, "Blocklisted")]
        [TestCase(DmarcResult.fail, DmarcResult.fail, 0, "hostProvider")]
        [TestCase(DmarcResult.fail, DmarcResult.pass, 1, "hostProvider")]
        [TestCase(DmarcResult.fail, DmarcResult.pass, 0, "hostProvider")]
        [TestCase(DmarcResult.pass, DmarcResult.fail, 1, "hostProvider")]
        [TestCase(DmarcResult.pass, DmarcResult.fail, 0, "hostProvider")]
        [TestCase(DmarcResult.pass, DmarcResult.pass, 1, "hostProvider")]
        [TestCase(DmarcResult.pass, DmarcResult.pass, 0, "hostProvider")]
        public void ProviderShouldBeOverridenWhenSpfandDkimFailAndOnBlocklist(DmarcResult spfResult, DmarcResult dkimResult, int blocklistCount, string expectedProvider)
        {
            string hostProvider = "hostProvider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(spfResult, dkimResult, blockListCount: blocklistCount, hostProvider: hostProvider);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();
            Assert.AreEqual(expectedProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldBeOverridenWhenArcIsTrue()
        {
            string hostProvider = "mail.host.provider";
            string provider = "ARC-Forwarded";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(hostProvider: hostProvider, arc: true);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(provider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        private AggregateReportRecordEnriched CreateTestRecord(DmarcResult? spfResult = DmarcResult.pass,
            DmarcResult? dkimResult = DmarcResult.pass, Policy? disposition = Policy.none, int count = 0,
            string headerFrom = "digital.ncsc.gov.uk", string organisationDomain = "ncsc.gov.uk",
            int blockListCount = 0, string hostProvider = "hostProvider", bool arc = false)
        {
            return new AggregateReportRecordEnriched("id", "1", "correlationId", "causationId", "orgName", "reportId",
                DateTime.MaxValue, "domain", Alignment.r, Alignment.s, Policy.none, Policy.none, 1, "fo", "sourceIp",
                count, disposition, dkimResult, spfResult, "envelopeTo", "envelopeFrom", headerFrom,  organisationDomain, new List<string>(){$"{headerFrom}:{spfResult}"},1, 1, new List<string>(){$"{headerFrom}:{dkimResult}"}, 1, 1,
                false, false, false, false, false, arc, false, "hostName", "hostOrganisationDomain", hostProvider, 1, "asDescription", "country",
                blockListCount, blockListCount, blockListCount, blockListCount, blockListCount, blockListCount,
                blockListCount, blockListCount);
        }

        private long TallyAllCounts(DomainDateProviderRecord record)
        {
            return record.SpfPassDkimPassNone + record.SpfPassDkimFailNone + record.SpfFailDkimPassNone + record.SpfFailDkimFailNone +
                   record.SpfPassDkimPassQuarantine + record.SpfPassDkimFailQuarantine + record.SpfFailDkimPassQuarantine + record.SpfFailDkimFailQuarantine +
                   record.SpfPassDkimPassReject + record.SpfPassDkimFailReject + record.SpfFailDkimPassReject + record.SpfFailDkimFailReject;
        }
    }
}