using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;
using MailCheck.AggregateReport.Api.V2.Mappers;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Mappers
{
    [TestFixture]
    public class SpfDomainStatsMapperTests
    {
        private SpfDomainStatsMapper _spfDomainStatsMapper;

        [SetUp]
        public void SetUp()
        {
            _spfDomainStatsMapper = new SpfDomainStatsMapper();
        }

        [Test]
        public void SpfDomainStatsMapperShouldMapCorrectly()
        {
            SpfDomainStats spfDomainStat = new SpfDomainStats("ncsc.gov.uk", "testProvider", "192.168.72.11", "ncsc.gov.uk", 5, 10);
            SpfDomainStatsDto result = _spfDomainStatsMapper.Map(spfDomainStat, "testAlias", "testMarkdown");
            
            Assert.That(result.Domain, Is.EqualTo("ncsc.gov.uk"));
            Assert.That(result.Provider, Is.EqualTo("testProvider"));
            Assert.That(result.Ip, Is.EqualTo("192.168.72.11"));
            Assert.That(result.SpfDomain, Is.EqualTo("ncsc.gov.uk"));
            Assert.That(result.SpfPass, Is.EqualTo(5));
            Assert.That(result.SpfFail, Is.EqualTo(10));
            Assert.That(result.ProviderAlias, Is.EqualTo("testAlias"));
            Assert.That(result.ProviderMarkdown, Is.EqualTo("testMarkdown"));
        }
    }
}