using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy.Sdk;
using MailCheck.AggregateReport.Contracts;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Host;
using NUnit.Framework;

namespace MailCheck.AggregateReport.DomDatProIpSpf.Test
{
    [TestFixture]
    public class AggregateReportRecordTests
    {
        [Test]
        public void SpfPassShouldSConvertCorrectly()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched =
                CreateTestRecord(new List<string>() {"domain1:pass"}, count: 10);

            List<DomDatProIpSpfRecord>
                result = aggregateReportRecordEnriched.ToDomDatProIpSpfRecord();
            
            Assert.AreEqual("domain1", result[0].SpfDomain);
            Assert.AreEqual(10, result[0].SpfPass);
            Assert.AreEqual(0, result[0].SpfFail);
        }
        
        [Test]
        public void SpfFailShouldSConvertCorrectly()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched =
                CreateTestRecord(new List<string>() {"domain11:fail"}, count: 12);

            List<DomDatProIpSpfRecord>
                result = aggregateReportRecordEnriched.ToDomDatProIpSpfRecord();
            
            Assert.AreEqual("domain11", result[0].SpfDomain);
            Assert.AreEqual(0, result[0].SpfPass);
            Assert.AreEqual(12, result[0].SpfFail);
        }
        
        [Test]
        public void SpfPassAndFailShouldSConvertCorrectly()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched =
                CreateTestRecord(new List<string>() {"domain1:pass", "domain2:fail"}, count:13);

            List<DomDatProIpSpfRecord>
                result = aggregateReportRecordEnriched.ToDomDatProIpSpfRecord();
            
            Assert.AreEqual("domain1", result[0].SpfDomain);
            Assert.AreEqual(13, result[0].SpfPass);
            Assert.AreEqual(0, result[0].SpfFail);
            
            Assert.AreEqual("domain2", result[1].SpfDomain);
            Assert.AreEqual(0, result[1].SpfPass);
            Assert.AreEqual(13, result[1].SpfFail);
        }

        [Test]
        public void ConvertingToRecordsShouldCreateAllProvidersRecord()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string>(){"domain1:pass"});

            List<DomDatProIpSpfRecord> result = aggregateReportRecordEnriched.ToDomDatProIpSpfRecord();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual("mail.host.provider", result[0].Provider);
            Assert.AreEqual("domain1", result[0].SpfDomain);

            Assert.AreEqual("All Providers", result[1].Provider);
            Assert.AreEqual("domain1", result[1].SpfDomain);
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
            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(new List<string> { "domain1:pass" }, spfResult, dkimResult, blockListCount: blocklistCount);
            var result = aggregateReportRecordEnriched.ToDomDatProIpSpfRecord();
            Assert.AreEqual(expectedProvider, result[0].Provider);
        }

        private AggregateReportRecordEnriched CreateTestRecord(List<string> spfAuthResults, DmarcResult? spfResult = DmarcResult.pass,
            DmarcResult? dkimResult = DmarcResult.pass, Policy? disposition = Policy.none, int count = 0,
            string headerFrom = "digital.ncsc.gov.uk", string organisationDomain = "ncsc.gov.uk",
            int blockListCount = 0, string hostSourceIp = "192.168.1.1", string hostProvider = "mail.host.provider")
        {
            return new AggregateReportRecordEnriched("id", "1", "correlationId", "causationId", "orgName", "reportId",
                DateTime.MaxValue, "domain", Alignment.r, Alignment.s, Policy.none, Policy.none, 1, "fo", hostSourceIp,
                count, disposition, dkimResult, spfResult, "envelopeTo", "envelopeFrom", headerFrom, organisationDomain,
                spfAuthResults, 1, 1, new List<string>(){$"{headerFrom}:{dkimResult}"}, 1, 1, true, true, true, true, true, true, true, "hostName", "hostOrganisationDomain", hostProvider, 1,
                "asDescription", "country", blockListCount, blockListCount, blockListCount, blockListCount,
                blockListCount, blockListCount, blockListCount, blockListCount);
        }

        private long TallyAllCounts(DomDatProIpSpfRecord record)
        {
            return record.SpfPass + record.SpfFail;
        }

    }
}