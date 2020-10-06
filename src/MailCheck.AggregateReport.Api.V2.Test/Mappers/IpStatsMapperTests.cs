using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;
using MailCheck.AggregateReport.Api.V2.Mappers;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Mappers
{
    [TestFixture]
    public class IpStatsMapperTests
    {
        private IpStatsMapper _ipStatsMapper;

        [SetUp]
        public void SetUp()
        {
            _ipStatsMapper = new IpStatsMapper();
        }

        [Test]
        public void SubdomainStatsMapperShouldMapCorrectly()
        {
            IpStats ipStat = new IpStats("ncsc.gov.uk", "testProvider", "192.168.72.11", "hostName", 0, 1, 2, 
                3, 4, 5, 6, 7,
                8, 9, 10, 11, 30, 31, 32, 33, 34, 35, 100, 36, 37, 38, 39, 12, 
                13, 14, 15, 16, 17, 
                18, 19, 20, 21, 22, 23,
                24, 25, 26, 27, 28);
            IpStatsDto result = _ipStatsMapper.Map(ipStat, "testAlias", "testMarkdown");
            
            Assert.That(result.Domain, Is.EqualTo("ncsc.gov.uk"));
            Assert.That(result.Provider, Is.EqualTo("testProvider"));
            Assert.That(result.Ip, Is.EqualTo("192.168.72.11"));
            Assert.That(result.Hostname, Is.EqualTo("hostName"));
            Assert.That(result.SpfPassDkimPassNone, Is.EqualTo(0));
            Assert.That(result.SpfPassDkimFailNone, Is.EqualTo(1));
            Assert.That(result.SpfFailDkimPassNone, Is.EqualTo(2));
            Assert.That(result.SpfFailDkimFailNone, Is.EqualTo(3));
            Assert.That(result.SpfPassDkimPassQuarantine, Is.EqualTo(4));
            Assert.That(result.SpfPassDkimFailQuarantine, Is.EqualTo(5));
            Assert.That(result.SpfFailDkimPassQuarantine, Is.EqualTo(6));
            Assert.That(result.SpfFailDkimFailQuarantine, Is.EqualTo(7));
            Assert.That(result.SpfPassDkimPassReject, Is.EqualTo(8));
            Assert.That(result.SpfPassDkimFailReject, Is.EqualTo(9));
            Assert.That(result.SpfFailDkimPassReject, Is.EqualTo(10));
            Assert.That(result.SpfFailDkimFailReject, Is.EqualTo(11));
            Assert.That(result.SpfMisaligned, Is.EqualTo(12));
            Assert.That(result.DkimMisaligned, Is.EqualTo(13));
            Assert.That(result.ProxyBlockListCount, Is.EqualTo(14));
            Assert.That(result.SuspiciousNetworkBlockListCount, Is.EqualTo(15));
            Assert.That(result.HijackedetworkBlockListCount, Is.EqualTo(16));
            Assert.That(result.EndUserNetworkBlockListCount, Is.EqualTo(17));
            Assert.That(result.SpamSourceBlockListCount, Is.EqualTo(18));
            Assert.That(result.MalwareBlockListCount, Is.EqualTo(19));
            Assert.That(result.EndUserBlockListCount, Is.EqualTo(20));
            Assert.That(result.BounceReflectorBlockListCount, Is.EqualTo(21));
            Assert.That(result.Forwarded, Is.EqualTo(22));
            Assert.That(result.SampledOut, Is.EqualTo(23));
            Assert.That(result.TrustedForwarder, Is.EqualTo(24));
            Assert.That(result.MailingList, Is.EqualTo(25));
            Assert.That(result.LocalPolicy, Is.EqualTo(26));
            Assert.That(result.Arc, Is.EqualTo(27));
            Assert.That(result.OtherOverrideReason, Is.EqualTo(28));

            Assert.That(result.FullyTrusted, Is.EqualTo(30));
            Assert.That(result.PartiallyTrusted, Is.EqualTo(31));
            Assert.That(result.Untrusted, Is.EqualTo(32));
            Assert.That(result.Quarantined, Is.EqualTo(33));
            Assert.That(result.Rejected, Is.EqualTo(34));
            Assert.That(result.Delivered, Is.EqualTo(35));
            Assert.That(result.PassSpfTotal, Is.EqualTo(36));
            Assert.That(result.PassDkimTotal, Is.EqualTo(37));
            Assert.That(result.FailSpfTotal, Is.EqualTo(38));
            Assert.That(result.FailDkimTotal, Is.EqualTo(39));

            Assert.That(result.TotalEmails, Is.EqualTo(100));

            Assert.That(result.ProviderAlias, Is.EqualTo("testAlias"));
            Assert.That(result.ProviderMarkdown, Is.EqualTo("testMarkdown"));
        }
    }
}