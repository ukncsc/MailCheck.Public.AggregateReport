﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MailCheck.AggregateReport.Parser.Client;
using MailCheck.AggregateReport.Parser.Config;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Domain.Report;
using MailCheck.AggregateReport.Parser.Exceptions;
using MailCheck.AggregateReport.Parser.Mapping;
using MailCheck.AggregateReport.Parser.Parser;
using MailCheck.AggregateReport.Parser.Persistence;
using MailCheck.AggregateReport.Parser.Publisher;
using Microsoft.Extensions.Logging;

namespace MailCheck.AggregateReport.Parser.Processor
{
    public interface IS3AggregateReportProcessor
    {
        Task Process(List<S3SourceInfo> s3SourceInfos);
    }

    internal class S3AggregateReportProcessor : IS3AggregateReportProcessor
    {
        private readonly IS3EmailMessageClient _s3Client;
        private readonly IAggregateReportParser _parser;
        private readonly IAggregateReportPersistor _persistor;
        private readonly IAggregateReportMessagePublisherComposite _publisher;
        private readonly IAggregateReportConfig _config;
        private readonly ILogger<S3AggregateReportProcessor> _logger;

        public S3AggregateReportProcessor(
            IS3EmailMessageClient s3Client,
            IAggregateReportParser parser,
            IAggregateReportPersistor persistor,
            IAggregateReportMessagePublisherComposite publisher,
            IAggregateReportConfig config,
            ILogger<S3AggregateReportProcessor> logger)
        {
            _s3Client = s3Client;
            _parser = parser;
            _persistor = persistor;
            _publisher = publisher;
            _config = config;
            _logger = logger;
        }

        public Task Process(List<S3SourceInfo> s3SourceInfos)
        {
            return Task.WhenAll(s3SourceInfos.Select(Process));
        }

        private async Task Process(S3SourceInfo s3SourceInfo)
        {
            using (_logger.BeginScope(new Dictionary<string, object> { 
                ["MessageId"] = s3SourceInfo.MessageId,
                ["RequestId"] = s3SourceInfo.RequestId,
                ["S3ObjectPath"] = $"{ s3SourceInfo.BucketName }/{ s3SourceInfo.ObjectName }"
            }))
            {
                _logger.LogDebug(
                    $"Processing report in s3 object {s3SourceInfo.BucketName}/{s3SourceInfo.ObjectName}, " +
                    $"message Id: {s3SourceInfo.MessageId}, request Id: {s3SourceInfo.RequestId}.");

                EmailMessageInfo emailMessageInfo = null;
                try
                {
                    emailMessageInfo = await _s3Client.GetEmailMessage(s3SourceInfo);

                    _logger.LogDebug(
                        $"Successfully retrieved report in s3 object {s3SourceInfo.BucketName}/{s3SourceInfo.ObjectName}, " +
                        $"message Id: {s3SourceInfo.MessageId}, request Id: {s3SourceInfo.RequestId}.");

                    if (emailMessageInfo.EmailMetadata.FileSizeKb > _config.MaxS3ObjectSizeKilobytes)
                    {
                        _logger.LogWarning($"Didnt process report as size was {emailMessageInfo.EmailMetadata.FileSizeKb} KB and MaxS3ObjectSizeKilobytes of {_config.MaxS3ObjectSizeKilobytes} KB was exceeded");

                        // Need to throw a non-AggregateReportParserException here so that the message 
                        // drops to the next queue to be picked up by the large processor
                        throw new ApplicationException("S3 object too large to process"); 
                    }
                    else
                    {
                        AggregateReportInfo aggregateReportInfo = null;
                        try
                        {
                            aggregateReportInfo = _parser.Parse(emailMessageInfo);
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning(e,
                                $"Bad formatting in attachment {emailMessageInfo.EmailMetadata?.Filename}");
                            throw new AggregateReportFormatException("Exception thrown during parse", e);
                        }
                        finally
                        {
                            emailMessageInfo.EmailStream.Dispose();
                            aggregateReportInfo?.AttachmentInfo.AttachmentStream.Dispose();
                        }

                        _logger.LogDebug(
                            $"Successfully parsed report in s3 object {s3SourceInfo.BucketName}/{s3SourceInfo.ObjectName}, " +
                            $"message Id: {s3SourceInfo.MessageId}, request Id: {s3SourceInfo.RequestId}.");

                        using (_logger.BeginScope(new Dictionary<string, object>
                        {
                            ["EmailAttachmentFileName"] = aggregateReportInfo?.AttachmentInfo?.AttachmentMetadata?.Filename,
                            ["AggregateReportInfoId"] = aggregateReportInfo?.Id,
                            ["AggregateReportId"] = aggregateReportInfo?.AggregateReport?.ReportMetadata?.ReportId,
                            ["AggregateReportOrgName"] = aggregateReportInfo?.AggregateReport?.ReportMetadata?.OrgName,
                            ["AggregateReportDomain"] = aggregateReportInfo?.AggregateReport?.PolicyPublished?.Domain,
                            ["AggregateReportEffectiveDate"] = aggregateReportInfo?.AggregateReport?.ReportMetadata?.Range?.EffectiveDate.ToDateTime().ToString("yyyy-MM-dd"),
                        }))
                        using (TransactionScope transactionScope = new TransactionScope(
                            TransactionScopeOption.Required,
                            new TransactionOptions
                            {
                                IsolationLevel = IsolationLevel.ReadCommitted,
                                Timeout = TimeSpan.FromSeconds(300)
                            },
                            TransactionScopeAsyncFlowOption.Enabled))
                        {
                            string[] errors = aggregateReportInfo?.AggregateReport?.ReportMetadata?.Error ?? Array.Empty<string>();
                            if (errors.Length > 0)
                            {
                                _logger.LogWarning($"Parsed report with errors: {Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
                            }

                            _logger.LogInformation($"Extracted {aggregateReportInfo?.AggregateReport?.Records?.Length ?? 0} records from report");

                            try
                            {
                                aggregateReportInfo = await _persistor.Persist(aggregateReportInfo);
                                    
                                _logger.LogDebug(
                                    $"Successfully persisted report in s3 object {s3SourceInfo.BucketName}/{s3SourceInfo.ObjectName}, " +
                                    $"message Id: {s3SourceInfo.MessageId}, request Id: {s3SourceInfo.RequestId}.");

                                await _publisher.Publish(aggregateReportInfo);

                                _logger.LogDebug(
                                    $"Successfully published report/s in s3 object {s3SourceInfo.BucketName}/{s3SourceInfo.ObjectName}, " +
                                    $"message Id: {s3SourceInfo.MessageId}, request Id: {s3SourceInfo.RequestId}.");

                                transactionScope.Complete();
                            }
                            catch (Exception e) when (LogException(e))
                            {
                            }
                        }
                    }
                    
                }
                catch (AggregateReportParserException)
                {
                }
                finally
                {
                    try
                    {
                        emailMessageInfo?.EmailStream?.Dispose();
                    }
                    catch { }
                }
            }
        }

        private bool LogException(Exception e)
        {
            switch (e)
            {
                case DuplicateAggregateReportException duplicateException:
                    _logger.LogInformation(e, "Duplicate Exception");
                    break;
                case AggregateReportParserException parserException:
                    _logger.LogWarning(e, "Parser Exception");
                    break;
                default:
                    _logger.LogError(e, "Unexpected error occurred");
                    break;
            }

            return false;
        }
    }
}