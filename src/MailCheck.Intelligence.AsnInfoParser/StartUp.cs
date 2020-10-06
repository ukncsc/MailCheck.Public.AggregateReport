using Amazon.SimpleSystemsManagement;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment;
using MailCheck.Common.Logging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.SSM;
using MailCheck.Intelligence.AsnInfoParser.Dao;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.Intelligence.AsnInfoParser
{
    internal class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IProcess, AsnToDescriptionAndCountryProcessor>()
                .AddTransient<IAsnToDescriptionAndCountryFetcher, AsnToDescriptionAndCountryFetcher>()
                .AddTransient<IAsnToDescriptionAndCountryParser, AsnToDescriptionAndCountryParser>()
                .AddTransient<IAsnToDescriptionAndCountryDao, AsnToDescriptionAndCountryDao>()
                .AddTransient<IConnectionInfoAsync, PostgresEnvironmentParameterStoreConnectionInfoAsync>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddEnvironment();
        }
    }
}
