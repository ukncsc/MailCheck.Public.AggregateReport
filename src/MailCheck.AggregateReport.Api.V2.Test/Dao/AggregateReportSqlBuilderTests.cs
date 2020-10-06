using MailCheck.Common.Data;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Api.V2.Dao
{
    [TestFixture]
    public class AggregateReportSqlBuilderTests
    {
        private ISqlBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = new SqlBuilder()
                .AddAggregateReportDefaults();
        }

        [Test]
        public void Build_Defaults_BuildsCorrectSql() {
            var actual = _builder.Build(ExampleTokenisedSqlA);
            Assert.That(actual, Is.EqualTo(DefaultSqlA));
        }

        [Test]
        public void Build_CategoryFilterFullyTrusted_BuildsCorrectSql()
        {
            _builder.AddCategoryFilter("fullyTrusted");
            var actual = _builder.Build(ExampleTokenisedSqlA);
            Assert.That(actual, Is.EqualTo(AlternatelyFilteredSqlA));
        }

        [Test]
        public void Build_CategoryFilterPartiallyTrusted_BuildsCorrectSql()
        {
            _builder.AddCategoryFilter("partiallyTrusted");
            var actual = _builder.Build(ExampleTokenisedSqlA);
            Assert.That(actual, Is.EqualTo(FilteredSqlA));
        }

        [Test]
        public void Build_ProviderFilter_BuildsCorrectSql()
        {
            _builder.AddProviderFilter("test.com");
            var actual = _builder.Build(ExampleTokenisedSqlB);
            Assert.That(actual, Is.EqualTo(ExampleSqlB));
        }

        private const string ExampleTokenisedSqlA = @"SELECT `domain`,
`provider`,
`ip`,
SUM({FullyTrustedSum}) as `fully_trusted`,
SUM({PartiallyTrustedSum}) as `partially_trusted`,
SUM({UntrustedSum}) as `untrusted`,
SUM({QuarantinedSum}) as `quarantined`,
SUM({RejectedSum}) as `rejected`,
SUM({DeliveredSum}) as `delivered`,
SUM({PassSpfSumFiltered}) as `pass_spf_total`,
SUM({PassDkimSumFiltered}) as `pass_dkim_total`,
SUM({FailSpfSumFiltered}) as `fail_spf_total`,
SUM({FailDkimSumFiltered}) as `fail_dkim_total`,
SUM({TotalEmailsSumFiltered}) as `total_emails`,
SUM(`spf_misaligned`) as `spf_misaligned`,
SUM(`dkim_misaligned`) as `dkim_misaligned`,
FROM `domain_date_provider_ip` as a
WHERE domain = @domain
AND provider = @provider
AND (@ip IS NULL OR ip = @ip)
AND date BETWEEN @startDate AND @endDate
GROUP BY ip
ORDER BY `total_emails` DESC
LIMIT @offset, @pageSize;";

        private const string DefaultSqlA = @"SELECT `domain`,
`provider`,
`ip`,
SUM(spf_pass_dkim_pass_none + 0) as `fully_trusted`,
SUM(spf_fail_dkim_pass_none + spf_pass_dkim_fail_none + 0) as `partially_trusted`,
SUM(spf_fail_dkim_fail_none + 0) as `untrusted`,
SUM(spf_pass_dkim_pass_quarantine + spf_pass_dkim_fail_quarantine + spf_fail_dkim_pass_quarantine + spf_fail_dkim_fail_quarantine + 0) as `quarantined`,
SUM(spf_pass_dkim_pass_reject + spf_pass_dkim_fail_reject + spf_fail_dkim_pass_reject + spf_fail_dkim_fail_reject + 0) as `rejected`,
SUM(spf_pass_dkim_pass_none + spf_fail_dkim_pass_none + spf_pass_dkim_fail_none + spf_fail_dkim_fail_none + 0) as `delivered`,
SUM(spf_pass_dkim_pass_none + spf_pass_dkim_fail_none + spf_pass_dkim_pass_quarantine + spf_pass_dkim_fail_quarantine + spf_pass_dkim_pass_reject + spf_pass_dkim_fail_reject + 0) as `pass_spf_total`,
SUM(spf_pass_dkim_pass_none + spf_fail_dkim_pass_none + spf_pass_dkim_pass_quarantine + spf_fail_dkim_pass_quarantine + spf_pass_dkim_pass_reject + spf_fail_dkim_pass_reject + 0) as `pass_dkim_total`,
SUM(spf_fail_dkim_pass_none + spf_fail_dkim_fail_none + spf_fail_dkim_pass_quarantine + spf_fail_dkim_fail_quarantine + spf_fail_dkim_pass_reject + spf_fail_dkim_fail_reject + 0) as `fail_spf_total`,
SUM(spf_pass_dkim_fail_none + spf_fail_dkim_fail_none + spf_pass_dkim_fail_quarantine + spf_fail_dkim_fail_quarantine + spf_pass_dkim_fail_reject + spf_fail_dkim_fail_reject + 0) as `fail_dkim_total`,
SUM(spf_pass_dkim_pass_none + spf_pass_dkim_fail_none + spf_fail_dkim_pass_none + spf_fail_dkim_fail_none + spf_pass_dkim_pass_quarantine + spf_pass_dkim_fail_quarantine + spf_fail_dkim_pass_quarantine + spf_fail_dkim_fail_quarantine + spf_pass_dkim_pass_reject + spf_pass_dkim_fail_reject + spf_fail_dkim_pass_reject + spf_fail_dkim_fail_reject + 0) as `total_emails`,
SUM(`spf_misaligned`) as `spf_misaligned`,
SUM(`dkim_misaligned`) as `dkim_misaligned`,
FROM `domain_date_provider_ip` as a
WHERE domain = @domain
AND provider = @provider
AND (@ip IS NULL OR ip = @ip)
AND date BETWEEN @startDate AND @endDate
GROUP BY ip
ORDER BY `total_emails` DESC
LIMIT @offset, @pageSize;";

        private const string FilteredSqlA = @"SELECT `domain`,
`provider`,
`ip`,
SUM(spf_pass_dkim_pass_none + 0) as `fully_trusted`,
SUM(spf_fail_dkim_pass_none + spf_pass_dkim_fail_none + 0) as `partially_trusted`,
SUM(spf_fail_dkim_fail_none + 0) as `untrusted`,
SUM(spf_pass_dkim_pass_quarantine + spf_pass_dkim_fail_quarantine + spf_fail_dkim_pass_quarantine + spf_fail_dkim_fail_quarantine + 0) as `quarantined`,
SUM(spf_pass_dkim_pass_reject + spf_pass_dkim_fail_reject + spf_fail_dkim_pass_reject + spf_fail_dkim_fail_reject + 0) as `rejected`,
SUM(spf_pass_dkim_pass_none + spf_fail_dkim_pass_none + spf_pass_dkim_fail_none + spf_fail_dkim_fail_none + 0) as `delivered`,
SUM(spf_pass_dkim_fail_none + 0) as `pass_spf_total`,
SUM(spf_fail_dkim_pass_none + 0) as `pass_dkim_total`,
SUM(spf_fail_dkim_pass_none + 0) as `fail_spf_total`,
SUM(spf_pass_dkim_fail_none + 0) as `fail_dkim_total`,
SUM(spf_fail_dkim_pass_none + spf_pass_dkim_fail_none + 0) as `total_emails`,
SUM(`spf_misaligned`) as `spf_misaligned`,
SUM(`dkim_misaligned`) as `dkim_misaligned`,
FROM `domain_date_provider_ip` as a
WHERE domain = @domain
AND provider = @provider
AND (@ip IS NULL OR ip = @ip)
AND date BETWEEN @startDate AND @endDate
GROUP BY ip
ORDER BY `total_emails` DESC
LIMIT @offset, @pageSize;";

        private const string AlternatelyFilteredSqlA = @"SELECT `domain`,
`provider`,
`ip`,
SUM(spf_pass_dkim_pass_none + 0) as `fully_trusted`,
SUM(spf_fail_dkim_pass_none + spf_pass_dkim_fail_none + 0) as `partially_trusted`,
SUM(spf_fail_dkim_fail_none + 0) as `untrusted`,
SUM(spf_pass_dkim_pass_quarantine + spf_pass_dkim_fail_quarantine + spf_fail_dkim_pass_quarantine + spf_fail_dkim_fail_quarantine + 0) as `quarantined`,
SUM(spf_pass_dkim_pass_reject + spf_pass_dkim_fail_reject + spf_fail_dkim_pass_reject + spf_fail_dkim_fail_reject + 0) as `rejected`,
SUM(spf_pass_dkim_pass_none + spf_fail_dkim_pass_none + spf_pass_dkim_fail_none + spf_fail_dkim_fail_none + 0) as `delivered`,
SUM(spf_pass_dkim_pass_none + 0) as `pass_spf_total`,
SUM(spf_pass_dkim_pass_none + 0) as `pass_dkim_total`,
SUM(0) as `fail_spf_total`,
SUM(0) as `fail_dkim_total`,
SUM(spf_pass_dkim_pass_none + 0) as `total_emails`,
SUM(`spf_misaligned`) as `spf_misaligned`,
SUM(`dkim_misaligned`) as `dkim_misaligned`,
FROM `domain_date_provider_ip` as a
WHERE domain = @domain
AND provider = @provider
AND (@ip IS NULL OR ip = @ip)
AND date BETWEEN @startDate AND @endDate
GROUP BY ip
ORDER BY `total_emails` DESC
LIMIT @offset, @pageSize;";

        private const string ExampleTokenisedSqlB = @"SELECT `domain`,
`provider`,
`ip`,
SUM(`spf_misaligned`) as `spf_misaligned`,
SUM(`dkim_misaligned`) as `dkim_misaligned`,
FROM `domain_date_provider_ip` as a
WHERE domain = @domain
AND provider = @provider
{ProviderFilter}
AND (@ip IS NULL OR ip = @ip)
AND date BETWEEN @startDate AND @endDate
GROUP BY ip
ORDER BY `total_emails` DESC
LIMIT @offset, @pageSize;";

        private const string ExampleSqlB = @"SELECT `domain`,
`provider`,
`ip`,
SUM(`spf_misaligned`) as `spf_misaligned`,
SUM(`dkim_misaligned`) as `dkim_misaligned`,
FROM `domain_date_provider_ip` as a
WHERE domain = @domain
AND provider = @provider
 AND provider like @providerFilter
AND (@ip IS NULL OR ip = @ip)
AND date BETWEEN @startDate AND @endDate
GROUP BY ip
ORDER BY `total_emails` DESC
LIMIT @offset, @pageSize;";
    }
}
