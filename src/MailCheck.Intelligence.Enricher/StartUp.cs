using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using DnsClient;
using Louw.PublicSuffix;
using MailCheck.AggregateReport.Contracts;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.Logging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Messaging.Sns;
using MailCheck.Common.SSM;
using MailCheck.Common.Util;
using MailCheck.Intelligence.Enricher.Asn;
using MailCheck.Intelligence.Enricher.Blocklist;
using MailCheck.Intelligence.Enricher.Config;
using MailCheck.Intelligence.Enricher.Dao;
using MailCheck.Intelligence.Enricher.Dns;
using MailCheck.Intelligence.Enricher.ProviderResolver;
using MailCheck.Intelligence.Enricher.ReverseDns;
using MailCheck.Intelligence.Enricher.ReverseDns.Dao;
using MailCheck.Intelligence.Enricher.ReverseDns.Dns;
using MailCheck.Intelligence.Enricher.ReverseDns.Processor;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MailCheck.Intelligence.Enricher
{
    internal class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterBlocklistSources(services);

            services
                .AddTransient<IIpAddressDetailsDao, IpAddressDetailsDao>()
                .AddTransient<IIpAddressProcessor, IpAddressProcessor>()
                .AddTransient<IConnectionInfoAsync, PostgresEnvironmentParameterStoreConnectionInfoAsync>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IAsnDao, AsDao>()
                .AddTransient<IIpAddressLookup, IpAddressLookup>()
                .AddTransient<IAsInfoProvider, AsInfoProvider>()
                .AddTransient<IReverseDnsProvider, ReverseDnsProvider>()
                .AddTransient<IAuditTrailParser, AuditTrailParser>()
                .AddTransient<IReverseDnsLookup, ReverseDnsLookup>()
                .AddTransient<IOrganisationalDomainProvider, OrganisationalDomainProvider>()
                .AddTransient<IPublicSuffixDao, PublicSuffixDao>()
                .AddTransient<ITldRuleProvider, DaoTldRuleProvider>()
                .AddTransient<IDnsResolver, DnsResolver>()
                .AddSingleton<IDnsNameServerProvider, LinuxDnsNameServerProvider>()
                .AddSingleton(CreateLookupClient)
                .AddTransient<IBlocklistProvider, BlocklistProvider>()
                .AddTransient<IClock, Clock>()
                .AddTransient<IEnricherConfig, EnricherConfig>()
                .AddTransient<IHandle<AggregateReportRecordBatch>, AggregateReportEnricher>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddTransient<IAggregateReportRecordEnrichedFactory, AggregateReportRecordEnrichedFactory>()
                .AddSingleton<IProviderResolver, ProviderResolver.ProviderResolver>();
        }

        private static void RegisterBlocklistSources(IServiceCollection serviceCollection)
        {
            string sources = File.ReadAllText("Blocklist/blocklistSources.json");
            List<BlocklistSource> blocklistSources = JsonConvert.DeserializeObject<List<BlocklistSource>>(sources);

            foreach (BlocklistSource blockListSource in blocklistSources)
            {
                serviceCollection.AddTransient<IBlocklistSourceProcessor>(x => new BlocklistSourceProcessor(blockListSource, x.GetRequiredService<ILookupClient>(), x.GetRequiredService<ILogger<BlocklistSourceProcessor>>()));
            }
        }

        private static ILookupClient CreateLookupClient(IServiceProvider provider)
        {
            IPEndPoint[] nameServers = provider
                .GetService<IDnsNameServerProvider>()
                .GetNameServers()
                .Select(_ => new IPEndPoint(_, 53))
                .ToArray();

            LookupClientOptions options = new LookupClientOptions(nameServers)
            {
                Timeout = provider.GetRequiredService<IEnricherConfig>().DnsRecordLookupTimeout,
                UseTcpOnly = true,
                ContinueOnDnsError = false,
                ContinueOnEmptyResponse = false,
                EnableAuditTrail = true,
                UseCache = false,
            };

            LookupClient lookupClient = new LookupClient(options);

            return new AuditTrailLoggingLookupClientWrapper(lookupClient, provider.GetService<IAuditTrailParser>(), provider.GetService<ILogger<AuditTrailLoggingLookupClientWrapper>>());
        }
    }
}