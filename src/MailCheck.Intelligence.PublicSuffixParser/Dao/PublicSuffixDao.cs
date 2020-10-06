using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace MailCheck.Intelligence.PublicSuffixParser.Dao
{
    public interface IPublicSuffixDao
    {
        Task Save(List<string> publicSuffixEntries);
    }

    public class PublicSuffixDao : IPublicSuffixDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<PublicSuffixDao> _log;

        public PublicSuffixDao(IConnectionInfoAsync connectionInfo, 
            ILogger<PublicSuffixDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public async Task Save(List<string> publicSuffixEntries)
        {
            _log.LogInformation("Persisting public suffix list");
            Stopwatch stopwatch = Stopwatch.StartNew();

            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    await Truncate(connection, transaction);
                    await Save(publicSuffixEntries, connection, transaction);

                    await transaction.CommitAsync();
                    await RefreshMaterializedView(connection);
                }
                connection.Close();
            }
            _log.LogInformation($"Persisted public suffix list in {stopwatch.ElapsedMilliseconds} ms.");
        }

        private async Task Save(List<string> ipToAsns, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            if (ipToAsns.Any())
            {
                string valueFormats = string.Join(",", 
                    ipToAsns.Select((v, i) => string.Format(PublicSuffixDaoResource.InsertPublicSuffixValueFormatString, i)));

                string commandText =
                    $"{PublicSuffixDaoResource.InsertPublicSuffix}{valueFormats} {PublicSuffixDaoResource.InsertPublicSuffixOnConflict}";

                NpgsqlParameter[] parameters = ipToAsns.Select((v,i) => new NpgsqlParameter($"suffix_{i}", v)).ToArray();

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
            using (NpgsqlCommand command = new NpgsqlCommand(PublicSuffixDaoResource.Truncate, connection))
            {
                command.Transaction = transaction;
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task RefreshMaterializedView(NpgsqlConnection connection)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(PublicSuffixDaoResource.RefreshMaterializedView, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
