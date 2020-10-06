
using MailCheck.AggregateReport.Parser.Domain.Report;

namespace MailCheck.AggregateReport.Parser.Domain
{
    public class AggregateReportInfo 
    {
        public AggregateReportInfo(Dmarc.AggregateReport aggregateReport,
            EmailMetadata emailMetadata,
            AttachmentInfo attachmentInfo)
        {
            AggregateReport = aggregateReport;
            AttachmentInfo = attachmentInfo;
            EmailMetadata = emailMetadata;
        }

        public long Id { get; set; }
        public EmailMetadata EmailMetadata { get; }
        public Dmarc.AggregateReport AggregateReport { get; }
        public AttachmentInfo AttachmentInfo { get; set; }
    }
}