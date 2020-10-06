using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;
using MailCheck.AggregateReport.Api.V2.Mappers;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Mappers
{
    [TestFixture]
    public class SubdomainStatsMapperTests
    {
        private SubdomainStatsMapper _subdomainStatsMapper;

        [SetUp]
        public void SetUp()
        {
            _subdomainStatsMapper = new SubdomainStatsMapper();
        }

        [Test]
        public void SubdomainStatsMapperShouldMapCorrectly()
        {
            SubdomainStats domainStats = new SubdomainStats("ncsc.gov.uk", "testProvider", "digital.ncsc.gov.uk", 0, 1, 2, 
                3, 4, 5, 6, 7,
                8, 9, 10, 11, 20, 21, 22);
            SubdomainStatsDto result = _subdomainStatsMapper.Map(domainStats, "testAlias", "testMarkdown");
            
            Assert.That(result.Domain, Is.EqualTo("ncsc.gov.uk"));
            Assert.That(result.Provider, Is.EqualTo("testProvider"));
            Assert.That(result.Subdomain, Is.EqualTo("digital.ncsc.gov.uk"));
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
            Assert.That(result.TotalEmails, Is.EqualTo(20));
            Assert.That(result.FailSpfTotal, Is.EqualTo(21));
            Assert.That(result.FailDkimTotal, Is.EqualTo(22));
            Assert.That(result.ProviderAlias, Is.EqualTo("testAlias"));
            Assert.That(result.ProviderMarkdown, Is.EqualTo("testMarkdown"));
        }
    }
}