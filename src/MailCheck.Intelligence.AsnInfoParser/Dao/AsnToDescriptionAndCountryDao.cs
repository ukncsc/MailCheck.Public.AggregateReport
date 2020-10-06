using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Util;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace MailCheck.Intelligence.AsnInfoParser.Dao
{
    public interface IAsnToDescriptionAndCountryDao
    {
        Task Save(List<AsnDescriptionCountryInfo> asnNameCountryInfos);
    }

    public class AsnToDescriptionAndCountryDao : IAsnToDescriptionAndCountryDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<AsnToDescriptionAndCountryDao> _log;

        public AsnToDescriptionAndCountryDao(IConnectionInfoAsync connectionInfo,
            ILogger<AsnToDescriptionAndCountryDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public async Task Save(List<AsnDescriptionCountryInfo> asnNameCountryInfos)
        {
            _log.LogInformation("Persisting asn to description and country info");
            Stopwatch stopwatch = Stopwatch.StartNew();
            IEnumerable<IEnumerable<AsnDescriptionCountryInfo>> batches = asnNameCountryInfos.Batch(15000);

            string connectionString = await _connectionInfo.GetConnectionStringAsync();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    await Truncate(connection, transaction);
                    foreach (IEnumerable<AsnDescriptionCountryInfo> batch in batches)
                    {
                        await Save(batch.ToList(), connection, transaction);
                    }

                    await transaction.CommitAsync();
                    await RefreshMaterializedView(connection);
                }
                connection.Close();
            }
            _log.LogInformation($"Persisted asn to description and country info in {stopwatch.ElapsedMilliseconds} ms.");
        }

        private async Task Save(List<AsnDescriptionCountryInfo> ipToAsns, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            IEnumerable<NpgsqlParameter> CreateParameters(AsnDescriptionCountryInfo result, int i)
            {
                yield return new NpgsqlParameter($"asn_{i}", result.Asn);
                yield return new NpgsqlParameter($"description_{i}", result.Description);
                yield return new NpgsqlParameter($"country_{i}", result.Country);
            }

            if (ipToAsns.Any())
            {
                string valueFormats = string.Join(",", ipToAsns.Select((v, i) => string.Format(AsnToDescriptionAndCountryDaoResource.InsertAsnToDescriptionAndCountryValueFormatString, i)));

                string commandText =
                    $"{AsnToDescriptionAndCountryDaoResource.InsertAsnToDescriptionAndCountry}{valueFormats} {AsnToDescriptionAndCountryDaoResource.InsertAsnToDescriptionAndCountryOnConflict}";

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
            using (NpgsqlCommand command = new NpgsqlCommand(AsnToDescriptionAndCountryDaoResource.Truncate, connection))
            {
                command.Transaction = transaction;
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task RefreshMaterializedView(NpgsqlConnection connection)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(AsnToDescriptionAndCountryDaoResource.RefreshMaterializedView, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}