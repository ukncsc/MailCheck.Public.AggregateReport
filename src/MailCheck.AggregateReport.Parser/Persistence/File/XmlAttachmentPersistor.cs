using System.IO;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;

namespace MailCheck.AggregateReport.Parser.Persistence.File
{
    internal class XmlAttachmentPersistor : IAggregateReportPersistor
    {
        private readonly ICommandLineArgs _commandLineArgs;

        public XmlAttachmentPersistor(ICommandLineArgs commandLineArgs)
        {
            _commandLineArgs = commandLineArgs;
        }

        public Task<AggregateReportInfo> Persist(AggregateReportInfo aggregateReportInfo)
        {
            if (!_commandLineArgs.XmlDirectory.Exists)
            {
                _commandLineArgs.XmlDirectory.Create();
            }

            using (FileStream fileStream = System.IO.File.Create($"{_commandLineArgs.XmlDirectory.FullName}/{aggregateReportInfo.AttachmentInfo.AttachmentMetadata.Filename}.xml"))
            {
                aggregateReportInfo.AttachmentInfo.AttachmentStream.Seek(0, SeekOrigin.Begin);
                aggregateReportInfo.AttachmentInfo.AttachmentStream.CopyTo(fileStream);
            }

            return Task.FromResult(aggregateReportInfo);
        }
    }
}