using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;

namespace MailCheck.Intelligence.Enricher.Blocklist
{
    public interface IBlocklistProvider
    {
        Task<List<BlocklistResult>> GetBlocklistAppearances(List<string> ipAddresses);
    }

    public class BlocklistProvider : IBlocklistProvider
    {
        private readonly IEnumerable<IBlocklistSourceProcessor> _sourceProcessors;

        public BlocklistProvider(IEnumerable<IBlocklistSourceProcessor> sourceProcessors)
        {
            _sourceProcessors = sourceProcessors;;
        }

        public async Task<List<BlocklistResult>> GetBlocklistAppearances(List<string> ipAddresses)
        {
            List<Task<List<BlocklistResult>>> sourceProcessorTasks = _sourceProcessors
                .Select(sourceProcessor => sourceProcessor.ProcessSource(ipAddresses))
                .ToList();

            List<BlocklistResult>[] sourceProcessorResults = await Task.WhenAll(sourceProcessorTasks);

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