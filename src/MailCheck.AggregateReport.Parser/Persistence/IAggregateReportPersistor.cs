using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;

namespace MailCheck.AggregateReport.Parser.Persistence
{
    public interface IAggregateReportPersistor
    {
        Task<AggregateReportInfo> Persist(AggregateReportInfo aggregateReportInfo);
    }
}