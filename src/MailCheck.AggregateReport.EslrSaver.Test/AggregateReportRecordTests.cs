using System;
using System.Collections.Generic;
using MailCheck.AggregateReport.Contracts;
using NUnit.Framework;

namespace MailCheck.AggregateReport.EslrSaver.Test
{
    [TestFixture]
    public class AggregateReportRecordTests
    {
        [TestCase(DmarcResult.fail, DmarcResult.fail, 1, "Blocklisted", "mail.host.provider")]
        [TestCase(DmarcResult.fail, DmarcResult.fail, 0, "mail.host.provider", "mail.host.provider")]
        [TestCase(DmarcResult.fail, DmarcResult.pass, 1, "mail.host.provider", "mail.host.provider")]
        [TestCase(DmarcResult.fail, DmarcResult.pass, 0, "mail.host.provider", "mail.host.provider")]
        [TestCase(DmarcResult.pass, DmarcResult.fail, 1, "mail.host.provider", "mail.host.provider")]
        [TestCase(DmarcResult.pass, DmarcResult.fail, 0, "mail.host.provider", "mail.host.provider")]
        [TestCase(DmarcResult.pass, DmarcResult.pass, 1, "mail.host.provider", "mail.host.provider")]
        [TestCase(DmarcResult.pass, DmarcResult.pass, 0, "mail.host.provider", "mail.host.provider")]
        public void ProviderShouldBeOverridenWhenSpfandDkimFailAndOnBlocklist(DmarcResult spfResult,
            DmarcResult dkimResult, int blocklistCount, string expectedProvider, string originalProvider)
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched =
                CreateTestRecord(spfResult, dkimResult, blockListCount: blocklistCount);
            EslrSaverRecord result = aggregateReportRecordEnriched.ToEslrSaverRecord();
            Assert.AreEqual(expectedProvider, result.Provider);
            Assert.AreEqual(originalProvider, result.OriginalProvider);
        }

        [Test]
        public void ProviderShouldBeOverridenWhenArcIsTrue()
        {
            string hostProvider = "mail.host.provider";
            string provider = "ARC-Forwarded";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(hostProvider: hostProvider, arc: true);
            EslrSaverRecord result = aggregateReportRecordEnriched.ToEslrSaverRecord();

            Assert.AreEqual(provider, result.Provider);
            Assert.AreEqual(hostProvider, result.OriginalProvider);
        }

        private AggregateReportRecordEnriched CreateTestRecord(DmarcResult? spfResult = DmarcResult.pass,
            DmarcResult? dkimResult = DmarcResult.pass, Policy? disposition = Policy.none, int count = 0,
            string headerFrom = "digital.ncsc.gov.uk", string organisationDomain = "ncsc.gov.uk",
            int blockListCount = 0, string hostSourceIp = "192.168.1.1", string hostname = "hostname.org",
            string hostProvider = "mail.host.provider", bool arc = false)
        {
            return new AggregateReportRecordEnriched("id", "1", "correlationId", "causationId", "orgName", "reportId",
                DateTime.MaxValue, "domain", Alignment.r, Alignment.s, Policy.none, Policy.none, 1, "fo", hostSourceIp,
                count, disposition, dkimResult, spfResult, "envelopeTo", "envelopeFrom", headerFrom, organisationDomain,
                new List<string>() {$"{headerFrom}:{spfResult}"}, 1, 1,
                new List<string>() {$"{headerFrom}:{dkimResult}"}, 1, 1,
                false, false, false, false, false, arc, false,
                hostname, "hostOrganisationDomain", hostProvider, 1,
                "asDescription", "country", blockListCount, blockListCount,
                blockListCount, blockListCount, blockListCount, blockListCount,
                blockListCount, blockListCount);
        }
    }
}