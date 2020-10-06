using System;
using System.Collections.Generic;
using MailCheck.AggregateReport.Contracts;
using NUnit.Framework;

namespace MailCheck.AggregateReport.RollupDomainDate.Test
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
            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(spfResult, dkimResult, disposition, 10000, date: new DateTime(2001, 01, 01));

            List<DomainDateRecord> result = aggregateReportRecordEnriched.ToDomainDateRecords();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(new DateTime(2001, 01, 01), result[0].Date);
            Assert.AreEqual("digital.ncsc.gov.uk", result[0].Domain);
            Assert.AreEqual(10000, TallyAllCounts(result[0]));
            Assert.AreEqual(10000, typeof(DomainDateRecord).GetProperty(aggregationField)?.GetValue(result[0], null));

            Assert.AreEqual(new DateTime(2001, 01, 01), result[1].Date);
            Assert.AreEqual("ncsc.gov.uk", result[1].Domain);
            Assert.AreEqual(10000, TallyAllCounts(result[1]));
            Assert.AreEqual(10000, typeof(DomainDateRecord).GetProperty(aggregationField)?.GetValue(result[1], null));
        }
  
        [Test]
        public void ConvertingToRecordShouldSplitIntoSubdomains()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(headerFrom: "a.b.c.d.e.gov.uk", organisationDomain: "e.gov.uk", date: new DateTime(2001, 01, 01));

            List<DomainDateRecord> result = aggregateReportRecordEnriched.ToDomainDateRecords();

            Assert.AreEqual(5, result.Count);

            Assert.AreEqual(new DateTime(2001, 01, 01), result[0].Date);
            Assert.AreEqual(new DateTime(2001, 01, 01), result[1].Date);
            Assert.AreEqual(new DateTime(2001, 01, 01), result[2].Date);
            Assert.AreEqual(new DateTime(2001, 01, 01), result[3].Date);
            Assert.AreEqual(new DateTime(2001, 01, 01), result[4].Date);

            Assert.AreEqual("a.b.c.d.e.gov.uk", result[0].Domain);
            Assert.AreEqual("b.c.d.e.gov.uk", result[1].Domain);
            Assert.AreEqual("c.d.e.gov.uk", result[2].Domain);
            Assert.AreEqual("d.e.gov.uk", result[3].Domain);
            Assert.AreEqual("e.gov.uk", result[4].Domain);
        }

        private AggregateReportRecordEnriched CreateTestRecord(DmarcResult? spfResult = DmarcResult.pass,
            DmarcResult? dkimResult = DmarcResult.pass, Policy? disposition = Policy.none, int count = 0,
            string headerFrom = "digital.ncsc.gov.uk", string organisationDomain = "ncsc.gov.uk",
            DateTime date = new DateTime())
        {
            return new AggregateReportRecordEnriched("id", "1", "correlationId", "causationId", "orgName", "reportId",
                date, "domain", Alignment.r, Alignment.s, Policy.none, Policy.none, 1, "fo", "sourceIp", count,
                disposition, dkimResult, spfResult, "envelopeTo", "envelopeFrom", headerFrom, organisationDomain, new List<string>(){$"{headerFrom}:{spfResult}"}, 1, 1, new List<string>(){$"{headerFrom}:{dkimResult}"},
                1, 1, true, true,
                true, true, true, true, true, "hostName", "hostOrganisationDomain", "hostProvider", 1, "asDescription", "country", 0, 0, 0, 0,
                0, 0,
                0, 0);
        }

        private long TallyAllCounts(DomainDateRecord record)
        {
            return record.SpfPassDkimPassNone + record.SpfPassDkimFailNone + record.SpfFailDkimPassNone + record.SpfFailDkimFailNone +
                   record.SpfPassDkimPassQuarantine + record.SpfPassDkimFailQuarantine + record.SpfFailDkimPassQuarantine + record.SpfFailDkimFailQuarantine +
                   record.SpfPassDkimPassReject + record.SpfPassDkimFailReject + record.SpfFailDkimPassReject + record.SpfFailDkimFailReject;
        }
    }
}