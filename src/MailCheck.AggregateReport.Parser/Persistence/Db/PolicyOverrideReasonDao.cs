using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.Common.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MailCheck.AggregateReport.Parser.Persistence.Db
{
    internal interface IPolicyOverrideReasonDao
    {
        Task Persist(List<Record> records, MySqlConnection connection);
    }

    internal class PolicyOverrideReasonDao : IPolicyOverrideReasonDao
    {
        private readonly ILogger<PolicyOverrideReasonDao> _log;
        private const int BatchSize = 10000;

        public PolicyOverrideReasonDao(ILogger<PolicyOverrideReasonDao> log)
        {
            _log = log;
        }

        public async Task Persist(List<Record> records, MySqlConnection connection)
        {
            List<Tuple<long, PolicyOverrideReason>> policyOverrideReasons = records
                .Select(_ => _.Row.PolicyEvaluated.Reasons.Select(s => Tuple.Create(_.Id, s)))
                .SelectMany(_ => _)
                .ToList();

            _log.LogDebug($"Persisting {policyOverrideReasons.Count} policy override reasons.");

            foreach (IEnumerable<Tuple<long, PolicyOverrideReason>> batch in policyOverrideReasons.Batch(BatchSize))
            {
                Tuple<long, PolicyOverrideReason>[] reasonBatch = batch.ToArray();
                using (MySqlCommand command = new MySqlCommand())
                {
                    StringBuilder stringBuilder =
                        new StringBuilder(AggregateReportDaoResources.InsertPolicyOverrideReason);

                    for (int i = 0; i < reasonBatch.Length; i++)
                    {
                        stringBuilder.Append(
                            string.Format(AggregateReportDaoResources.InsertPolicyOverrideReasonValueFormatString, i));
                        stringBuilder.Append(i < reasonBatch.Length - 1 ? "," : ";");

                        command.Parameters.AddWithValue($"a{i}", reasonBatch[i].Item1);
                        command.Parameters.AddWithValue($"b{i}", reasonBatch[i].Item2.PolicyOverride?.ToString());
                        command.Parameters.AddWithValue($"c{i}", reasonBatch[i].Item2.Comment);
                    }

                    command.Connection = connection;
                    command.CommandText = stringBuilder.ToString();

                    await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
                }
            }
        }
    }
}