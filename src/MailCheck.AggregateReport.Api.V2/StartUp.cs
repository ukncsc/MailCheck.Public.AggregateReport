using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using FluentValidation;
using FluentValidation.AspNetCore;
using MailCheck.AggregateReport.Api.V2.Dao;
using MailCheck.AggregateReport.Api.V2.Services;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Mappers;
using MailCheck.AggregateReport.Api.V2.Provider;
using MailCheck.AggregateReport.Api.V2.Validation;
using MailCheck.Common.Api.Authentication;
using MailCheck.Common.Api.Authorisation.Service;
using MailCheck.Common.Api.Authorisation.Service.Domain;
using MailCheck.Common.Api.Middleware;
using MailCheck.Common.Api.Middleware.Audit;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.Logging;
using MailCheck.Common.SSM;
using MailCheck.Common.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using WebApiContrib.Core.Formatter.Csv;
using MailCheck.Common.Logging.Telemetry;

namespace MailCheck.AggregateReport.Api.V2
{
    public class StartUp
    {
        public StartUp(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () =>
            {
                JsonSerializerSettings serializerSetting = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                };

                return serializerSetting;
            };

            if (RunInDevMode())
            {
                services.AddCors(CorsOptions);
            }

            services
                .AddHealthChecks(checks =>
                    checks.AddValueTaskCheck("HTTP Endpoint", () =>
                        new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok"))))
                .AddTransient<IAggregateReportApiDao, SparseAggregateReportApiDao>()
                .AddTransient<IAggregateReportService, AggregateReportService>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IConnectionInfoAsync, MySqlEnvironmentParameterStoreConnectionInfoAsync>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddTransient<IValidator<DomainDateRangeRequest>, DomainDateRangeRequestValidator>()
                .AddTransient<IValidator<DomainProviderDateRangeRequest>, DomainProviderDateRangeRequestValidator>()
                .AddTransient<IValidator<DomainProviderIpDateRangeRequest>, DomainProviderIpDateRangeRequestValidator>()
                .AddTransient<IValidator<DataExportRequest>, DataExportRequestValidator>()
                .AddTransient<IProviderDetailsProvider, ProviderDetailsProvider>()
                .AddTransient<ISubdomainStatsMapper, SubdomainStatsMapper>()
                .AddTransient< IProviderStatsMapper, ProviderStatsMapper>()
                .AddTransient<IIpStatsMapper, IpStatsMapper>()
                .AddTransient<ISpfDomainStatsMapper, SpfDomainStatsMapper>()
                .AddTransient<IDkimDomainStatsMapper, DkimDomainStatsMapper>()
                .AddTransient<IDomainValidator, DomainValidator>()
                .AddSingleton<IAggregateReportExportStatsFactory, AggregateReportExportStatsFactory>()
                .AddMailCheckAuthenticationClaimsPrincipleClient()
                .AddSerilogLogging()
                .AddAudit("Aggregate-Report-Api-V2")
                .AddControllers(config =>
                {
                    AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                }).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter(new CsvFormatterOptions()
                {
                    CsvDelimiter = ",",
                })))
                .AddFluentValidation();

            services
                .AddAuthorization()
                .AddAuthentication(AuthenticationSchemes.Claims)
              .AddMailCheckClaimsAuthentication();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (RunInDevMode())
            {
                app.UseCors(CorsPolicyName);
            }

            app.UseMiddleware<AuditTimerMiddleware>()
               .UseMiddleware<OidcHeadersToClaimsMiddleware>()
               .UseMiddleware<ApiKeyToClaimsMiddleware>()
               .UseAuthentication()
               .UseMiddleware<AuditLoggingMiddleware>()
               .UseMiddleware<UnhandledExceptionMiddleware>()
               .UseRouting()
               .UseEndpoints(endpoints => {
                   endpoints.MapDefaultControllerRoute();
                   endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
               });

            new TelemetryConfig()
                .InstrumentAspNet(app, "MailCheck.AggregateReport.Api.V2")
                .InstrumentAwsSdk()
                .InstrumentFlurlHttp();
        }

        private bool RunInDevMode()
        {
            bool.TryParse(Environment.GetEnvironmentVariable("DevMode"), out bool isDevMode);
            return isDevMode;
        }

        private static Action<CorsOptions> CorsOptions => options =>
        {
            options.AddPolicy(CorsPolicyName, builder =>
                builder
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        };

        private const string CorsPolicyName = "CorsPolicy";
    }
}