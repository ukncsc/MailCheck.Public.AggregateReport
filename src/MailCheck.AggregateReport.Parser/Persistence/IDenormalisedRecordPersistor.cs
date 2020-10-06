using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;

namespace MailCheck.AggregateReport.Parser.Persistence
{
    public interface IDenormalisedRecordPersistor
    {
        Task Persist(List<DenormalisedRecord> records);
    }
}