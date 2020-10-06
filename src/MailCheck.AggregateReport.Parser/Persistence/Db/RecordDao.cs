using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.Common.Util;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MailCheck.AggregateReport.Parser.Persistence.Db
{
    internal interface IRecordDao
    {
        Task Persist(AggregateReportInfo aggregateReport, MySqlConnection connection);
    }

    internal class RecordDao : IRecordDao
    {
        private const int BatchSize = 10000;

        private readonly ISpfAuthResultDao _spfAuthResultDao;
        private readonly IDkimAuthResultDao _dkimAuthResultDao;
        private readonly IPolicyOverrideReasonDao _policyOverrideReasonDao;
        private readonly ILogger<RecordDao> _log;

        public RecordDao(
            ISpfAuthResultDao spfAuthResultDao, 
            IDkimAuthResultDao dkimAuthResultDao,
            IPolicyOverrideReasonDao policyOverrideReasonDao,
            ILogger<RecordDao> log)
        {
            _spfAuthResultDao = spfAuthResultDao;
            _dkimAuthResultDao = dkimAuthResultDao;
            _policyOverrideReasonDao = policyOverrideReasonDao;
            _log = log;
        }

        public  async Task Persist(AggregateReportInfo aggregateReport, MySqlConnection connection)
        {
            _log.LogDebug($"Persisting {aggregateReport.AggregateReport.Records.Length} records.");

            foreach (IEnumerable<Record> batch in aggregateReport.AggregateReport.Records.Batch(BatchSize))
            {
                Record[] recordBatch = batch.ToArray();

                using (MySqlCommand command = new MySqlCommand())
                {
                    StringBuilder stringBuilder = new StringBuilder(AggregateReportDaoResources.InsertRecord);
                    for (int i = 0; i < recordBatch.Length; i++)
                    {
                        stringBuilder.Append(string.Format(AggregateReportDaoResources.InsertRecordValueFormatString, i));
                        stringBuilder.Append(i < recordBatch.Length - 1 ? "," : ";");

                        command.Parameters.AddWithValue($"a{i}", aggregateReport.Id);
                        command.Parameters.AddWithValue($"b{i}", recordBatch[i].Row.SourceIp);
                        command.Parameters.AddWithValue($"c{i}", recordBatch[i].Row.Count);
                        command.Parameters.AddWithValue($"d{i}", recordBatch[i].Row.PolicyEvaluated.Disposition?.ToString());
                        command.Parameters.AddWithValue($"e{i}", recordBatch[i].Row.PolicyEvaluated.Dkim?.ToString());
                        command.Parameters.AddWithValue($"f{i}", recordBatch[i].Row.PolicyEvaluated.Spf.ToString());
                        command.Parameters.AddWithValue($"g{i}", recordBatch[i].Identifiers.EnvelopeTo);
                        command.Parameters.AddWithValue($"h{i}", recordBatch[i].Identifiers.EnvelopeFrom);
                        command.Parameters.AddWithValue($"i{i}", recordBatch[i].Identifiers.HeaderFrom);
                    }

                    command.Connection = connection;
                    command.CommandText = stringBuilder.ToString();

                    await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);

                    long lastInsertId = command.LastInsertedId;

                    foreach (Record record in recordBatch)
                    {
                        record.Id = lastInsertId++;
                    }
                }
            }

            await _spfAuthResultDao.Persist(aggregateReport.AggregateReport.Records.ToList(), connection).ConfigureAwait(false);
            await _dkimAuthResultDao.Persist(aggregateReport.AggregateReport.Records.ToList(), connection).ConfigureAwait(false);
            await _policyOverrideReasonDao.Persist(aggregateReport.AggregateReport.Records.ToList(), connection).ConfigureAwait(false);
        }
    }
}