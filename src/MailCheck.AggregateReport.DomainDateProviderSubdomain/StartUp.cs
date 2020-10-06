using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.DomainDateProviderSubdomain.Dao;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.SSM;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MailCheck.AggregateReport.DomainDateProviderSubdomain
{
    internal class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            services
                .RegisterAggregateReportCommon()
                .AddTransient<RecordHandler>()
                .AddTransient<IDateDomainProviderSubdomainAggregatorDao, DateDomainProviderSubdomainAggregatorDao>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IConnectionInfoAsync, MySqlEnvironmentParameterStoreConnectionInfoAsync>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>();
        }
    }
}
