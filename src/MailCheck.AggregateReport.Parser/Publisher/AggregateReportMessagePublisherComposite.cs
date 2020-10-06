using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Domain;

namespace MailCheck.AggregateReport.Parser.Publisher
{
    public interface IAggregateReportMessagePublisherComposite : IAggregateReportMessagePublisher{}

    internal class AggregateReportMessagePublisherComposite : IAggregateReportMessagePublisherComposite
    {
        private readonly IEnumerable<IAggregateReportMessagePublisher> _publishers;

        public AggregateReportMessagePublisherComposite(IEnumerable<IAggregateReportMessagePublisher> publishers)
        {
            _publishers = publishers;
        }

        public Task Publish(AggregateReportInfo aggregateReportInfo)
        {
            return Task.WhenAll(_publishers.Select(_ => _.Publish(aggregateReportInfo)));
        }
    }
}