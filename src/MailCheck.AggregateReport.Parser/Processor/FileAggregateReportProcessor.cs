using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Domain.Report;
using MailCheck.AggregateReport.Parser.Parser;
using MailCheck.AggregateReport.Parser.Persistence;
using Microsoft.Extensions.Logging;

namespace MailCheck.AggregateReport.Parser.Processor
{
    public interface IFileAggregateReportProcessor
    {
        Task Process(List<FileInfo> fileInfos);
    }

    internal class FileAggregateReportProcessor : IFileAggregateReportProcessor
    {
        private readonly IAggregateReportParser _parser;
        private readonly IAggregateReportPersistorComposite _persistor;
        private readonly ILogger<FileAggregateReportProcessor> _logger;

        public FileAggregateReportProcessor(
            IAggregateReportParser parser,
            IAggregateReportPersistorComposite persistor, 
            ILogger<FileAggregateReportProcessor> logger)
        {
            _parser = parser;
            _persistor = persistor;
            _logger = logger;
        }

        public async Task Process(List<FileInfo> fileInfos)
        {
            foreach (FileInfo fileInfo in fileInfos)
            {
                await Process(fileInfo);
            }
        }

        private async Task Process(FileInfo fileInfo)
        {
            _logger.LogDebug(
                $"Processing file {fileInfo.FullName}.");

            try
            {
                EmailMessageInfo emailMessageInfo = CreateEmailMessageInfo(fileInfo);

                _logger.LogDebug(
                    $"Successfully retrieved report in file {fileInfo.FullName}.");
                
                AggregateReportInfo aggregateReportInfo = _parser.Parse(emailMessageInfo);
                emailMessageInfo.EmailStream.Dispose();

                _logger.LogDebug(
                    $"Successfully parsed report in file {fileInfo.FullName}.");

                await _persistor.Persist(aggregateReportInfo);

                _logger.LogDebug(
                    $"Successfully persisted report in file {fileInfo.FullName}.");

                aggregateReportInfo.AttachmentInfo.AttachmentStream.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to process {fileInfo.Name} with error: {e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }

        private EmailMessageInfo CreateEmailMessageInfo(FileInfo fileInfo)
        {
            return new EmailMessageInfo(new EmailMetadata(fileInfo.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name),
                fileInfo.Length / 1024), File.Open(fileInfo.FullName, FileMode.Open));
        }
    }
}