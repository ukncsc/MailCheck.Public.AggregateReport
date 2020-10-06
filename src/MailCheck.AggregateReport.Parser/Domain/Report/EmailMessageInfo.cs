using System.IO;

namespace MailCheck.AggregateReport.Parser.Domain.Report
{
    public class EmailMessageInfo
    {
        public EmailMessageInfo(
            EmailMetadata emailMetadata,
            Stream emailStream)
        {
            EmailMetadata = emailMetadata;
            EmailStream = emailStream;
        }

        public EmailMetadata EmailMetadata { get; }
        public Stream EmailStream { get; }
    }
}
