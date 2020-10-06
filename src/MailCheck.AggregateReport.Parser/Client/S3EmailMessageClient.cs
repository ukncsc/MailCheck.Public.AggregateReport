using System.Diagnostics;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using MailCheck.AggregateReport.Parser.Config;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Domain.Report;
using MailCheck.Common.Util;
using Microsoft.Extensions.Logging;

namespace MailCheck.AggregateReport.Parser.Client
{
    public interface IS3EmailMessageClient
    {
        Task<EmailMessageInfo> GetEmailMessage(S3SourceInfo s3SourceInfo);
    }

    internal class S3EmailMessageClient : IS3EmailMessageClient
    {
        private readonly IAggregateReportConfig _config;
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<S3EmailMessageClient> _log;

        public S3EmailMessageClient(IAggregateReportConfig config, IAmazonS3 s3Client, ILogger<S3EmailMessageClient> log)
        {
            _config = config;
            _s3Client = s3Client;
            _log = log;
        }

        public async Task<EmailMessageInfo> GetEmailMessage(S3SourceInfo s3SourceInfo)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            GetObjectResponse response = await _s3Client.GetObjectAsync(s3SourceInfo.BucketName, s3SourceInfo.ObjectName)
                .TimeoutAfter(_config.TimeoutS3)
                .ConfigureAwait(false);

            _log.LogDebug($"Retrieving aggregate report from bucket took {stopwatch.Elapsed}");

            stopwatch.Stop();

            string originalUri = $"{s3SourceInfo.BucketName}/{s3SourceInfo.ObjectName}";

            return new EmailMessageInfo(new EmailMetadata(s3SourceInfo.RequestId, s3SourceInfo.MessageId, originalUri, s3SourceInfo.ObjectName, s3SourceInfo.ObjectSize / 1024), response.ResponseStream);
        }
    }
}
