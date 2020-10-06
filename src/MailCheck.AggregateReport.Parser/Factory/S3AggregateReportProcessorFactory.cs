using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using MailCheck.AggregateReport.Parser.Client;
using MailCheck.AggregateReport.Parser.Compression;
using MailCheck.AggregateReport.Parser.Config;
using MailCheck.AggregateReport.Parser.Parser;
using MailCheck.AggregateReport.Parser.Persistence;
using MailCheck.AggregateReport.Parser.Persistence.Db;
using MailCheck.AggregateReport.Parser.Processor;
using MailCheck.AggregateReport.Parser.Publisher;
using MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.Logging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Messaging.Sns;
using MailCheck.Common.SSM;
using MailCheck.Common.Util;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.AggregateReport.Parser.Factory
{
    internal static class S3AggregateReportProcessorFactory
    {
        public static IS3AggregateReportProcessor Create()
        {
            return new ServiceCollection()
                .AddTransient<IS3AggregateReportProcessor, S3AggregateReportProcessor>()
                .AddTransient<IAmazonS3, AmazonS3Client>()
                .AddTransient <IS3EmailMessageClient,S3EmailMessageClient>()
                .AddTransient<IConnectionInfoAsync, MySqlEnvironmentParameterStoreConnectionInfoAsync>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddTransient<IAggregateReportConfig, AggregateReportConfig>()
                .AddTransient<IMessagePublisher, SnsMessagePublisher>()
                .AddTransient<IFileAggregateReportProcessor, FileAggregateReportProcessor>()
                .AddTransient<IAggregateReportParser, AggregateReportParser>()
                .AddTransient<IMimeMessageFactory, MimeMessageFactory>()
                .AddTransient<IAttachmentStreamNormaliser, AttachmentStreamNormaliser>()
                .AddTransient<IAggregateReportDeserialiser, AggregateReportDeserialiser>()
                .AddTransient<IContentTypeProvider, ContentTypeProvider>()
                .AddTransient<IGZipDecompressor, GZipDecompressor>()
                .AddTransient<IZipDecompressor, ZipDecompressor>()
                .AddTransient<IReportMetadataDeserialiser, ReportMetadataDeserialiser>()
                .AddTransient<IPolicyPublishedDeserialiser, PolicyPublishedDeserialiser>()
                .AddTransient<IRecordDeserialiser, RecordDeserialiser>()
                .AddTransient<IDomainValidator, DomainValidator>()
                .AddTransient<IRowDeserialiser, RowDeserialiser>()
                .AddTransient<IIdentifiersDeserialiser, IdentifiersDeserialiser>()
                .AddTransient<IAuthResultDeserialiser, AuthResultDeserialiser>()
                .AddTransient<IPolicyEvaluatedDeserialiser, PolicyEvaluatedDeserialiser>()
                .AddTransient<IPolicyOverrideReasonDeserialiser, PolicyOverrideReasonDeserialiser>()
                .AddTransient<IDkimAuthResultDeserialiser, DkimAuthResultDeserialiser>()
                .AddTransient<ISpfAuthResultDeserialiser, SpfAuthResultDeserialiser>()
                .AddTransient<ISpfAuthResultDao, SpfAuthResultDao>()
                .AddTransient<IDkimAuthResultDao, DkimAuthResultDao>()
                .AddTransient<IPolicyOverrideReasonDao, PolicyOverrideReasonDao>()
                .AddTransient<IRecordDao , RecordDao>()
                .AddTransient<IAggregateReportPersistor, AggregateReportDao>()
                .AddTransient<IAggregateReportMessagePublisher, DkimSelectorsSeenPublisher>()
                .AddTransient<IAggregateReportMessagePublisher, AggregateReportRecordBatchPublisher>()
                .AddTransient<IAggregateReportMessagePublisherComposite, AggregateReportMessagePublisherComposite>()
                .AddSerilogLogging()
                .BuildServiceProvider()
                .GetRequiredService<IS3AggregateReportProcessor>();
        }
    }
}