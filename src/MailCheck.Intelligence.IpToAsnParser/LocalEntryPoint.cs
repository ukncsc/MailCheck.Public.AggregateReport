using System;
using System.Threading.Tasks;
using MailCheck.Common.Messaging;
using Microsoft.Extensions.CommandLineUtils;
using Environment = System.Environment;

namespace MailCheck.Intelligence.IpToAsnParser
{
    public static class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication(false);

            commandLineApplication.Command("lambda", command =>
            {
                command.Description = "Run ip address intelligence  lambda code locally.";

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
            try
            {
                Console.WriteLine($"Running Lambda...");
                LambdaEntryPoint lambdaEntryPoint = new LambdaEntryPoint();
                await lambdaEntryPoint.FunctionHandler(null, LambdaContext.NonExpiringLambda);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured running lambda {e.Message} {Environment.NewLine} {e.StackTrace}");
            }
        }
    }
}