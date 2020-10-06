using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Data;
using Microsoft.Extensions.Logging;

namespace MailCheck.AggregateReport.EslrSaver.Dao
{
    public interface IEslrSaverDao
    {
        Task Save(EslrSaverRecord records);
    }

    public class EslrSaverDao : IEslrSaverDao
    {
        private static EslrSaverSqlBuilderSettings _settings = new EslrSaverSqlBuilderSettings
        {
            FieldNames = new[] {
                "record_id",
                "effective_date",
                "domain",
                "reverse_domain",
                "provider",
                "original_provider",
                "reporter_org_name",
                "ip",
                "count",
                "disposition",
                "dkim",
                "spf",
                "envelope_to",
                "envelope_from",
                "header_from",
                "organisation_domain_from",
                "spf_auth_results",
                "spf_pass_count",
                "spf_fail_count",
                "dkim_auth_results",
                "dkim_pass_count",
                "dkim_fail_count",
                "forwarded",
                "sampled_out",
                "trusted_forwarder",
                "mailing_list",
                "local_policy",
                "arc",
                "other_override_reason",
                "host_name",
                "host_org_domain",
                "host_provider",
                "host_as_number",
                "host_as_description",
                "host_country",
                "proxy_blocklist",
                "suspicious_network_blocklist",
                "hijacked_network_blocklist",
                "enduser_network_blocklist",
                "spam_source_blocklist",
                "malware_blocklist",
                "enduser_blocklist",
                "bounce_reflector_blocklist"
            }
        };

        private readonly ILogger<EslrSaverDao> _log;
        private readonly IDatabase _database;
        private readonly Func<string, IDictionary<string, object>, Task<int>> _saveOperation;

        public EslrSaverDao(ILogger<EslrSaverDao> log, IDatabase database) : this(log, database, null) {}

        internal EslrSaverDao(
            ILogger<EslrSaverDao> log,
            IDatabase database,
            Func<string, IDictionary<string, object>, Task<int>> saveOperation
        )
        {
            _log = log;
            _database = database;
            _saveOperation = saveOperation ?? DefaultSaveToDatabase;
        }

        public async Task Save(EslrSaverRecord record)
        {
            var builder = new SqlBuilder()
                .AddEslrSaverTokens(_settings);

            var commandText = builder.Build(EslrSaverDaoResources.Insert);

            await _saveOperation(commandText, CreateParameters(record));
        }

        private async Task<int> DefaultSaveToDatabase(string commandText, IDictionary<string, object> parameterValues)
        {
            int rowsAffected = 0;

            await WithRetry(5, async () => {
                rowsAffected = await SaveToDatabase(commandText, parameterValues);
            });

            return rowsAffected;
        }

        private async Task<int> SaveToDatabase(string commandText, IDictionary<string, object> parameterValues)
        {
            using (var connection = await _database.CreateAndOpenConnectionAsync())
            using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = System.Data.CommandType.Text;
                command.Transaction = transaction;

                var parameters = parameterValues.Select(kvp =>
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = kvp.Key;
                    parameter.Value = kvp.Value;
                    return parameter;
                }).ToArray();

                command.Parameters.AddRange(parameters);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                transaction.Commit();

                return rowsAffected;
            }
        }

        private async Task WithRetry(int maxAttempts, Func<Task> work)
        {
            int attempt = 0;

            while (attempt++ <= maxAttempts)
            {
                try
                {
                    await work();
                    break;
                }
                catch (Exception e)
                {
                    var message = $"Error occured saving records to database (attempt {attempt} of {maxAttempts})";

                    if (attempt == maxAttempts)
                    {
                        _log.LogError(e, message);
                        throw;
                    }
                    else
                    {
                        _log.LogWarning(e, message);
                    }

                    await Task.Delay(attempt * 1000);
                }
            }
        }

        private static IDictionary<string, object> CreateParameters(EslrSaverRecord eslr)
        {
            return new Dictionary<string, object>
            {
                ["record_id"] = eslr.RecordId,
                ["effective_date"] = eslr.EffectiveDate.ToString("yyyy-MM-dd"),
                ["domain"] = eslr.Domain,
                ["reverse_domain"] = eslr.ReverseDomain,
                ["provider"] = eslr.Provider,
                ["original_provider"] = eslr.OriginalProvider,
                ["reporter_org_name"] = eslr.ReporterOrgName,
                ["ip"] = eslr.Ip,
                ["count"] = eslr.Count,
                ["disposition"] = eslr.Disposition,
                ["dkim"] = eslr.Dkim,
                ["spf"] = eslr.Spf,
                ["envelope_to"] = eslr.EnvelopeTo,
                ["envelope_from"] = eslr.EnvelopeFrom,
                ["header_from"] = eslr.HeaderFrom,
                ["organisation_domain_from"] = eslr.OrganisationDomainFrom,
                ["spf_auth_results"] = eslr.SpfAuthResults,
                ["spf_pass_count"] = eslr.SpfPassCount,
                ["spf_fail_count"] = eslr.SpfFailCount,
                ["dkim_auth_results"] = eslr.DkimAuthResults,
                ["dkim_pass_count"] = eslr.DkimPassCount,
                ["dkim_fail_count"] = eslr.DkimFailCount,
                ["forwarded"] = eslr.Forwarded,
                ["sampled_out"] = eslr.SampledOut,
                ["trusted_forwarder"] = eslr.TrustedForwarder,
                ["mailing_list"] = eslr.MailingList,
                ["local_policy"] = eslr.LocalPolicy,
                ["arc"] = eslr.Arc,
                ["other_override_reason"] = eslr.OtherOverrideReason,
                ["host_name"] = eslr.HostName,
                ["host_org_domain"] = eslr.HostOrgDomain,
                ["host_provider"] = eslr.HostProvider,
                ["host_as_number"] = eslr.HostAsNumber,
                ["host_as_description"] = eslr.HostAsDescription,
                ["host_country"] = eslr.HostCountry,
                ["proxy_blocklist"] = eslr.ProxyBlockListCount,
                ["suspicious_network_blocklist"] = eslr.HijackedNetworkBlockListCount,
                ["hijacked_network_blocklist"] = eslr.SuspiciousNetworkBlockListCount,
                ["enduser_network_blocklist"] = eslr.EndUserNetworkBlockListCount,
                ["spam_source_blocklist"] = eslr.SpamSourceBlockListCount,
                ["malware_blocklist"] = eslr.MalwareBlockListCount,
                ["enduser_blocklist"] = eslr.EndUserBlockListCount,
                ["bounce_reflector_blocklist"] = eslr.BounceReflectorBlockListCount
            };
        }
    }
}