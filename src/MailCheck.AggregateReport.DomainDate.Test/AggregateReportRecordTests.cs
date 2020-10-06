using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;
using NUnit.Framework;

namespace MailCheck.AggregateReport.DomainDate.Test
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
            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(spfResult, dkimResult, disposition, 10000, "digital.ncsc.gov.uk", new DateTime(2001,01,01));

            DomainDateRecord result = aggregateReportRecordEnriched.ToDomainDateRecord();

            Assert.AreEqual(new DateTime(2001, 01, 01), result.Date);
            Assert.AreEqual("digital.ncsc.gov.uk", result.Domain);
            Assert.AreEqual(10000, typeof(DomainDateRecord).GetProperty(aggregationField)?.GetValue(result, null));
        }

        private AggregateReportRecordEnriched CreateTestRecord(DmarcResult? spfResult, DmarcResult? dkimResult,
            Policy? disposition, int count, string headerFrom, DateTime date)
        {
            return new AggregateReportRecordEnriched("id", "1", "correlationId", "causationId", "orgName", "reportId",
                date, "domain", Alignment.r, Alignment.s, Policy.none, Policy.none, 1, "fo", "sourceIp", count,
                disposition, dkimResult, spfResult, "envelopeTo", "envelopeFrom", headerFrom, "organisationDomain", new List<string>(){$"{headerFrom}:{spfResult}"},1, 1, new List<string>(){$"{headerFrom}:{dkimResult}"},1, 1, true, true,
                true, true, true, true, true, "hostName", "hostOrganisationDomain", "hostProvider", 1, "asDescription", "country", 0, 0, 0, 0, 0,
                0, 0, 0);
        }
    }
}