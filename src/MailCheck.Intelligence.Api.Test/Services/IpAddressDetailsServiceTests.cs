using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Api.Dao;
using MailCheck.Intelligence.Api.Domain;
using MailCheck.Intelligence.Api.Services;
using MailCheck.Intelligence.Api.Utilities;
using NUnit.Framework;

namespace MailCheck.Intelligence.Api.Test.Services
{
    [TestFixture]
    public class IpAddressDetailsServiceTests
    {
        IpAddressDetailsService _ipAddressDetailsService;
        IIpAddressDetailsApiDao _ipAddressDetailsApiDao;
        IIpAddressDateRangeDetailsBuilder _ipAddressDateRangeDetailsBuilder;

        [SetUp]
        public void SetUp()
        {
            _ipAddressDetailsApiDao = A.Fake<IIpAddressDetailsApiDao>();
            _ipAddressDateRangeDetailsBuilder = A.Fake<IIpAddressDateRangeDetailsBuilder>();

            _ipAddressDetailsService = new IpAddressDetailsService(_ipAddressDetailsApiDao, _ipAddressDateRangeDetailsBuilder);
        }

        [Test]
        public async Task ShouldBuildWithIpAddressWhenGettingIpAddressDetails()
        {
            string ipAddress = "testIpAddress";
            List<IpAddressDetails> resultFromDao = GetDefaultDaoResult(ipAddress);

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            A.CallTo(() => _ipAddressDetailsApiDao.GetIpAddressDetails(ipAddress, startDate, endDate)).Returns(resultFromDao);

            await _ipAddressDetailsService.GetIpAddressDetails(ipAddress, startDate, endDate);

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.SetIpAddress(ipAddress))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.SetIpAddress(A<string>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ShouldBuildWithBlockListWhenGettingIpAddressDetails()
        {
            string ipAddress = "testIpAddress";
            DateTime date = DateTime.UnixEpoch;
            List<IpAddressDetails> resultFromDao = GetDefaultDaoResult(ipAddress, date);

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            A.CallTo(() => _ipAddressDetailsApiDao.GetIpAddressDetails(ipAddress, startDate, endDate)).Returns(resultFromDao);

            await _ipAddressDetailsService.GetIpAddressDetails(ipAddress, startDate, endDate);

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date, "source1",
                    A<List<Flag>>.That.Matches(x =>
                        x.Exists(y => y.Name == "flag1a" && y.Description == "description1a") &&
                        x.Exists(y => y.Name == "flag1b" && y.Description == "description1b"))))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(date, "source2",
                    A<List<Flag>>.That.Matches(x =>
                        x.Exists(y => y.Name == "flag2a" && y.Description == "description2a"))))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(A<DateTime>._, A<string>._, A<List<Flag>>._))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        public async Task ShouldBuildWithAsInfoWhenGettingIpAddressDetails()
        {
            string ipAddress = "testIpAddress";
            DateTime date = DateTime.UnixEpoch;
            int asNumber = 1;
            string description = "testDescription";
            string countryCode = "testCountryCode";

            List<IpAddressDetails> resultFromDao = GetDefaultDaoResult(ipAddress, date, asNumber, description, countryCode);

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            A.CallTo(() => _ipAddressDetailsApiDao.GetIpAddressDetails(ipAddress, startDate, endDate)).Returns(resultFromDao);

            await _ipAddressDetailsService.GetIpAddressDetails(ipAddress, startDate, endDate);

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddAsInfo(date, asNumber, description, countryCode)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddAsInfo(A<DateTime>._, A<int>._, A<string>._, A<string>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ShouldBuildWithReverseDnsWhenGettingIpAddressDetailsWithForwardMatchIfAvailable()
        {
            string ipAddress = "testIpAddress";
            DateTime date = DateTime.UnixEpoch;

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            List<IpAddressDetails> resultFromDao = GetDefaultDaoResult(ipAddress, date);

            A.CallTo(() => _ipAddressDetailsApiDao.GetIpAddressDetails(ipAddress, startDate, endDate)).Returns(resultFromDao);

            await _ipAddressDetailsService.GetIpAddressDetails(ipAddress, startDate, endDate);

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date, "host1", "organisationDomain1", true)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(A<DateTime>._, A<string>._, A<string>._, A<bool>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ShouldBuildWithReverseDnsWhenGettingIpAddressDetailsWithoutForwardMatchIfNotAvailable()
        {
            string ipAddress = "testIpAddress";
            DateTime date = DateTime.UnixEpoch;

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            List<IpAddressDetails> resultFromDao = GetDefaultDaoResult(ipAddress, date);
            resultFromDao[0].ReverseDnsResponses.RemoveAt(0);

            A.CallTo(() => _ipAddressDetailsApiDao.GetIpAddressDetails(ipAddress, startDate, endDate)).Returns(resultFromDao);

            await _ipAddressDetailsService.GetIpAddressDetails(ipAddress, startDate, endDate);

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(date, "host2", "organisationDomain2", false)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(A<DateTime>._, A<string>._, A<string>._, A<bool>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ShouldReturnResultOfBuilderWhenGettingDetails()
        {
            string ipAddress = "testIpAddress";
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            List<IpAddressDetails> resultFromDao = GetDefaultDaoResult(ipAddress);

            A.CallTo(() => _ipAddressDetailsApiDao.GetIpAddressDetails(ipAddress, startDate, endDate)).Returns(resultFromDao);

            IpAddressDateRangeDetails resultFromBuilder = new IpAddressDateRangeDetails();
            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.GetDetails()).Returns(resultFromBuilder);

            IpAddressDateRangeDetails result = await _ipAddressDetailsService.GetIpAddressDetails(ipAddress, startDate, endDate);

            Assert.AreSame(resultFromBuilder, result);
        }

        [Test]
        public async Task ShouldBuildWithEachRowReturnedFromDaoWhenGettingDetails()
        {
            string ipAddress = "testIpAddress";
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            List<IpAddressDetails> resultFromDao = GetDefaultDaoResult(ipAddress);
            resultFromDao = resultFromDao.Concat(resultFromDao).ToList();

            A.CallTo(() => _ipAddressDetailsApiDao.GetIpAddressDetails(ipAddress, startDate, endDate)).Returns(resultFromDao);

            await _ipAddressDetailsService.GetIpAddressDetails(ipAddress, startDate, endDate);

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(A<DateTime>._, A<string>._, A<string>._, A<bool>._)).MustHaveHappenedTwiceExactly();

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddAsInfo(A<DateTime>._, A<int>._, A<string>._, A<string>._)).MustHaveHappenedTwiceExactly();

            A.CallTo(() => _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(A<DateTime>._, A<string>._, A<List<Flag>>._))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 4);
        }

        private List<IpAddressDetails> GetDefaultDaoResult(string ipAddress = "", DateTime? date = null, int asNumber = 0, string description = "", string countryCode = "")
        {
            List<BlocklistAppearance> blockListOccurrences = new List<BlocklistAppearance>
            {
                new BlocklistAppearance("flag1a", "source1", "description1a"),
                new BlocklistAppearance("flag1b", "source1", "description1b"),
                new BlocklistAppearance("flag2a", "source2", "description2a"),
            };
            List<ReverseDnsResponse> reverseDnsResponses = new List<ReverseDnsResponse>
            {
                new ReverseDnsResponse("host1", new List<string>{ipAddress}, "organisationDomain1"),
                new ReverseDnsResponse("host2", new List<string>{"someOtherIpAddress"}, "organisationDomain2"),
            };
            DateTime? asnLookupTimestamp = new DateTime(2000, 01, 01);
            DateTime? blocklistLookupTimestamp = new DateTime(2000, 01, 02);
            DateTime? reverseDnsLookupTimestamp = new DateTime(2000, 01, 03);

            return new List<IpAddressDetails>
            {
                new IpAddressDetails(ipAddress, date ?? DateTime.MinValue, asNumber, description,countryCode, blockListOccurrences, reverseDnsResponses, asnLookupTimestamp,blocklistLookupTimestamp, reverseDnsLookupTimestamp)
            };
        }
    }
}