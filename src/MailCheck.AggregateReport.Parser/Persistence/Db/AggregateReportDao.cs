using System;
using System.Threading;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Exceptions;
using MailCheck.AggregateReport.Parser.Mapping;
using MailCheck.Common.Data.Abstractions;
using MySql.Data.MySqlClient;

namespace MailCheck.AggregateReport.Parser.Persistence.Db
{
    internal class AggregateReportDao : IAggregateReportPersistor
    {
        private readonly IConnectionInfoAsync _connectionInfoAsync;
        private readonly IRecordDao _recordDao;

        public AggregateReportDao(IConnectionInfoAsync connectionInfoAsync,
            IRecordDao recordDao)
        {
            _connectionInfoAsync = connectionInfoAsync;
            _recordDao = recordDao;
        }

        public async Task<AggregateReportInfo> Persist(AggregateReportInfo aggregateReport)
        {
            string connectionString = await _connectionInfoAsync.GetConnectionStringAsync();
            
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (MySqlCommand command =
                    new MySqlCommand(AggregateReportDaoResources.InsertAggregateReport, connection))
                {
                    command.Parameters.AddWithValue("request_id", aggregateReport.EmailMetadata.RequestId);
                    command.Parameters.AddWithValue("original_uri", aggregateReport.EmailMetadata.OriginalUri);
                    command.Parameters.AddWithValue("attachment_filename", aggregateReport.AttachmentInfo.AttachmentMetadata.Filename);
                    command.Parameters.AddWithValue("version", aggregateReport.AggregateReport.Version);
                    command.Parameters.AddWithValue("org_name",aggregateReport.AggregateReport?.ReportMetadata?.OrgName);
                    command.Parameters.AddWithValue("email", aggregateReport.AggregateReport?.ReportMetadata?.Email);
                    command.Parameters.AddWithValue("report_id", aggregateReport.AggregateReport?.ReportMetadata?.ReportId);
                    command.Parameters.AddWithValue("extra_contact_info", aggregateReport.AggregateReport?.ReportMetadata?.ExtraContactInfo);
                    command.Parameters.AddWithValue("effective_date", aggregateReport.AggregateReport?.ReportMetadata?.Range.EffectiveDate.ToDateTime());
                    command.Parameters.AddWithValue("begin_date", aggregateReport.AggregateReport?.ReportMetadata?.Range.Begin.ToDateTime());
                    command.Parameters.AddWithValue("end_date", aggregateReport.AggregateReport?.ReportMetadata?.Range.End.ToDateTime());
                    command.Parameters.AddWithValue("domain", aggregateReport.AggregateReport.PolicyPublished?.Domain);
                    command.Parameters.AddWithValue("adkim", aggregateReport.AggregateReport.PolicyPublished?.Adkim.ToString());
                    command.Parameters.AddWithValue("aspf", aggregateReport.AggregateReport.PolicyPublished?.Aspf?.ToString());
                    command.Parameters.AddWithValue("p", aggregateReport.AggregateReport.PolicyPublished?.P.ToString());
                    command.Parameters.AddWithValue("sp", aggregateReport.AggregateReport.PolicyPublished?.Sp?.ToString());
                    command.Parameters.AddWithValue("pct", aggregateReport.AggregateReport.PolicyPublished?.Pct);
                    command.Parameters.AddWithValue("fo", aggregateReport.AggregateReport.PolicyPublished?.Fo);
                    command.Parameters.AddWithValue("created_date", DateTime.UtcNow);

                    int numberOfUpdates = await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);

                    if (numberOfUpdates == 0)
                    {
                        throw new DuplicateAggregateReportException($"Aggregate report {aggregateReport.EmailMetadata.OriginalUri} is a duplicate.");
                    }

                    aggregateReport.Id = command.LastInsertedId;
                }

                await _recordDao.Persist(aggregateReport, connection);

                connection.Close();
            }

            return aggregateReport;
        }
    }
}