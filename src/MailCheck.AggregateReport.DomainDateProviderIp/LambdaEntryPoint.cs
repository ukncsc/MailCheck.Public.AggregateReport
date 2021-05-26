using Amazon.Lambda.Core;
using MailCheck.AggregateReport.Common.Aggregators;
using MailCheck.AggregateReport.DomainDateProviderIp;
using MailCheck.Common.Messaging.Sqs;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

//DO NOT UPDATE THIS NAMESPACE AS IT WILL BE TOO LONG
namespace MailCheck.DomainDateProviderIp
{
    public class LambdaEntryPoint : SqsTriggeredLambdaEntryPoint
    {
        public LambdaEntryPoint() : base(new StartUp(), StartUpOverride.EventProcessorOverrides)
        {
        }
    }
}
