using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DnsClient;
using Louw.PublicSuffix;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment;
using MailCheck.Common.Logging;
using MailCheck.Intelligence.Enricher.Config;
using MailCheck.Intelligence.Enricher.Dns;
using MailCheck.Intelligence.Enricher.ReverseDns;
using MailCheck.Intelligence.Enricher.ReverseDns.Dao;
using MailCheck.Intelligence.Enricher.ReverseDns.Dns;
using MailCheck.Intelligence.Enricher.ReverseDns.Processor;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace MailCheck.Intelligence.Enricher.Test
{
    [TestFixture(Category = "Integration")]
    public class ReverseDnsSmokeTest
    {
        private const string ConnectionString = "User ID = dev_ipintelapi; Pwd=;Host=localhost;Port=5432;Database=ip_intelligence; Pooling=true;";
        private const string Ip = "";

        private IReverseDnsProvider _reverseDnsProvider;

        [SetUp]
        public void SetUp()
        {
            Environment.SetEnvironmentVariable("DnsRecordLookupTimeoutSeconds", "10");

            ILookupClient CreateLookupClient(IServiceProvider provider)
            {
                IPEndPoint[] nameservers = provider.GetService<IDnsNameServerProvider>()
                    .GetNameServers()
                    .Select(_ => new IPEndPoint(_, 53))
                    .ToArray();

                LookupClientOptions options = new LookupClientOptions(nameservers)
                {
                    Timeout = provider.GetRequiredService<IEnricherConfig>().DnsRecordLookupTimeout,
                    UseTcpOnly = true,
                };

                return new LookupClient(options);
            }

            _reverseDnsProvider = new ServiceCollection()
                .AddTransient<IReverseDnsProvider, ReverseDnsProvider>()
                .AddTransient<IReverseDnsLookup, ReverseDnsLookup>()
                .AddTransient<IOrganisationalDomainProvider, OrganisationalDomainProvider>()
                .AddTransient<IPublicSuffixDao, PublicSuffixDao>()
                .AddTransient<IConnectionInfoAsync>(_ => new StringConnectionInfoAsync(ConnectionString))
                .AddTransient<ITldRuleProvider, DaoTldRuleProvider>()
                .AddTransient<IDnsResolver, DnsResolver>()
                .AddTransient<IEnricherConfig, EnricherConfig>()
                .AddTransient(CreateLookupClient)
                .AddEnvironment()
                .AddSerilogLogging()
                .BuildServiceProvider()
                .GetRequiredService<IReverseDnsProvider>();
        }

        [Test]
        public async Task Test()
        {
            List<ReverseDnsResult> reverseDnsResult = await _reverseDnsProvider.GetReverseDnsResult(new List<string> {Ip});
        }
    }
}
