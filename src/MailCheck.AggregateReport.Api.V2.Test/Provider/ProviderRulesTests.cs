using System;
using MailCheck.AggregateReport.Api.V2.Provider;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Test.Provider
{
    public class ProviderRulesTests
    {
        private ProviderDetailsProvider _providerDetailsProvider;

        [SetUp]
        public void SetUp()
        {
            _providerDetailsProvider = new ProviderDetailsProvider();
        }
        
        [Test]
        public void ProviderAliasAndMarkdownShouldMapToThreeDictionaries()
        {
            Assert.That(_providerDetailsProvider.ProviderAlias["testProvider"], Is.EqualTo("testAlias"));
            Assert.That(_providerDetailsProvider.AliasProvider["testAlias"], Is.EqualTo("testProvider"));
            Assert.That(_providerDetailsProvider.ProviderMarkdown["testProvider"], Is.EqualTo("testMarkdown"));
        }

        [Test]
        public void GetProviderMarkdownShouldReturnMarkdown()
        {
            string markdown = _providerDetailsProvider.GetProviderMarkdown("testProvider");
            string expected = $"testMarkdown{Environment.NewLine}***{Environment.NewLine}### Did this help?{Environment.NewLine}Email [mailcheck@digital.ncsc.gov.uk](mailto:mailcheck@digital.ncsc.gov.uk) if you note any errors or have any tips you would like to share.";
            Assert.That(markdown, Is.EqualTo(expected));
        }
        
        [Test]
        public void GetProviderAliasOutShouldReturnAlias()
        {
            string alias = _providerDetailsProvider.GetProviderAliasOut("testProvider");
            
            Assert.That(alias, Is.EqualTo("testAlias"));
        }
        
        [Test]
        public void GetProviderAliasInShouldReturnProvider()
        {
            string provider = _providerDetailsProvider.GetProviderAliasIn("testAlias");
            
            Assert.That(provider, Is.EqualTo("testProvider"));
        }
}
}