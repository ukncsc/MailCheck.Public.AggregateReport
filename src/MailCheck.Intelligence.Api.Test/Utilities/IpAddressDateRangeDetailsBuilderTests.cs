using System;
using System.Collections.Generic;
using Amazon.SimpleSystemsManagement.Model;
using MailCheck.Intelligence.Api.Domain;
using NUnit.Framework;
using MailCheck.Intelligence.Api.Utilities;

namespace MailCheck.Intelligence.Api.Test.Utilities
{
    [TestFixture]
    public class IpAddressDateRangeDetailsBuilderTests
    {
        private IpAddressDateRangeDetailsBuilder _ipAddressDateRangeDetailsBuilder;
        private IAsInfoComparer _asInfoComparer;
        private IBlocklistDetailsComparer _blocklistDetailsComparer;
        private IReverseDnsDetailComparer _reverseDnsDetailComparer;

        [SetUp]
        public void SetUp()
        {
            _asInfoComparer = new AsInfoComparer();
            _blocklistDetailsComparer = new BlocklistDetailsComparer();
            _reverseDnsDetailComparer = new ReverseDnsDetailComparer();
            _ipAddressDateRangeDetailsBuilder = new IpAddressDateRangeDetailsBuilder(_asInfoComparer, _blocklistDetailsComparer, _reverseDnsDetailComparer);
        }

        [Test]
        public void ShouldReturnResultWithIpAddress()
        {
            string ipAddress = "testIpAddress";

            _ipAddressDateRangeDetailsBuilder.SetIpAddress(ipAddress);

            IpAddressDateRangeDetails result = _ipAddressDateRangeDetailsBuilder.GetDetails();

            Assert.IsInstanceOf(typeof(IpAddressDateRangeDetails), result);
            Assert.AreSame(ipAddress, result.IpAddress);
        }

        [Test]
        public void ShouldExtendAsInfoDateRangeIfNearestDaysDetailsAreEqual()
        {
            string description = "testDescription";
            int asNumber = 123;
            string countryCode = "testCC";
            DateTime date = DateTime.Today;

            _ipAddressDateRangeDetailsBuilder.AddAsInfo(date, asNumber, description, countryCode);
            _ipAddressDateRangeDetailsBuilder.AddAsInfo(date.AddDays(1), asNumber, description, countryCode);
            _ipAddressDateRangeDetailsBuilder.AddAsInfo(date.AddDays(4), asNumber, description, countryCode);
            IpAddressDateRangeDetails result = _ipAddressDateRangeDetailsBuilder.GetDetails();

            Assert.AreEqual(1, result.AsInfo.Count);
            Assert.AreEqual(date, result.AsInfo[0].StartDate);
            Assert.AreEqual(date.AddDays(4), result.AsInfo[0].EndDate);
            Assert.AreEqual(asNumber, result.AsInfo[0].AsNumber);
            Assert.AreEqual(description, result.AsInfo[0].Description);
            Assert.AreEqual(countryCode, result.AsInfo[0].CountryCode);
        }

