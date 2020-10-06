using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using DnsClient;
using Louw.PublicSuffix;
using MailCheck.AggregateReport.Contracts.Migration;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Messaging.Sns;
using MailCheck.Common.SSM;
using MailCheck.Common.Util;
using MailCheck.Intelligence.Enricher;
using MailCheck.Intelligence.Enricher.Asn;
using MailCheck.Intelligence.Enricher.Blocklist;
using MailCheck.Intelligence.Enricher.Config;
using MailCheck.Intelligence.Enricher.Dao;
using MailCheck.Intelligence.Enricher.Dns;
using MailCheck.Intelligence.Enricher.Migration;
using MailCheck.Intelligence.Enricher.ReverseDns;
using MailCheck.Intelligence.Enricher.ReverseDns.Dao;
using MailCheck.Intelligence.Enricher.ReverseDns.Dns;
using MailCheck.Intelligence.Enricher.ReverseDns.Processor;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.Intelligence.AsnBackfiller
{
    internal class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
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
                .AddTransient<IBlocklistProvider, BlocklistProvider>()
                .AddTransient<IClock, Clock>()
                .AddTransient<IEnricherConfig, EnricherConfig>()
                .AddTransient<IMessagePublisher, SnsMessagePublisher>()
                .AddTransient<IHandle<AsnBackfillBatch>, AsnBackfillLambda>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();
        }
    }
}
