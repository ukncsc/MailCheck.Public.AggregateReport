using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using MailCheck.AggregateReport.RollupDomainDate.Dao;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.SSM;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.AggregateReport.RollupDomainDate
{
    internal class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .RegisterAggregateReportCommon()
                .AddTransient<RecordHandler>()
                .AddTransient<IDomainDateAggregatorDao, DomainDateAggregatorDao>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IConnectionInfoAsync, MySqlEnvironmentParameterStoreConnectionInfoAsync>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>();

        }
    }
}
