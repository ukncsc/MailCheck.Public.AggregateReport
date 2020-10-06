using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MailCheck.Common.Data;

namespace MailCheck.AggregateReport.Common.Aggregators
{
    public interface IRecord
    {
        /// <summary>
        /// Id of the originating record
        /// </summary>
        long Id { get; }
    }

    public interface IAggregatorDao<TRecord>
        where TRecord : IRecord
    {
        Task<Outcome> Save(IEnumerable<TRecord> records, AggregatorDao<TRecord>.Settings settings);
    }

    public class AggregatorDao<TRecord> : IAggregatorDao<TRecord>
        where TRecord : IRecord
    {
        private readonly ILogger<AggregatorDao<TRecord>> _log;
        private readonly IDatabase _database;
        private readonly Func<string, IDictionary<string, object>, Task<int>> _saveOperation;

        public AggregatorDao(ILogger<AggregatorDao<TRecord>> log, IDatabase database) : this(log, database, null) {}

        internal AggregatorDao(
            ILogger<AggregatorDao<TRecord>> log,
            IDatabase database,
            Func<string, IDictionary<string, object>, Task<int>> saveOperation
            )
        {
            _log = log;
            _database = database;
            _saveOperation = saveOperation ?? DefaultSaveToDatabase;
        }

        public async Task<Outcome> Save(IEnumerable<TRecord> records, Settings settings)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            var recordsList = records as IList<TRecord> ?? new List<TRecord>(records);

            if (recordsList.Count == 0)
            {
                _log.LogDebug("Empty record list passed to Save()");
                return Outcome.EmptyRecords;
            }

            var distinctRecordIds = recordsList
                .Select(r => r.Id)
                .Distinct()
                .ToArray();

            if (distinctRecordIds.Length > 1)
            {
                throw new AggregatorException($"Expected record batch to all use the same record ID but found {distinctRecordIds.Length} different IDs.");
            }

            var recordId = distinctRecordIds[0];

            var builder = new SqlBuilder()
                .AddAggregatorTokens(settings, recordsList.Count);

            var commandText = builder.Build(AggregatorDaoResources.IdempotentRecordUpsert);

            var parameters = new Dictionary<string, object>(
                recordsList.SelectMany((record, recordNum) => settings
                    .ParameterValuesMapper(record)
                    .ToDictionary(
                        paramValue => $"{paramValue.Key}_{recordNum}",
                        paramValue => paramValue.Value
                    )
                )
            );

            parameters.Add("record_id", recordId);

            var rowsAffected = await _saveOperation(commandText, parameters);

            _log.LogInformation("Save operation completed successfully for record {0} (records: {1} rows affected: {2})", recordId, recordsList.Count, rowsAffected);

            return Outcome.SavedSuccessfully;
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
                        throw new AggregatorException(message, e);
                    }
                    else
                    {
                        _log.LogWarning(e, message);
                    }

                    await Task.Delay(attempt * 1000);
                }
            }
        }

        public class Settings : AggregatorSqlBuilderSettings
        {
            public Func<TRecord, IEnumerable<KeyValuePair<string, object>>> ParameterValuesMapper { get; set; }
        }
    }

    public class Outcome
    {
        public static readonly Outcome SavedSuccessfully = new Outcome
        {
            Success = true,
            Message = "Records saved successfully"
        };

        public static readonly Outcome AlreadyProcessed = new Outcome
        {
            Success = true,
            Message = "Records already processed - ignored."
        };

        public static readonly Outcome EmptyRecords = new Outcome
        {
            Success = true,
            Message = "Records empty - nothing to save."
        };

        public bool Success { get; private set; }

        public string Message { get; private set; }
    }
}
