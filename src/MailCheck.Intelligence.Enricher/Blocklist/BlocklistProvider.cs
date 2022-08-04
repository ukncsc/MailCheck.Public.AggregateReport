using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Util;

namespace MailCheck.Intelligence.Enricher.Blocklist
{
    public interface IBlocklistProvider
    {
        Task<List<BlocklistResult>> GetBlocklistAppearances(List<string> ipAddresses);
    }

    public class BlocklistProvider : IBlocklistProvider
    {
        private readonly IEnumerable<IBlocklistSourceProcessor> _sourceProcessors;
        private readonly Func<TimeSpan, Task> _delay;
        private const int MaxRequestsInPeriod = 10;
        private static readonly TimeSpan RateLimitPeriod = TimeSpan.FromSeconds(1);

        public BlocklistProvider(IEnumerable<IBlocklistSourceProcessor> sourceProcessors)
            :this (sourceProcessors, null)
        {
        }

        internal BlocklistProvider(IEnumerable<IBlocklistSourceProcessor> sourceProcessors, Func<TimeSpan, Task> delay)
        {
            _sourceProcessors = sourceProcessors;
            _delay = delay ?? Task.Delay;
        }

        public async Task<List<BlocklistResult>> GetBlocklistAppearances(List<string> ipAddresses)
        {
            var sourceProcessorTasks = _sourceProcessors
                .SelectMany(sourceProcessor => sourceProcessor.ProcessSource(ipAddresses))
                .Batch(MaxRequestsInPeriod)
                .Select(async x =>
                {
                    var workTasks = Task.WhenAll(x);
                    var delay = _delay(RateLimitPeriod);
                    await Task.WhenAll(workTasks, delay);
                    return await workTasks;
                });

            var sourceProcessorResults = await Task.WhenAll(sourceProcessorTasks);

            IEnumerable<BlocklistResult> flattenedResults = sourceProcessorResults.SelectMany(x => x);

            IEnumerable<BlocklistResult> groupedResults = flattenedResults.GroupBy(x => x.IpAddress, (ipAddress, blocklistResults) =>
            {
                List<BlocklistAppearance> blocklistAppearances = blocklistResults.SelectMany(x => x.BlocklistAppearances).ToList();
                return new BlocklistResult(ipAddress, blocklistAppearances);
            });

            return groupedResults.ToList();
        }
    }
}