using MailCheck.AggregateReport.Contracts;
using MailCheck.Common.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.AggregateReport.Common.Aggregators
{
    public class StartUpOverride
    {
        public static void EventProcessorOverrides(IServiceCollection services)
        {
            services.Decorate(typeof(IHandle<AggregateReportRecordEnriched>),
                typeof(AggregateReportLoggingHandlerWrapper));
        }
    }
}
