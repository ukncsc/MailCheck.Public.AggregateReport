using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Factory;
using MailCheck.AggregateReport.Parser.Mapping;
using MailCheck.AggregateReport.Parser.Processor;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MailCheck.AggregateReport.Parser
{
    public class LambdaEntryPoint
    {
        private readonly IS3AggregateReportProcessor _processor;

        public LambdaEntryPoint()
        {
            _processor = S3AggregateReportProcessorFactory.Create();
        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            List<S3SourceInfo> sourceInfos = evnt.ToS3SourceInfos(context.AwsRequestId);

            await _processor.Process(sourceInfos);
        }
    }
}
