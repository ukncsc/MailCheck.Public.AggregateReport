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
    internal interface IDkimAuthResultDao
    {
        Task Persist(List<Record> records, MySqlConnection connection);
    }

    internal class DkimAuthResultDao : IDkimAuthResultDao
    {
        private const int BatchSize = 10000;

        private readonly ILogger<DkimAuthResultDao> _log;

        public DkimAuthResultDao(ILogger<DkimAuthResultDao> log)
        {
            _log = log;
        }

        public async Task Persist(List<Record> records, MySqlConnection connection)
        {
            List<Tuple<long, DkimAuthResult>> dkimAuthResults = records
                .Select(_ => _.AuthResults.Dkim.Select(s => Tuple.Create(_.Id, s)))
                .SelectMany(_ => _)
                .ToList();

            _log.LogDebug($"Persisting {dkimAuthResults.Count} dkim auth results.");

            foreach (IEnumerable<Tuple<long, DkimAuthResult>> batch in dkimAuthResults.Batch(BatchSize))
            {
                Tuple<long, DkimAuthResult>[] dkimAuthResultsBatch = batch.ToArray();
                using (MySqlCommand command = new MySqlCommand())
                {
                    StringBuilder stringBuilder =
                        new StringBuilder(AggregateReportDaoResources.InsertDkimAuthResult);

                    for (int i = 0; i < dkimAuthResultsBatch.Length; i++)
                    {
                        stringBuilder.Append(string.Format(
                            AggregateReportDaoResources.InsertDkimAuthResultValueFormatString, i));
                        stringBuilder.Append(i < dkimAuthResultsBatch.Length - 1 ? "," : ";");

                        command.Parameters.AddWithValue($"a{i}", dkimAuthResultsBatch[i].Item1);
                        command.Parameters.AddWithValue($"b{i}", dkimAuthResultsBatch[i].Item2.Domain);
                        command.Parameters.AddWithValue($"c{i}", dkimAuthResultsBatch[i].Item2.Selector);
                        command.Parameters.AddWithValue($"d{i}", dkimAuthResultsBatch[i].Item2.Result?.ToString());
                        command.Parameters.AddWithValue($"e{i}", dkimAuthResultsBatch[i].Item2.HumanResult);
                    }

                    command.Connection = connection;
                    command.CommandText = stringBuilder.ToString();

                    await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
                }
            }
        }
    }
}