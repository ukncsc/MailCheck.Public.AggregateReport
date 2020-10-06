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
using MailCheck.Intelligence.Enricher.ReverseDns;
using MailCheck.Intelligence.Enricher.ReverseDns.Dao;
using MailCheck.Intelligence.Enricher.ReverseDns.Dns;
using MailCheck.Intelligence.Enricher.ReverseDns.Processor;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MailCheck.Intelligence.Enricher.Migration
{
    internal static class MigratorFactory
    {
        public static IMigrator Create()
        {
            return new ServiceCollection()
                .AddTransient<IIpAddressDetailsDao, IpAddressDetailsDao>()
                .AddTransient<IMigrator, Migrator>()
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
                .AddTransient<IDnsNameServerProvider, LinuxDnsNameServerProvider>()
                .AddTransient(CreateLookupClient)
                .AddTransient<IBlocklistProvider, BlocklistProvider>()
                .AddTransient<IClock, Clock>()
                .AddTransient<IEnricherConfig, EnricherConfig>()
                .AddTransient<IMessagePublisher, SnsMessagePublisher>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .RegisterBlocklistSources()
                .AddSerilogLogging()
                .BuildServiceProvider()
                .GetRequiredService<IMigrator>();
        }

        private static IServiceCollection RegisterBlocklistSources(this IServiceCollection serviceCollection)
        {
            string sources = File.ReadAllText("Blocklist/blockListSources.json");
            List<BlockListSource> blocklistSources = JsonConvert.DeserializeObject<List<BlockListSource>>(sources);

            foreach (BlockListSource blockListSource in blocklistSources)
            {
                serviceCollection.AddTransient<IBlocklistSourceProcessor>(x => new BlocklistSourceProcessor(blockListSource, x.GetRequiredService<ILookupClient>(), x.GetRequiredService<ILogger<BlocklistSourceProcessor>>()));
            }

            return serviceCollection;
        }

        private static ILookupClient CreateLookupClient(IServiceProvider serviceProvider)
        {
            bool runningWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (runningWindows)
            {
                //return new LookupClient(NameServer.GooglePublicDns, NameServer.GooglePublicDnsIPv6)
                return new LookupClient()
                {
                    Timeout = serviceProvider.GetRequiredService<IEnricherConfig>().DnsRecordLookupTimeout
                };
            }

            IDnsNameServerProvider dnsNameServerProvider = serviceProvider.GetService<IDnsNameServerProvider>();
            TimeSpan dnsRecordLookupTimeout = serviceProvider.GetRequiredService<IEnricherConfig>().DnsRecordLookupTimeout;
            List<IPAddress> nameServers = dnsNameServerProvider.GetNameServers();
            IPEndPoint[] endPoints = nameServers.Select(_ => new IPEndPoint(_, 53)).ToArray();

            return new LookupClient(endPoints)
            {
                Timeout = dnsRecordLookupTimeout,
                UseTcpOnly = true,
            };
        }
    }
}