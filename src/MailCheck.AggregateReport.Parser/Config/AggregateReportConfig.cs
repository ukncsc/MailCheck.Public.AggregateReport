using System;
using System.Collections.Generic;
using System.Text;
using MailCheck.Common.Environment.Abstractions;

namespace MailCheck.AggregateReport.Parser.Config
{
    public interface IAggregateReportConfig
    {
        string SnsTopicArn { get; }
        string DkimSelectorsTopicArn { get; }
        string SqsQueueUrl { get; }
        TimeSpan RemainingTimeTheshold { get; }
        TimeSpan TimeoutSqs { get; }
        TimeSpan TimeoutS3 { get; }
        long MaxS3ObjectSizeKilobytes { get; }
    }

    public class AggregateReportConfig : IAggregateReportConfig
    {
        public AggregateReportConfig(IEnvironmentVariables environmentVariables)
        {
            SnsTopicArn = environmentVariables.Get("SnsTopicArn");
            RemainingTimeTheshold = TimeSpan.FromSeconds(environmentVariables.GetAsDouble("RemainingTimeThresholdSeconds"));
            SqsQueueUrl = environmentVariables.Get("SqsQueueUrl");
            DkimSelectorsTopicArn = environmentVariables.Get("DkimSelectorsTopicArn");
            TimeoutSqs = TimeSpan.FromSeconds(environmentVariables.GetAsDouble("TimeoutS3Seconds"));
            TimeoutS3 = TimeSpan.FromSeconds(environmentVariables.GetAsDouble("TimeoutSqsSeconds"));
            MaxS3ObjectSizeKilobytes = environmentVariables.GetAsLong("MaxS3ObjectSizeKilobytes");
        }

        public string SnsTopicArn { get; }
        public string DkimSelectorsTopicArn { get; }
        public string SqsQueueUrl { get; }
        public TimeSpan RemainingTimeTheshold { get; }
        public TimeSpan TimeoutSqs { get; }
        public TimeSpan TimeoutS3 { get; }
        public long MaxS3ObjectSizeKilobytes { get; }
    }
}
