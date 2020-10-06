using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;

namespace MailCheck.AggregateReport.Parser.Publisher
{
    public interface IAggregateReportMessagePublisher
    {
        Task Publish(AggregateReportInfo aggregateReportInfo);
    }
}