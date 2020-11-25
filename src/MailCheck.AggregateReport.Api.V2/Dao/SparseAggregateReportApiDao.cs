using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.Common.Data;
using MailCheck.Common.Data.Abstractions;
using MySql.Data.MySqlClient;
using MailCheck.Common.Data.Util;
using MailCheck.Common.Util;
using MySqlHelper = MailCheck.Common.Data.Util.MySqlHelper;

namespace MailCheck.AggregateReport.Api.V2.Dao
{
    public class SparseAggregateReportApiDao : IAggregateReportApiDao
    {
        private readonly IConnectionInfoAsync _connectionInfoAsync;
        private readonly IAggregateReportExportStatsFactory _aggregateReportExportStatsFactory;

        public SparseAggregateReportApiDao(IConnectionInfoAsync connectionInfoAsync, IAggregateReportExportStatsFactory aggregateReportExportStatsFactory)
        {
            _connectionInfoAsync = connectionInfoAsync;
            _aggregateReportExportStatsFactory = aggregateReportExportStatsFactory;
        }

        public async Task<StatsSummary> GetDomainStatsSummary(string domain, DateTime startDate, DateTime endDate,
            bool rollup = false, string categoryFilter = null)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            string commandTemplate = rollup
                        ? AggregateReportDaoV2.SelectDomainRollupStatsSummary
                        : AggregateReportDaoV2.SelectDomainStatsSummary;

            string commandText = new SqlBuilder()
                .AddAggregateReportDefaults()
                .AddCategoryFilter(categoryFilter)
                .Build(commandTemplate);

            MySqlParameter[] parameters =
            {
                new MySqlParameter("domain", domain),
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd"))
            };