        [Test]
        public void ShouldExtendBlockListInfoDateRangeIfNearestDaysDetailsAreEqual()
        {
            string source = "testSource";
            DateTime date = DateTime.Today;
            List<Flag> flags = new List<Flag>();
            flags.Add(new Flag("flagName", "flagDescription"));

            _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date, source, flags);
            _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date.AddDays(3), source, flags);
            _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date.AddDays(4), source, flags);
            IpAddressDateRangeDetails result = _ipAddressDateRangeDetailsBuilder.GetDetails();

            Assert.AreEqual(1, result.BlockListDetails.Count);
            Assert.AreEqual(source, result.BlockListDetails[0].Source);
            Assert.AreEqual(date, result.BlockListDetails[0].StartDate);
            Assert.AreEqual(date.AddDays(4), result.BlockListDetails[0].EndDate);
            Assert.AreEqual(1, result.BlockListDetails[0].Flags.Count);
            Assert.AreEqual("flagName", result.BlockListDetails[0].Flags[0].Name);
            Assert.AreEqual("flagDescription", result.BlockListDetails[0].Flags[0].Description);
        }

        [Test]
        public void ShouldExtendReverseDnsInfoDateRangeIfNearestDaysDetailsAreEqual()
        {
            string host = "testHost";
            string orgDomain = "testOrgDomain";
            DateTime date = DateTime.Today;

            _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date, host, orgDomain, true);
            _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date.AddDays(2), host, orgDomain, true);
            _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date.AddDays(4), host, orgDomain, true);
            IpAddressDateRangeDetails result = _ipAddressDateRangeDetailsBuilder.GetDetails();

            Assert.AreEqual(1, result.ReverseDnsDetails.Count);
            Assert.AreEqual(host, result.ReverseDnsDetails[0].Host);
            Assert.AreEqual(orgDomain, result.ReverseDnsDetails[0].OrganisationalDomain);
            Assert.IsTrue(result.ReverseDnsDetails[0].ForwardMatches);
            Assert.AreEqual(date, result.ReverseDnsDetails[0].StartDate);
            Assert.AreEqual(date.AddDays(4), result.ReverseDnsDetails[0].EndDate);
        }

        [Test]
        public void ShouldCreateNewAsInfoDateRangeWhenAsInfoChanges()
        {
            int asNumber1 = 123;
            int asNumber2 = 456;
            string description1 = "testDescription1";
            string description2 = "testDescription2";
            string countryCode1 = "testCC1";
            string countryCode2 = "testCC2";
            DateTime date = DateTime.Today;

            _ipAddressDateRangeDetailsBuilder.AddAsInfo(date, asNumber1, description1, countryCode1);
            _ipAddressDateRangeDetailsBuilder.AddAsInfo(date.AddDays(1), asNumber1, description1, countryCode2);
            _ipAddressDateRangeDetailsBuilder.AddAsInfo(date.AddDays(2), asNumber1, description2, countryCode2);
            _ipAddressDateRangeDetailsBuilder.AddAsInfo(date.AddDays(3), asNumber2, description2, countryCode2);
            IpAddressDateRangeDetails result = _ipAddressDateRangeDetailsBuilder.GetDetails();

            Assert.AreEqual(4, result.AsInfo.Count);
            Assert.AreEqual(date, result.AsInfo[0].StartDate);
            Assert.AreEqual(date, result.AsInfo[0].EndDate);

            Assert.AreEqual(date.AddDays(3), result.AsInfo[3].StartDate);
            Assert.AreEqual(date.AddDays(3), result.AsInfo[3].EndDate);
        }

        [Test]
        public void ShouldCreateNewBlockListDateRangeWhenBlockListSourceOrFlagNameChanges()
        {
            DateTime date = DateTime.Today;
            List<Flag> flags1 = new List<Flag>() { new Flag("flagName", "flagDescription") };

            _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date, "source1", new List<Flag>()
            {
                new Flag("name1", "desc1")
            });
            _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date.AddDays(1), "source2", new List<Flag>()
            {
                new Flag("name1", "desc1")
            });
            _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date.AddDays(2), "source2", new List<Flag>()
            {
                new Flag("name1", "desc1"),
                new Flag("name2", "desc2")
            });
            _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date.AddDays(3), "source2", new List<Flag>()
            {
                new Flag("name2", "desc2")
            });
            _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date.AddDays(4), "source1", new List<Flag>()
            {
                new Flag("name2", "desc2")
            });
            _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date.AddDays(5), "source1", new List<Flag>()
            {
                new Flag("name1", "desc1")
            });
            IpAddressDateRangeDetails result = _ipAddressDateRangeDetailsBuilder.GetDetails();

            Assert.AreEqual(6, result.BlockListDetails.Count);
        }

        [Test]
        public void ShouldCreateNewReverseDnsDateRangeWhenHostChanges()
        {
            string host1 = "testHost1";
            string host2 = "testHost2";
            string orgDomain = "testOrgDomain";
            DateTime date = DateTime.Today;

            _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date, host1, orgDomain, true);
            _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date.AddDays(3), host2, orgDomain, true);
            IpAddressDateRangeDetails result = _ipAddressDateRangeDetailsBuilder.GetDetails();

            Assert.AreEqual(2, result.ReverseDnsDetails.Count);

            Assert.AreEqual(host1, result.ReverseDnsDetails[0].Host);
            Assert.AreEqual(orgDomain, result.ReverseDnsDetails[0].OrganisationalDomain);
            Assert.IsTrue(result.ReverseDnsDetails[0].ForwardMatches);
            Assert.AreEqual(date, result.ReverseDnsDetails[0].StartDate);
            Assert.AreEqual(date, result.ReverseDnsDetails[0].EndDate);

            Assert.AreEqual(host2, result.ReverseDnsDetails[1].Host);
            Assert.AreEqual(orgDomain, result.ReverseDnsDetails[1].OrganisationalDomain);
            Assert.IsTrue(result.ReverseDnsDetails[1].ForwardMatches);
            Assert.AreEqual(date.AddDays(3), result.ReverseDnsDetails[1].StartDate);
            Assert.AreEqual(date.AddDays(3), result.ReverseDnsDetails[1].EndDate);
        }

        [Test]
        public void ShouldCreateNewReverseDnsDateRangeWhenOrgDomainChanges()
        {
            string host = "testHost";
            string orgDomain1 = "testOrgDomain1";
            string orgDomain2 = "testOrgDomain2";
            DateTime date = DateTime.Today;

            _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date, host, orgDomain1, true);
            _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date.AddDays(3), host, orgDomain2, true);
            IpAddressDateRangeDetails result = _ipAddressDateRangeDetailsBuilder.GetDetails();

            Assert.AreEqual(2, result.ReverseDnsDetails.Count);

            Assert.AreEqual(host, result.ReverseDnsDetails[0].Host);
            Assert.AreEqual(orgDomain1, result.ReverseDnsDetails[0].OrganisationalDomain);
            Assert.IsTrue(result.ReverseDnsDetails[0].ForwardMatches);
            Assert.AreEqual(date, result.ReverseDnsDetails[0].StartDate);
            Assert.AreEqual(date, result.ReverseDnsDetails[0].EndDate);

            Assert.AreEqual(host, result.ReverseDnsDetails[1].Host);
            Assert.AreEqual(orgDomain2, result.ReverseDnsDetails[1].OrganisationalDomain);
            Assert.IsTrue(result.ReverseDnsDetails[1].ForwardMatches);
            Assert.AreEqual(date.AddDays(3), result.ReverseDnsDetails[1].StartDate);
            Assert.AreEqual(date.AddDays(3), result.ReverseDnsDetails[1].EndDate);
        }

        [Test]
        public void ShouldCreateNewReverseDnsDateRangeWhenFwdMatchChanges()
        {
            string host = "testHost";
            string orgDomain = "testOrgDomain";
            DateTime date = DateTime.Today;

            _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date, host, orgDomain, true);
            _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date.AddDays(3), host, orgDomain, false);
            IpAddressDateRangeDetails result = _ipAddressDateRangeDetailsBuilder.GetDetails();

            Assert.AreEqual(2, result.ReverseDnsDetails.Count);

            Assert.AreEqual(host, result.ReverseDnsDetails[0].Host);
            Assert.AreEqual(orgDomain, result.ReverseDnsDetails[0].OrganisationalDomain);
            Assert.IsTrue(result.ReverseDnsDetails[0].ForwardMatches);
            Assert.AreEqual(date, result.ReverseDnsDetails[0].StartDate);
            Assert.AreEqual(date, result.ReverseDnsDetails[0].EndDate);

            Assert.AreEqual(host, result.ReverseDnsDetails[1].Host);
            Assert.AreEqual(orgDomain, result.ReverseDnsDetails[1].OrganisationalDomain);
            Assert.IsFalse(result.ReverseDnsDetails[1].ForwardMatches);
            Assert.AreEqual(date.AddDays(3), result.ReverseDnsDetails[1].StartDate);
            Assert.AreEqual(date.AddDays(3), result.ReverseDnsDetails[1].EndDate);
        }
    }
}