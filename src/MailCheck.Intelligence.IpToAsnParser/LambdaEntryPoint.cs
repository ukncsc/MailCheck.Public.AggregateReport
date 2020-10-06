using Amazon.Lambda.Core;
using MailCheck.Common.Messaging.CloudWatch;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MailCheck.Intelligence.IpToAsnParser
{
    public class LambdaEntryPoint : CloudWatchTriggeredLambdaEntryPoint
    {
        public LambdaEntryPoint() : base(new StartUp())
        {
        }
    }
}