            StatsSummary ipStatsSummary = null;
            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, commandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    ipStatsSummary = new StatsSummary(
                        reader.GetInt32("TotalEmails"),
                        reader.GetInt32("TotalFullyTrusted"),
                        reader.GetInt32("TotalPartiallyTrusted"),
                        reader.GetInt32("TotalQuarantined"),
                        reader.GetInt32("TotalRejected"),
                        reader.GetInt32("TotalUntrusted")
                    );
                }
            }

            return ipStatsSummary;
        }

        public async Task<List<DomainStats>> GetDomainStats(string domain, DateTime startDate, DateTime endDate,
            bool rollup = false, string categoryFilter = null)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            string commandTemplate = rollup
                ? AggregateReportDaoV2.SelectDominStatsRollup
                : AggregateReportDaoV2.SelectDomainStats;

            string commandText = new SqlBuilder()
                .AddAggregateReportDefaults()
                .AddCategoryFilter(categoryFilter)
                .Build(commandTemplate);

            MySqlParameter[] parameters =
            {
                new MySqlParameter("domain", domain),
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd"))
            };

            List<DomainStats> domainStats = new List<DomainStats>();

            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, commandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    domainStats.Add(new DomainStats(
                        reader.GetString("domain"),
                        reader.GetDateTime("date"),
                        reader.GetInt64("spf_pass_dkim_pass_none"),
                        reader.GetInt64("spf_pass_dkim_fail_none"),
                        reader.GetInt64("spf_fail_dkim_pass_none"),
                        reader.GetInt64("spf_fail_dkim_fail_none"),
                        reader.GetInt64("spf_pass_dkim_pass_quarantine"),
                        reader.GetInt64("spf_pass_dkim_fail_quarantine"),
                        reader.GetInt64("spf_fail_dkim_pass_quarantine"),
                        reader.GetInt64("spf_fail_dkim_fail_quarantine"),
                        reader.GetInt64("spf_pass_dkim_pass_reject"),
                        reader.GetInt64("spf_pass_dkim_fail_reject"),
                        reader.GetInt64("spf_fail_dkim_pass_reject"),
                        reader.GetInt64("spf_fail_dkim_fail_reject")
                    ));
                }
            }

            return domainStats;
        }

        public async Task<StatsSummary> GetProviderStatsSummary(string domain, DateTime startDate, DateTime endDate,
            int page, int pageSize, bool rollup = false, string categoryFilter = null, string providerFilter = null)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            string commandTemplate = 
                rollup
                    ? AggregateReportDaoV2.SelectProviderRollupStatsSummary
                    : AggregateReportDaoV2.SelectProviderStatsSummary;

            string commandText = new SqlBuilder()
                .AddAggregateReportDefaults()
                .AddCategoryFilter(categoryFilter)
                .AddProviderFilter(providerFilter)
                .Build(commandTemplate);

            MySqlParameter[] parameters =
            {
                new MySqlParameter("domain", domain),
                new MySqlParameter("providerFilter", FormatProvider(providerFilter)),
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd"))
            };

            StatsSummary ipStatsSummary = null;
            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, commandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    ipStatsSummary = new StatsSummary(
                        reader.GetInt32("TotalEmails"),
                        reader.GetInt32("TotalFullyTrusted"),
                        reader.GetInt32("TotalPartiallyTrusted"),
                        reader.GetInt32("TotalQuarantined"),
                        reader.GetInt32("TotalRejected"),
                        reader.GetInt32("TotalUntrusted"));
                }
            }

            return ipStatsSummary;
        }

        public async Task<ProviderStatsResult> GetProviderStats(string domain, DateTime startDate, DateTime endDate,
            int page, int pageSize, bool rollup = false, string categoryFilter = null, string providerFilter = null)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            string commandTemplate = rollup
                    ? AggregateReportDaoV2.SelectProviderStatsRollup
                    : AggregateReportDaoV2.SelectProviderStats;

            var sqlBuilder = new SqlBuilder()
                .AddAggregateReportDefaults()
                .AddCategoryFilter(categoryFilter)
                .AddProviderFilter(providerFilter);

            var commandText = sqlBuilder
                .Build(commandTemplate);

            MySqlParameter[] parameters =
            {
                new MySqlParameter("domain", domain),
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("providerFilter", FormatProvider(providerFilter)),
                new MySqlParameter("offset", (page - 1) * pageSize),
                new MySqlParameter("pageSize", pageSize),
            };

            List<ProviderStats> providerStats = new List<ProviderStats>();

            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, commandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    providerStats.Add(new ProviderStats(
                        reader.GetString("domain"),
                        reader.GetString("provider"),
                        reader.GetInt64("spf_pass_dkim_pass_none"),
                        reader.GetInt64("spf_pass_dkim_fail_none"),
                        reader.GetInt64("spf_fail_dkim_pass_none"),
                        reader.GetInt64("spf_fail_dkim_fail_none"),
                        reader.GetInt64("spf_pass_dkim_pass_quarantine"),
                        reader.GetInt64("spf_pass_dkim_fail_quarantine"),
                        reader.GetInt64("spf_fail_dkim_pass_quarantine"),
                        reader.GetInt64("spf_fail_dkim_fail_quarantine"),
                        reader.GetInt64("spf_pass_dkim_pass_reject"),
                        reader.GetInt64("spf_pass_dkim_fail_reject"),
                        reader.GetInt64("spf_fail_dkim_pass_reject"),
                        reader.GetInt64("spf_fail_dkim_fail_reject"),
                        reader.GetInt64("fully_trusted"),
                        reader.GetInt64("partially_trusted"),
                        reader.GetInt64("untrusted"),
                        reader.GetInt64("quarantined"),
                        reader.GetInt64("rejected"),
                        reader.GetInt64("total_emails"),
                        reader.GetInt64("fail_spf_total"),
                        reader.GetInt64("fail_dkim_total")
                    ));
                }
            }

            string providersCountCommandFormatString = rollup
                ? AggregateReportDaoV2.SelectProviderStatsRollupCount
                : AggregateReportDaoV2.SelectProviderStatsCount;
            string providersCountCommandText = sqlBuilder.Build(providersCountCommandFormatString);
            Task<object> providersCount = MySqlHelper.ExecuteScalarAsync(connectionString, providersCountCommandText, parameters);
            
            string allProvidersTotalEmailCountCommandFormatString = rollup
                ? AggregateReportDaoV2.SelectAllProviderRollupCount
                : AggregateReportDaoV2.SelectAllProviderCount;
            string allProvidersTotalEmailCountCommandText = sqlBuilder.Build(allProvidersTotalEmailCountCommandFormatString);
            Task<object> allProviderCount = MySqlHelper.ExecuteScalarAsync(connectionString, allProvidersTotalEmailCountCommandText, parameters);

            await Task.WhenAll(providersCount, allProviderCount);

            return new ProviderStatsResult(providerStats, ParseInt(providersCount.Result),
                ParseInt(allProviderCount.Result));
        }

        public async Task<SubdomainStatsResult> GetSubdomainStats(string domain, string provider, DateTime startDate,
            DateTime endDate, int page, int pageSize, string categoryFilter = null)
        {
            var sqlBuilder = new SqlBuilder()
                .AddAggregateReportDefaults()
                .AddCategoryFilter(categoryFilter);

            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            MySqlParameter[] parameters =
            {
                new MySqlParameter("domain", domain),
                new MySqlParameter("provider", provider),
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("offset", (page - 1) * pageSize),
                new MySqlParameter("pageSize", pageSize),
            };

            SubdomainStats domainStats = null;

            string selectSingleProviderStatsCommandText = sqlBuilder.Build(AggregateReportDaoV2.SelectSingleProviderStats);

            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString,
                selectSingleProviderStatsCommandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    domainStats = CreateSubdomainStats(reader);
                    break;
                }
            }

            List<SubdomainStats> subdomainStats = new List<SubdomainStats>();

            string selectSubdomainStatsCommandText = sqlBuilder.Build(AggregateReportDaoV2.SelectSubdomainStats);

            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, selectSubdomainStatsCommandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    subdomainStats.Add(CreateSubdomainStats(reader));
                }
            }

            string selectSubdomainCountCommandText = sqlBuilder.Build(AggregateReportDaoV2.SelectSubdomainCount);

            object subdomainCount = await MySqlHelper.ExecuteScalarAsync(connectionString, selectSubdomainCountCommandText, parameters);

            return new SubdomainStatsResult(domainStats, subdomainStats, ParseInt(subdomainCount));
        }

        public async Task<IpStatsResult> GetIpStats(string domain, DateTime startDate, DateTime endDate,
            string provider, int page, int pageSize, string ipFilter = null, string hostFilter = null,
            string categoryFilter = null)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            var sqlBuilder = new SqlBuilder()
                .AddAggregateReportDefaults()
                .AddCategoryFilter(categoryFilter)
                .AddIpFilter(ipFilter);

            string commandText = sqlBuilder.Build(AggregateReportDaoV2.SelectIpStats);

            MySqlParameter[] parameters =
            {
                new MySqlParameter("domain", domain),
                new MySqlParameter("provider", provider),
                new MySqlParameter("ip", ipFilter),
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("offset", (page - 1) * pageSize),
                new MySqlParameter("pageSize", pageSize)
            };

            List<IpStats> ipStats = new List<IpStats>();

            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, commandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    ipStats.Add(new IpStats(
                        reader.GetString("domain"),
                        reader.GetString("provider"),
                        reader.GetString("ip"),
                        reader.GetString("hostname"),
                        reader.GetInt64("spf_pass_dkim_pass_none"),
                        reader.GetInt64("spf_pass_dkim_fail_none"),
                        reader.GetInt64("spf_fail_dkim_pass_none"),
                        reader.GetInt64("spf_fail_dkim_fail_none"),
                        reader.GetInt64("spf_pass_dkim_pass_quarantine"),
                        reader.GetInt64("spf_pass_dkim_fail_quarantine"),
                        reader.GetInt64("spf_fail_dkim_pass_quarantine"),
                        reader.GetInt64("spf_fail_dkim_fail_quarantine"),
                        reader.GetInt64("spf_pass_dkim_pass_reject"),
                        reader.GetInt64("spf_pass_dkim_fail_reject"),
                        reader.GetInt64("spf_fail_dkim_pass_reject"),
                        reader.GetInt64("spf_fail_dkim_fail_reject"),
                        reader.GetInt64("fully_trusted"),
                        reader.GetInt64("partially_trusted"),
                        reader.GetInt64("untrusted"),
                        reader.GetInt64("quarantined"),
                        reader.GetInt64("rejected"),
                        reader.GetInt64("delivered"),
                        reader.GetInt64("total_emails"),
                        reader.GetInt64("pass_spf_total"),
                        reader.GetInt64("pass_dkim_total"),
                        reader.GetInt64("fail_spf_total"),
                        reader.GetInt64("fail_dkim_total"),
                        reader.GetInt64("spf_misaligned"),
                        reader.GetInt64("dkim_misaligned"),
                        reader.GetInt32("blocklists_proxy"),
                        reader.GetInt32("blocklists_suspiciousnetwork"),
                        reader.GetInt32("blocklists_hijackednetwork"),
                        reader.GetInt32("blocklists_endusernetwork"),
                        reader.GetInt32("blocklists_spamsource"),
                        reader.GetInt32("blocklists_malware"),
                        reader.GetInt32("blocklists_enduser"),
                        reader.GetInt32("blocklists_bouncereflector"),
                        reader.GetInt32("por_forwarded"),
                        reader.GetInt32("por_sampledout"),
                        reader.GetInt32("por_trustedforwarder"),
                        reader.GetInt32("por_mailinglist"),
                        reader.GetInt32("por_localpolicy"),
                        reader.GetInt32("por_arc"),
                        reader.GetInt32("por_other")
                    ));
                }
            }

            long hostnameCount = 0, totalEmails = 0;

            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, sqlBuilder.Build(AggregateReportDaoV2.SelectIpStatsCount), parameters))
            {
                while (await reader.ReadAsync())
                {
                    hostnameCount = reader.GetInt64("hostname_count");
                    totalEmails = reader.GetInt64("total_emails");
                }
            }

            return new IpStatsResult(ipStats, hostnameCount, totalEmails);
        }

        public async Task<StatsSummary> GetIpStatsSummary(string domain, DateTime startDate, DateTime endDate,
            string provider, string categoryFilter = null, string hostFilter = null)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            var sqlBuilder = new SqlBuilder()
                .AddAggregateReportDefaults()
                .AddCategoryFilter(categoryFilter);

            string commandText = sqlBuilder.Build(AggregateReportDaoV2.SelectIpStatsSummary);

            MySqlParameter[] parameters =
            {
                new MySqlParameter("domain", domain),
                new MySqlParameter("provider", provider),
                new MySqlParameter("hostFilter", string.IsNullOrWhiteSpace(hostFilter) ? null : hostFilter + "%"),
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd"))
            };

            StatsSummary ipStatsSummary = null;
            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, commandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    ipStatsSummary = new StatsSummary(
                        reader.GetInt32("TotalEmails"),
                        reader.GetInt32("TotalFullyTrusted"),
                        reader.GetInt32("TotalPartiallyTrusted"),
                        reader.GetInt32("TotalQuarantined"),
                        reader.GetInt32("TotalRejected"),
                        reader.GetInt32("TotalUntrusted"));
                }
            }

            return ipStatsSummary;
        }

        public async Task<List<SpfDomainStats>> GetSpfDomainStats(string domain, string provider, string ip,
            DateTime startDate, DateTime endDate)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            string commandText = AggregateReportDaoV2.SelectSpfDomainStats;

            MySqlParameter[] parameters =
            {
                new MySqlParameter("domain", domain),
                new MySqlParameter("provider", provider),
                new MySqlParameter("ip", ip),
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd"))
            };

            List<SpfDomainStats> spfDomainStats = new List<SpfDomainStats>();

            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, commandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    spfDomainStats.Add(new SpfDomainStats(
                        reader.GetString("domain"),
                        reader.GetString("provider"),
                        reader.GetString("ip"),
                        reader.GetString("spf_domain"),
                        reader.GetInt64("spf_pass"),
                        reader.GetInt64("spf_fail")
                    ));
                }
            }

            return spfDomainStats;
        }

        public async Task<List<DkimDomainStats>> GetDkimDomainStats(string domain, string provider, string ip,
            DateTime startDate, DateTime endDate)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            string commandText = AggregateReportDaoV2.SelectDkimDomainStats;

            MySqlParameter[] parameters =
            {
                new MySqlParameter("domain", domain),
                new MySqlParameter("provider", provider),
                new MySqlParameter("ip", ip),
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd"))
            };

            List<DkimDomainStats> dkimDomainStats = new List<DkimDomainStats>();

            using (var reader = await MySqlHelper.ExecuteReaderAsync(connectionString, commandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    dkimDomainStats.Add(new DkimDomainStats(
                        reader.GetString("domain"),
                        reader.GetString("provider"),
                        reader.GetString("ip"),
                        reader.GetString("dkim_domain"),
                        reader.GetString("dkim_selector"),
                        reader.GetInt64("dkim_pass"),
                        reader.GetInt64("dkim_fail")
                    ));
                }
            }

            return dkimDomainStats;
        }

       

        public async Task<List<AggregateReportExportStats>> GetAggregateReportExport(string domain, string provider, string ip, DateTime startDate,
            DateTime endDate, bool includeSubdomains)
        {
            string reverseDomain = DomainNameUtils.ReverseDomainName(domain);
            
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();

            string commandText = includeSubdomains
                ? AggregateReportDaoV2.SelectAggregateExportDataWithSubdomains
                : AggregateReportDaoV2.SelectAggregateExportData;

            MySqlParameter[] parameters =
            {
                new MySqlParameter("reverseDomain", reverseDomain),
                new MySqlParameter("provider", provider),
                new MySqlParameter("ip", ip), 
                new MySqlParameter("startDate", startDate.ToString("yyyy-MM-dd")),
                new MySqlParameter("endDate", endDate.ToString("yyyy-MM-dd"))
            };

            List<AggregateReportExportStats> aggregateReportExportStats = new List<AggregateReportExportStats>();

            using (DbDataReader reader = await MySqlHelper.ExecuteReaderAsync(connectionString, commandText, parameters))
            {
                while (await reader.ReadAsync())
                {
                    aggregateReportExportStats.Add(_aggregateReportExportStatsFactory.Create(reader));
                }
            }
            return aggregateReportExportStats;
        }

        private SubdomainStats CreateSubdomainStats(DbDataReader reader)
        {
            return new SubdomainStats(
                reader.GetString("domain"),
                reader.GetString("provider"),
                reader.GetString("subdomain"),
                reader.GetInt64("spf_pass_dkim_pass_none"),
                reader.GetInt64("spf_pass_dkim_fail_none"),
                reader.GetInt64("spf_fail_dkim_pass_none"),
                reader.GetInt64("spf_fail_dkim_fail_none"),
                reader.GetInt64("spf_pass_dkim_pass_quarantine"),
                reader.GetInt64("spf_pass_dkim_fail_quarantine"),
                reader.GetInt64("spf_fail_dkim_pass_quarantine"),
                reader.GetInt64("spf_fail_dkim_fail_quarantine"),
                reader.GetInt64("spf_pass_dkim_pass_reject"),
                reader.GetInt64("spf_pass_dkim_fail_reject"),
                reader.GetInt64("spf_fail_dkim_pass_reject"),
                reader.GetInt64("spf_fail_dkim_fail_reject"),
                reader.GetInt64("total_emails"),
                reader.GetInt64("fail_spf_total"),
                reader.GetInt64("fail_dkim_total")
            );
        }

        private int ParseInt(object value)
        {
            return string.IsNullOrWhiteSpace(value.ToString()) ? 0 : int.Parse(value.ToString());
        }
        
        private string FormatProvider(string filter)
        {
            return string.IsNullOrWhiteSpace(filter) ? null : filter + "%";
        }
    }
}