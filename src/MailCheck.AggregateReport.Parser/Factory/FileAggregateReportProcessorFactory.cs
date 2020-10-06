using MailCheck.AggregateReport.Parser.Compression;
using MailCheck.AggregateReport.Parser.Parser;
using MailCheck.AggregateReport.Parser.Persistence;
using MailCheck.AggregateReport.Parser.Persistence.File;
using MailCheck.AggregateReport.Parser.Processor;
using MailCheck.AggregateReport.Parser.Serialisation;
using MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation;
using MailCheck.Common.Logging;
using MailCheck.Common.Util;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.AggregateReport.Parser.Factory
{
    public static class FileAggregateReportProcessorFactory
    {
        public static IFileAggregateReportProcessor Create(ICommandLineArgs commandLineArgs)
        {
            return new ServiceCollection()
                .AddSingleton(commandLineArgs)
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
                .AddTransient<IAggregateReportPersistorComposite, AggregateReportPersistorComposite>()
                .AddTransientConditional<IAggregateReportPersistor, DenormalisedRecordPersistorAdaptor>(commandLineArgs.CsvFile != null || commandLineArgs.SqlFile != null)
                .AddTransientConditional<IDenormalisedRecordPersistorComposite, DenormalisedRecordPersistorComposite>(commandLineArgs.CsvFile != null || commandLineArgs.SqlFile != null)
                .AddTransientConditional<ICsvDenormalisedRecordSerialiser, CsvDenormalisedRecordSerialiser>(commandLineArgs.CsvFile != null)
                .AddTransientConditional<IAggregateReportPersistor, XmlAttachmentPersistor>(commandLineArgs.XmlDirectory != null)
                .AddTransientConditional<IDenormalisedRecordPersistor, CsvPersistor>(commandLineArgs.CsvFile != null)
                .AddTransientConditional<IDenormalisedRecordPersistor, SqlLitePersistor>(commandLineArgs.SqlFile != null)
                .AddSerilogLogging()
                .BuildServiceProvider()
                .GetRequiredService<IFileAggregateReportProcessor>();
        }

        public static IServiceCollection AddTransientConditional<TInterface, TImplementation>(
            this IServiceCollection serviceCollection,
            bool condition)
        {
            if (condition)
            {
                serviceCollection.AddTransient(typeof(TInterface), typeof(TImplementation));
            }

            return serviceCollection;
        }
    }
}