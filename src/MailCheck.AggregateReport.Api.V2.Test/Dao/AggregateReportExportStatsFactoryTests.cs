using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Api.V2.Dao;
using MailCheck.AggregateReport.Api.V2.Domain;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Dao
{
    [TestFixture]
    public class AggregateReportExportStatsFactoryTests
    {
        private AggregateReportExportStatsFactory _aggregateReportExportStatsFactory;
        private readonly DateTime _testDateTime = new DateTime(2000, 10, 20, 10, 30, 50);

        [SetUp]
        public void SetUp()
        {
            _aggregateReportExportStatsFactory = new AggregateReportExportStatsFactory();
        }

        [Test]
        public void CreateMapsDateTimeToIso8601()
        {
            AggregateReportExportStats result = null;
            using (DataTableReader reader = GetDataTable().CreateDataReader())
            {
                while (reader.Read())
                {
                    result = _aggregateReportExportStatsFactory.Create(reader);
                }
            }

            Assert.AreEqual("2000-10-20", result.EffectiveDate, "Effective date is mapped to ISO8601 ignoring the current culture date format");
        }

        [Test]
        public void CreateMapsAllFieldsCorrectly()
        {
            AggregateReportExportStats result = null;
            using (DataTableReader reader = GetDataTable().CreateDataReader())
            {
                while (reader.Read())
                {
                    result = _aggregateReportExportStatsFactory.Create(reader);
                }
            }

            Assert.AreEqual("2000-10-20", result.EffectiveDate);
            Assert.AreEqual("testDomain", result.Domain);
            Assert.AreEqual("testProvider", result.Provider);
            Assert.AreEqual("testOriginalProvider", result.OriginalProvider);
            Assert.AreEqual("testReporterOrgName", result.ReporterOrgName);
            Assert.AreEqual("testIp", result.Ip);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("testDisposition", result.Disposition);
            Assert.AreEqual("testDkim", result.Dkim);
            Assert.AreEqual("testSpf", result.Spf);
            Assert.AreEqual("testEnvelopeTo", result.EnvelopeTo);
            Assert.AreEqual("testEnvelopeFrom", result.EnvelopeFrom);
            Assert.AreEqual("testHeaderFrom", result.HeaderFrom);
            Assert.AreEqual("testOrganisationDomainFrom", result.OrganisationDomainFrom);
            Assert.AreEqual("testSpfAuthResults", result.SpfAuthResults);
            Assert.AreEqual(2, result.SpfPassCount);
            Assert.AreEqual(3, result.SpfFailCount);
            Assert.AreEqual("testDkimAuthResults", result.DkimAuthResults);
            Assert.AreEqual(4, result.DkimPassCount);
            Assert.AreEqual(5, result.DkimFailCount);
            Assert.AreEqual(6, result.Forwarded);
            Assert.AreEqual(7, result.SampledOut);
            Assert.AreEqual(8, result.TrustedForwarder);
            Assert.AreEqual(9, result.MailingList);
            Assert.AreEqual(10, result.LocalPolicy);
            Assert.AreEqual(11, result.Arc);
            Assert.AreEqual(12, result.OtherOverrideReason);
            Assert.AreEqual("testHostName", result.HostName);
            Assert.AreEqual("testHostOrgDomain", result.HostOrgDomain);
            Assert.AreEqual("testHostProvider", result.HostProvider);
            Assert.AreEqual(13, result.HostAsNumber);
            Assert.AreEqual("testHostAsDescription", result.HostAsDescription);
            Assert.AreEqual("testHostCountry", result.HostCountry);
            Assert.AreEqual(14, result.ProxyBlockListCount);
            Assert.AreEqual(15, result.SuspiciousNetworkBlockListCount);
            Assert.AreEqual(16, result.HijackedNetworkBlockListCount);
            Assert.AreEqual(17, result.EndUserNetworkBlockListCount);
            Assert.AreEqual(18, result.SpamSourceBlockListCount);
            Assert.AreEqual(19, result.MalwareBlockListCount);
            Assert.AreEqual(20, result.EndUserBlockListCount);
            Assert.AreEqual(21, result.BounceReflectorBlockListCount);
        }

        private DataTable GetDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("effective_date", typeof(DateTime));
            table.Columns.Add("domain", typeof(string));
            table.Columns.Add("provider", typeof(string));
            table.Columns.Add("original_provider", typeof(string));
            table.Columns.Add("reporter_org_name", typeof(string));
            table.Columns.Add("ip", typeof(string));
            table.Columns.Add("count", typeof(int));
            table.Columns.Add("disposition", typeof(string));
            table.Columns.Add("dkim", typeof(string));
            table.Columns.Add("spf", typeof(string));
            table.Columns.Add("envelope_to", typeof(string));
            table.Columns.Add("envelope_from", typeof(string));
            table.Columns.Add("header_from", typeof(string));
            table.Columns.Add("organisation_domain_from", typeof(string));
            table.Columns.Add("spf_auth_results", typeof(string));
            table.Columns.Add("spf_pass_count", typeof(int));
            table.Columns.Add("spf_fail_count", typeof(int));
            table.Columns.Add("dkim_auth_results", typeof(string));
            table.Columns.Add("dkim_pass_count", typeof(int));
            table.Columns.Add("dkim_fail_count", typeof(int));
            table.Columns.Add("forwarded", typeof(int));
            table.Columns.Add("sampled_out", typeof(int));
            table.Columns.Add("trusted_forwarder", typeof(int));
            table.Columns.Add("mailing_list", typeof(int));
            table.Columns.Add("local_policy", typeof(int));
            table.Columns.Add("arc", typeof(int));
            table.Columns.Add("other_override_reason", typeof(int));
            table.Columns.Add("host_name", typeof(string));
            table.Columns.Add("host_org_domain", typeof(string));
            table.Columns.Add("host_provider", typeof(string));
            table.Columns.Add("host_as_number", typeof(int));
            table.Columns.Add("host_as_description", typeof(string));
            table.Columns.Add("host_country", typeof(string));
            table.Columns.Add("proxy_blocklist", typeof(int));
            table.Columns.Add("suspicious_network_blocklist", typeof(int));
            table.Columns.Add("hijacked_network_blocklist", typeof(int));
            table.Columns.Add("enduser_network_blocklist", typeof(int));
            table.Columns.Add("spam_source_blocklist", typeof(int));
            table.Columns.Add("malware_blocklist", typeof(int));
            table.Columns.Add("enduser_blocklist", typeof(int));
            table.Columns.Add("bounce_reflector_blocklist", typeof(int));

            table.Rows.Add(_testDateTime, "testDomain", "testProvider", "testOriginalProvider", "testReporterOrgName", "testIp", 1, "testDisposition", "testDkim", "testSpf", "testEnvelopeTo", "testEnvelopeFrom", "testHeaderFrom", "testOrganisationDomainFrom", "testSpfAuthResults", 2, 3, "testDkimAuthResults", 4, 5, 6, 7, 8, 9, 10, 11, 12, "testHostName", "testHostOrgDomain", "testHostProvider", 13, "testHostAsDescription", "testHostCountry", 14, 15, 16, 17, 18, 19, 20, 21);

            return table;
        }
    }
}
