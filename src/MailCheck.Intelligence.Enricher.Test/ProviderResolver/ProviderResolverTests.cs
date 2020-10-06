using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using NUnit.Framework;

namespace MailCheck.Intelligence.Enricher.Test.ProviderResolver
{
    [TestFixture]
    public class ProviderResolverTests
    {
        private Enricher.ProviderResolver.ProviderResolver _providerResolver;

        [SetUp]
        public void SetUp()
        {
            _providerResolver = new Enricher.ProviderResolver.ProviderResolver();
        }

        [TestCase("TestOrgDomain", "TestDescription", "TestOrgDomain")]
        [TestCase("Mismatch", "TestDescription", "TestDescription")]
        [TestCase("Unknown", "TestDescription", "TestDescription")]
        public void ProviderShouldBeDescriptionIfOrgDomainMismatchOrUnknown(string organisationalDomain, string description, string expectedProvider)
        {
            IpAddressDetails ipAddressDetails = CreateIpAddressDetails(organisationalDomain: organisationalDomain, description: description);

            string provider = _providerResolver.GetProvider(ipAddressDetails);

            Assert.AreEqual(expectedProvider, provider);
        }

        [TestCase("TestOrgDomain", "51.64.0.25", "Legacy Police National Network (PNN) SCN")]
        [TestCase("TestOrgDomain", "127.0.0.1", "Localhost")]
        [TestCase("TestOrgDomain", "127.0.0.2", "TestOrgDomain")]
        public void ProviderShouldBeOverriddenBasedOnIpAddressMappings(string organisationalDomain, string ipAddress, string expectedProvider)
        {
            IpAddressDetails ipAddressDetails = CreateIpAddressDetails(organisationalDomain: organisationalDomain, ipAddress: ipAddress, description: "");

            string provider = _providerResolver.GetProvider(ipAddressDetails);

            Assert.AreEqual(expectedProvider, provider);
        }

        [TestCase("TestOrgDomainThatMaps", "ProviderMappedByOrgDomain")]
        [TestCase("TestOrgDomain", "TestOrgDomain")]
        public void ProviderShouldBeOverriddenBasedOnOrgDomainMappings(string organisationalDomain, string expectedProvider)
        {
            IpAddressDetails ipAddressDetails = CreateIpAddressDetails(organisationalDomain: organisationalDomain);

            string provider = _providerResolver.GetProvider(ipAddressDetails);
            
            Assert.AreEqual(expectedProvider, provider);
        }

        [Test]
        public void ProviderShouldDefaultToDescription()
        {
            IpAddressDetails ipAddressDetails = CreateIpAddressDetails(description: "testDescription");

            string provider = _providerResolver.GetProvider(ipAddressDetails);

            Assert.AreEqual("testDescription", provider);
        }

        [Test]
        public void ProviderShouldLastResortToUnrouted()
        {
            IpAddressDetails ipAddressDetails = CreateIpAddressDetails();

            string provider = _providerResolver.GetProvider(ipAddressDetails);

            Assert.AreEqual("Unrouted", provider);
        }

        private IpAddressDetails CreateIpAddressDetails(string organisationalDomain = null, string description = null, string ipAddress = null)
        {
            return new IpAddressDetails(
                ipAddress,
                DateTime.UnixEpoch,
                null,
                description,
                "",
                Enumerable.Repeat(new BlocklistAppearance("", "", ""), 0).ToList(),
                new List<ReverseDnsResponse> { new ReverseDnsResponse("", null, organisationalDomain) },
                DateTime.UnixEpoch,
                DateTime.UnixEpoch,
                DateTime.UnixEpoch);
        }
    }
}