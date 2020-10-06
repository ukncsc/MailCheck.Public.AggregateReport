using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using MailCheck.AggregateReport.Parser.Factory;
using MailCheck.AggregateReport.Parser.Processor;
using MailCheck.Common.Messaging;
using Microsoft.Extensions.CommandLineUtils;

namespace MailCheck.AggregateReport.Parser
{
    public static class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication(false);

            CommandOption directory = commandLineApplication.Option("-d |--directory <directory>", "The directory containing aggregate report emails", CommandOptionType.SingleValue);
            CommandOption xmlDirectory = commandLineApplication.Option("-x |--xml <xmldirectory>", "The directory to write xml aggregate reports to", CommandOptionType.SingleValue);
            CommandOption csvFile = commandLineApplication.Option("-c |--csv <csvfile>", "The csv file to write denormalised records to", CommandOptionType.SingleValue);
            CommandOption sqlFile = commandLineApplication.Option("-s |--sqlite <sqlitefile>", "The sqlite file to write denormalised records to", CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                CommandLineArgs commandLineArgs = new CommandLineArgs(directory.Value(), xmlDirectory.Value(),
                    csvFile.Value(), sqlFile.Value());

                IFileAggregateReportProcessor processor = FileAggregateReportProcessorFactory.Create(commandLineArgs);

                processor.Process(commandLineArgs.Directory.GetFiles().ToList()).GetAwaiter().GetResult();

                return 0;
            });
            
            commandLineApplication.Command("lambda", command =>
            {
                command.Description = "Run aggregate report processor lambda code locally.";

                command.OnExecute(() =>
                {
                    RunLambda().ConfigureAwait(false).GetAwaiter().GetResult();
                    return 0;
                });

            }, false);

            commandLineApplication.Execute(args);
        }

        private static async Task RunLambda()
        {
            AmazonSQSClient client = new AmazonSQSClient();
            LambdaEntryPoint lambdaEntryPoint = new LambdaEntryPoint();
            string queueUrl = Environment.GetEnvironmentVariable("SqsQueueUrl");

            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest(queueUrl)
            {
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 20,
                MessageAttributeNames = new List<string> { "All" },
                AttributeNames = new List<string> { "All" },
            };

            while (true)
            {
                Console.WriteLine($"Polling {queueUrl} for messages...");
                ReceiveMessageResponse receiveMessageResponse = await client.ReceiveMessageAsync(receiveMessageRequest);
                Console.WriteLine($"Received {receiveMessageResponse.Messages.Count} messages from {queueUrl}.");

                if (receiveMessageResponse.Messages.Any())
                {
                    try
                    {
                        Console.WriteLine($"Running Lambda...");
                        SQSEvent sqsEvent = receiveMessageResponse.Messages.ToSqsEvent();
                        await lambdaEntryPoint.FunctionHandler(sqsEvent,
                            LambdaContext.NonExpiringLambda);

                        Console.WriteLine($"Lambda completed");

                        Console.WriteLine($"Deleting messages...");
                        await client.DeleteMessageBatchAsync(queueUrl,
                            sqsEvent.Records.Select(_ => new DeleteMessageBatchRequestEntry
                            {
                                Id = _.MessageId,
                                ReceiptHandle = _.ReceiptHandle
                            }).ToList());

                        Console.WriteLine($"Deleted messages.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An error occured running lambda {e.Message} {Environment.NewLine} {e.StackTrace}");
                    }
                }
            }
        }

        private static SQSEvent ToSqsEvent(this IEnumerable<Message> messages)
        {
            return new SQSEvent
            {
                Records = messages.Select(_ =>
                    new SQSEvent.SQSMessage
                    {
                        MessageAttributes = _.MessageAttributes.ToDictionary(a => a.Key, a =>
                            new SQSEvent.MessageAttribute
                            {
                                StringValue = a.Value.StringValue
                            }),
                        Attributes = _.Attributes,
                        Body = _.Body,
                        MessageId = _.MessageId,
                        ReceiptHandle = _.ReceiptHandle
                    }).ToList()
            };
        }
    }
}
