using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.SQSEvents;
using Amazon.S3.Util;
using MailCheck.AggregateReport.Parser.Domain;

namespace MailCheck.AggregateReport.Parser.Mapping
{
    public static class SqsEventExtensions
    {
        public static List<S3SourceInfo> ToS3SourceInfos(this SQSEvent sqsEvent, string requestId)
        {
            return sqsEvent.Records
                .Select(_ => Tuple.Create(S3EventNotification.ParseJson(_.Body), _.MessageId))
                .SelectMany(_ => _.Item1.Records.Select(r => Tuple.Create(r, _.Item2)))
                .Select(_ => new S3SourceInfo(_.Item1.S3.Bucket.Name, _.Item1.S3.Object.Key, _.Item1.S3.Object.Size,
                    _.Item2, requestId))
                .ToList();
        }
    }
}