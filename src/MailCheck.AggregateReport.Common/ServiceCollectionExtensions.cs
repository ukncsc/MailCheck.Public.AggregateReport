using MailCheck.Common.Data;
using MailCheck.AggregateReport.Common.Aggregators;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAggregateReportCommon(this IServiceCollection collection)
        {
            collection.AddSingleton<IDatabase, DefaultDatabase<MySqlProvider>>();
            collection.AddSingleton(typeof(IAggregatorDao<>), typeof(AggregatorDao<>));
            return collection;
        }
    }
}
