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
    internal interface ISpfAuthResultDao
    {
        Task Persist(List<Record> records, MySqlConnection connection);
    }

    internal class SpfAuthResultDao : ISpfAuthResultDao
    {
        private const int BatchSize = 10000;

        private readonly ILogger<SpfAuthResultDao> _log;

        public SpfAuthResultDao(ILogger<SpfAuthResultDao> log)
        {
            _log = log;
        }

        public async Task Persist(List<Record> records, MySqlConnection connection)
        {
            List<Tuple<long, SpfAuthResult>> spfAuthResults = records
                .Select(_ => _.AuthResults.Spf.Select(s => Tuple.Create(_.Id, s)))
                .SelectMany(_ => _)
                .ToList();

            _log.LogDebug($"Persisting {spfAuthResults.Count} spf auth results.");

            foreach (IEnumerable<Tuple<long, SpfAuthResult>> batch in spfAuthResults.Batch(BatchSize))
            {
                Tuple<long, SpfAuthResult>[] spfAuthResultsBatch = batch.ToArray();

                using (MySqlCommand command = new MySqlCommand())
                {
                    StringBuilder stringBuilder = new StringBuilder(AggregateReportDaoResources.InsertSpfAuthResult);

                    for (int i = 0; i < spfAuthResultsBatch.Length; i++)
                    {
                        stringBuilder.Append(string.Format(
                            AggregateReportDaoResources.InsertSpfAuthResultValueFormatString, i));
                        stringBuilder.Append(i < spfAuthResultsBatch.Length - 1 ? "," : ";");

                        command.Parameters.AddWithValue($"a{i}", spfAuthResultsBatch[i].Item1);
                        command.Parameters.AddWithValue($"b{i}", spfAuthResultsBatch[i].Item2.Domain);
                        command.Parameters.AddWithValue($"c{i}", spfAuthResultsBatch[i].Item2.Scope?.ToString());
                        command.Parameters.AddWithValue($"d{i}", spfAuthResultsBatch[i].Item2.Result?.ToString());
                    }

                    command.Connection = connection;
                    command.CommandText = stringBuilder.ToString();

                    await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
                }
            }
        }
    }
}