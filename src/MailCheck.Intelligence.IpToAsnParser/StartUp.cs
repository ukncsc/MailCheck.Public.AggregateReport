using Amazon.SimpleSystemsManagement;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.SSM;
using MailCheck.Intelligence.IpToAsnParser.Dao;
using MailCheck.Intelligence.IpToAsnParser.Fetchers;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.Intelligence.IpToAsnParser
{
    internal class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IProcess, IpAddressToAsnProcessor>()
                .AddTransient<IAsnToIpAddressParser, AsnToIpAddressParser>()
                .AddTransient<IAsnToIp4AddressFetcher, AsnToIp4AddressFetcher>()
                .AddTransient<IAsnToIp6AddressFetcher, AsnToIp6AddressFetcher>()
                .AddTransient<IIp4ToAsnDao, Ip4ToAsnDao>()
                .AddTransient<IIp6ToAsnDao, Ip6ToAsnDao>()
                .AddTransient<IConnectionInfoAsync, PostgresEnvironmentParameterStoreConnectionInfoAsync>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddEnvironment();
        }
    }
}
