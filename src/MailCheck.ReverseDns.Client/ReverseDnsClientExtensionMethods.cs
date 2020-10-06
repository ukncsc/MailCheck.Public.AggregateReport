using System;
using Amazon.SimpleSystemsManagement;
using MailCheck.Common.Api.AuthProviders;
using MailCheck.Common.Api.Service;
using MailCheck.Common.Environment;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.SSM;
using MailCheck.Common.Util;
using MailCheck.ReverseDns.Client.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MailCheck.ReverseDns.Client
{
    public static class ReverseDnsClientExtensionMethods
    {
        public static IServiceCollection AddReverseDnsClaimsPrincipalClient(this IServiceCollection serviceContainer)
        {
            return serviceContainer
                .AddEnvironment()
                .AddTransient<IReverseDnsClientConfig, ReverseDnsClientConfig>()
                .AddTransient<IReverseDnsClient, ReverseDnsClient>()
                .AddHttpContextAccessor()
                .AddTransient<IAuthenticationHeaderProvider, ClaimsPrincipalAuthenticationHeaderProvider>();
        }

        public static IServiceCollection AddReverseDnsApiKeyClient(this IServiceCollection serviceContainer)
        {
            return serviceContainer
                .AddEnvironment()
                .AddTransient<IReverseDnsClientConfig, ReverseDnsClientConfig>()
                .AddTransient<IReverseDnsClient>(ApiKeyReverseDnsClientFactory)
                .AddTransient<IReverseDnsApiKeyConfig, ReverseDnsApiKeyConfig>()
                .TryAddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .TryAddTransient<IApiKeyProvider, ApiKeyProvider>();
        }

        //use factory to prevent ambiguous reference to IAuthenticationHeaderProvider as each client
        //requires a different value for api key name, if multiple of these are reg'd could cause problems
        private static ReverseDnsClient ApiKeyReverseDnsClientFactory(IServiceProvider serviceProvider)
        {
            IReverseDnsClientConfig reverseDnsClientConfig = serviceProvider.GetRequiredService<IReverseDnsClientConfig>();
            ILogger<ReverseDnsClient> logger = serviceProvider.GetRequiredService<ILogger<ReverseDnsClient>>();

            IReverseDnsApiKeyConfig reverseDnsApiKeyConfig = serviceProvider.GetRequiredService<IReverseDnsApiKeyConfig>();
            IApiKeyProvider apiKeyProvider = serviceProvider.GetRequiredService<IApiKeyProvider>();

            ApiKeyAuthenticationHeaderProvider reverseDnsApiKeyAuthenticationHeaderProvider = 
                new ApiKeyAuthenticationHeaderProvider(reverseDnsApiKeyConfig.ReverseDnsClaimsName, apiKeyProvider);

            return new ReverseDnsClient(reverseDnsClientConfig, reverseDnsApiKeyAuthenticationHeaderProvider, logger);
        }
    }
}