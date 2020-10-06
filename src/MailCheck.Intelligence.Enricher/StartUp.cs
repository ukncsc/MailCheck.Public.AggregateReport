using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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
                .AddSingleton<IProviderResolver, ProviderResolver.ProviderResolver>()
                .AddTransient<IMessagePublisher, SnsMessagePublisher>();
        }

        private static void RegisterBlocklistSources(IServiceCollection serviceCollection)
        {
            string sources = File.ReadAllText("Blocklist/blockListSources.json");
            List<BlockListSource> blocklistSources = JsonConvert.DeserializeObject<List<BlockListSource>>(sources);

            foreach (BlockListSource blockListSource in blocklistSources)
            {
                serviceCollection.AddTransient<IBlocklistSourceProcessor>(x => new BlocklistSourceProcessor(blockListSource, x.GetRequiredService<ILookupClient>(), x.GetRequiredService<ILogger<BlocklistSourceProcessor>>()));
            }
        }

        private static ILookupClient CreateLookupClient(IServiceProvider provider)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new LookupClient(NameServer.GooglePublicDns, NameServer.GooglePublicDnsIPv6)
                {
                    Timeout = provider.GetRequiredService<IEnricherConfig>().DnsRecordLookupTimeout
                }
                : new LookupClient(provider.GetService<IDnsNameServerProvider>().GetNameServers()
                    .Select(_ => new IPEndPoint(_, 53)).ToArray())
                {
                    Timeout = provider.GetRequiredService<IEnricherConfig>().DnsRecordLookupTimeout,
                    UseTcpOnly = true,
                };
        }

        //private static ILookupClient CreateLookupClient(IServiceProvider serviceProvider)
        //{
        //    bool runningWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        //    if (runningWindows)
        //    {
        //        return new LookupClient(NameServer.GooglePublicDns, NameServer.GooglePublicDnsIPv6)
        //        {
        //            Timeout = serviceProvider.GetRequiredService<IEnricherConfig>().DnsRecordLookupTimeout
        //        };
        //    }

        //    IDnsNameServerProvider dnsNameServerProvider = serviceProvider.GetService<IDnsNameServerProvider>();
        //    TimeSpan dnsRecordLookupTimeout = serviceProvider.GetRequiredService<IEnricherConfig>().DnsRecordLookupTimeout;
        //    List<IPAddress> nameServers = dnsNameServerProvider.GetNameServers();
        //    IPEndPoint[] endPoints = nameServers.Select(_ => new IPEndPoint(_, 53)).ToArray();

        //    return new LookupClient(endPoints)
        //    {
        //        Timeout = dnsRecordLookupTimeout,
        //        UseTcpOnly = true,
        //    };
        //}
    }
}