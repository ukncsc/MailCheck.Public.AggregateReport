using Amazon.SimpleSystemsManagement;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment;
using MailCheck.Common.Logging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.SSM;
using MailCheck.Intelligence.PublicSuffixParser.Dao;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.Intelligence.PublicSuffixParser
{
    internal class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IProcess, PublicSuffixProcessor>()
                .AddTransient<IPublicSuffixFetcher, PublicSuffixFetcher>()
                .AddTransient<IPublicSuffixParser, PublicSuffixParser>()
                .AddTransient<IPublicSuffixDao, PublicSuffixDao>()
                .AddTransient<IConnectionInfoAsync, PostgresEnvironmentParameterStoreConnectionInfoAsync>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddEnvironment();
        }
    }
}
