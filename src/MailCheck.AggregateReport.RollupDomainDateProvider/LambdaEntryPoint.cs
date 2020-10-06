using Amazon.Lambda.Core;
using MailCheck.AggregateReport.RollupDomainDateProvider;
using MailCheck.Common.Messaging.Sqs;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

// ReSharper disable once CheckNamespace
namespace MailCheck.RollupDomainDateProvider
{
    public class LambdaEntryPoint : SqsTriggeredLambdaEntryPoint
    {
        public LambdaEntryPoint() : base(new StartUp())
        {
        }
    }
}
