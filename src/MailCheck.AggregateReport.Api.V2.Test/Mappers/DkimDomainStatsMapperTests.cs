using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;
using MailCheck.AggregateReport.Api.V2.Mappers;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Mappers
{
    [TestFixture]
    public class DkimDomainStatsMapperTests
    {
        private DkimDomainStatsMapper _dkimDomainStatsMapper;

        [SetUp]
        public void SetUp()
        {
            _dkimDomainStatsMapper = new DkimDomainStatsMapper();
        }

        [Test]
        public void DkimDomainStatsMapperShouldMapCorrectly()
        {
            DkimDomainStats dkimDomainStat = new DkimDomainStats("ncsc.gov.uk", "testProvider", "192.168.72.11", 
                "ncsc.gov.uk", "selector", 5, 10);
            DkimDomainStatsDto result = _dkimDomainStatsMapper.Map(dkimDomainStat, "testAlias", "testMarkdown");
            
            Assert.That(result.Domain, Is.EqualTo("ncsc.gov.uk"));
            Assert.That(result.Provider, Is.EqualTo("testProvider"));
            Assert.That(result.Ip, Is.EqualTo("192.168.72.11"));
            Assert.That(result.DkimDomain, Is.EqualTo("ncsc.gov.uk"));
            Assert.That(result.DkimSelector, Is.EqualTo("selector"));
            Assert.That(result.DkimPass, Is.EqualTo(5));
            Assert.That(result.DkimFail, Is.EqualTo(10));
            Assert.That(result.ProviderAlias, Is.EqualTo("testAlias"));
            Assert.That(result.ProviderMarkdown, Is.EqualTo("testMarkdown"));
        }
    }
}