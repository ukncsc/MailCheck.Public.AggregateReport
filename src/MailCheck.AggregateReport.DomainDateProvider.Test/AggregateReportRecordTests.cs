﻿using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;
using NUnit.Framework;

namespace MailCheck.AggregateReport.DomainDateProvider.Test
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

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("hostProvider", result[0].Provider);
            Assert.AreEqual("digital.ncsc.gov.uk", result[0].Domain);
            Assert.AreEqual(10000, TallyAllCounts(result[0]));
            Assert.AreEqual(10000, typeof(DomainDateProviderRecord).GetProperty(aggregationField)?.GetValue(result[0], null));

            Assert.AreEqual("All Providers", result[1].Provider);
            Assert.AreEqual("digital.ncsc.gov.uk", result[1].Domain);
            Assert.AreEqual(10000, TallyAllCounts(result[1]));
            Assert.AreEqual(10000, typeof(DomainDateProviderRecord).GetProperty(aggregationField)?.GetValue(result[1], null));
        }

        [Test]
        public void ConvertingToRecordsShouldCreateAllProvidersRecord()
        {
            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(headerFrom: "a.b.c.d.e.gov.uk", organisationDomain: "e.gov.uk");

            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual("hostProvider", result[0].Provider);
            Assert.AreEqual("a.b.c.d.e.gov.uk", result[0].Domain);

            Assert.AreEqual("All Providers", result[1].Provider);
            Assert.AreEqual("a.b.c.d.e.gov.uk", result[1].Domain);
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
            var result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();
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

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameUnknownAndDmarcPasses()
        {
            string hostName = "Unknown";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(hostProvider: hostProvider, hostName: hostName);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldBeOverridenWhenHostNameUnknownAndDmarcFails()
        {
            string hostName = "Unknown";
            string hostProvider = "mail.host.provider";
            string provider = "ReverseDnsFail";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(spfResult: DmarcResult.fail, dkimResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(provider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameMismatchedAndDmarcPasses()
        {
            string hostName = "Mismatch";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(hostProvider: hostProvider, hostName: hostName);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldBeOverridenWhenHostNameMismatchedAndDmarcFails()
        {
            string hostName = "Mismatch";
            string hostProvider = "mail.host.provider";
            string provider = "ReverseDnsFail";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(spfResult: DmarcResult.fail, dkimResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(provider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameMismatchedAndSpfPasses()
        {
            string hostName = "Mismatch";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(dkimResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameMismatchedAndDkimPasses()
        {
            string hostName = "Mismatch";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(spfResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameUnknownAndDkimPasses()
        {
            string hostName = "Unknown";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(spfResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        [Test]
        public void ProviderShouldNotBeOverridenWhenHostNameUnknownAndSpfPasses()
        {
            string hostName = "Unknown";
            string hostProvider = "mail.host.provider";

            AggregateReportRecordEnriched aggregateReportRecordEnriched = CreateTestRecord(dkimResult: DmarcResult.fail, hostProvider: hostProvider, hostName: hostName);
            List<DomainDateProviderRecord> result = aggregateReportRecordEnriched.ToDomainDateProviderRecord();

            Assert.AreEqual(hostProvider, result[0].Provider);
            Assert.AreEqual(hostProvider, result[0].OriginalProvider);
        }

        private AggregateReportRecordEnriched CreateTestRecord(DmarcResult? spfResult = DmarcResult.pass,
            DmarcResult? dkimResult = DmarcResult.pass, Policy? disposition = Policy.none, int count = 0,
            string headerFrom = "digital.ncsc.gov.uk", string organisationDomain = "ncsc.gov.uk",
            int blockListCount = 0, bool arc = false, string hostName = "hostName", string hostProvider = "hostProvider")
        {
            return new AggregateReportRecordEnriched("id", "1", "correlationId", "causationId", "orgName", "reportId",
                DateTime.MaxValue, "domain", Alignment.r, Alignment.s, Policy.none, Policy.none, 1, "fo", "sourceIp",
                count, disposition, dkimResult, spfResult, "envelopeTo", "envelopeFrom", headerFrom, organisationDomain,
                new List<string>(){$"{headerFrom}:{spfResult}"}, 1, 1, new List<string>(){$"{headerFrom}:{dkimResult}"}, 1, 1, false, false, false, false, false, arc, false, hostName, "hostOrganisationDomain", hostProvider, 1,
                "asDescription", "country", blockListCount, blockListCount, blockListCount, blockListCount,
                blockListCount, blockListCount, blockListCount, blockListCount);
        }

        private long TallyAllCounts(DomainDateProviderRecord record)
        {
            return record.SpfPassDkimPassNone + record.SpfPassDkimFailNone + record.SpfFailDkimPassNone + record.SpfFailDkimFailNone +
                   record.SpfPassDkimPassQuarantine + record.SpfPassDkimFailQuarantine + record.SpfFailDkimPassQuarantine + record.SpfFailDkimFailQuarantine +
                   record.SpfPassDkimPassReject + record.SpfPassDkimFailReject + record.SpfFailDkimPassReject + record.SpfFailDkimFailReject;
        }
    }
}