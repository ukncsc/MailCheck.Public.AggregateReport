using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Parser.Config;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Mapping;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Util;
using Microsoft.Extensions.Logging;

namespace MailCheck.AggregateReport.Parser.Publisher
{
    internal class DkimSelectorsSeenPublisher : IAggregateReportMessagePublisher
    {
        private const int Concurrency = 8;

        private readonly IMessagePublisher _publisher;
        private readonly IAggregateReportConfig _config;
        private readonly ILogger<DkimSelectorsSeenPublisher> _log;

        public DkimSelectorsSeenPublisher(
            IMessagePublisher publisher, 
            IAggregateReportConfig config, 
            ILogger<DkimSelectorsSeenPublisher> log
            )
        {
            _publisher = publisher;
            _config = config;
            _log = log;
        }
        
        public async Task Publish(AggregateReportInfo aggregateReportInfo)
        {
            List<Message> dkimSelectorsSeenMessages = aggregateReportInfo.ToDkimSelectorsSeenMessages();

            _log.LogDebug($"Publishing {dkimSelectorsSeenMessages.Count} dkim selector seen messages.");

            foreach (IEnumerable<Message> dkimSelectorsSeenMessageBatch in dkimSelectorsSeenMessages.Batch(Concurrency))
            {
                await Task.WhenAll(dkimSelectorsSeenMessageBatch.Select(_ => _publisher.Publish(_, _config.DkimSelectorsTopicArn)));
            }
        }
    }
}