using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using MailCheck.Common.Messaging;
using Microsoft.Extensions.CommandLineUtils;

namespace MailCheck.Intelligence.Enricher
{
    public static class LocalEntryPoint
    {
        private static CancellationTokenSource cancellationTokenSource;

        public static int Main(string[] args)
        {
            cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.Title = "MailCheck.Intelligence.Enricher";
            CommandLineApplication commandLineApplication = new CommandLineApplication(false);
            
            commandLineApplication.OnExecute(async () =>
            {
                await RunLambda();
                return 0;
            });

            return commandLineApplication.Execute(args);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            cancellationTokenSource.Cancel();
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

            while (!cancellationTokenSource.IsCancellationRequested)
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
