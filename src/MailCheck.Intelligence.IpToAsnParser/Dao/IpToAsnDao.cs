using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Util;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace MailCheck.Intelligence.IpToAsnParser.Dao
{
    public abstract class IpToAsnDao
    {
        protected abstract string InsertTpToAsnValueFormatString { get; }
        protected abstract string InsertIpToAsnCommand { get; }
        protected abstract string InsertIpToAsnOnConflict { get; }
        protected abstract string TruncateCommand { get; }
        protected abstract string RefreshMaterializedViewCommand { get; }

        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<IpToAsnDao> _log;

        protected IpToAsnDao(IConnectionInfoAsync connectionInfo,
            ILogger<IpToAsnDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public async Task Save(List<IpToAsn> ipToAsns)
        {
            _log.LogInformation("Persisting ip to asn info");
            Stopwatch stopwatch = Stopwatch.StartNew();
            IEnumerable<IEnumerable<IpToAsn>> batches = ipToAsns.Batch(20000);

            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    await Truncate(connection, transaction);
                    foreach (IEnumerable<IpToAsn> batch in batches)
                    {
                        await Save(batch.ToList(), connection, transaction);
                    }

                    await transaction.CommitAsync();
                    await RefreshMaterializedView(connection);
                }
                connection.Close();
            }
            _log.LogInformation($"Persisted ip to asn info in {stopwatch.ElapsedMilliseconds} ms.");
        }



        private async Task Save(List<IpToAsn> ipToAsns, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            IEnumerable<NpgsqlParameter> CreateParameters(IpToAsn result, int i)
            {
                yield return new NpgsqlParameter($"ip_address_{i}", result.IpAddr);
                yield return new NpgsqlParameter($"asn_{i}", result.Asn);
            }

            if (ipToAsns.Any())
            {
                string valueFormats = string.Join(",", ipToAsns.Select((v, i) => string.Format(InsertTpToAsnValueFormatString, i)));

                string commandText =
                    $"{InsertIpToAsnCommand}{valueFormats} {InsertIpToAsnOnConflict}";

                NpgsqlParameter[] parameters = ipToAsns.Select(CreateParameters).SelectMany(_ => _).ToArray();

                using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
                {
                    command.Transaction = transaction;
                    command.Parameters.AddRange(parameters);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task Truncate(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(TruncateCommand, connection))
            {
                command.Transaction = transaction;
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task RefreshMaterializedView(NpgsqlConnection connection)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(RefreshMaterializedViewCommand, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}