using System.Collections.Generic;
using MailCheck.AggregateReport.Contracts.IpIntelligence;

namespace MailCheck.Intelligence.Enricher.Blocklist
{
    public class BlocklistResult
    {
        public BlocklistResult(string ipAddress, List<BlocklistAppearance> blocklistAppearances = null)
        {
            IpAddress = ipAddress;
            BlocklistAppearances = blocklistAppearances ?? new List<BlocklistAppearance>();
        }

        public string IpAddress { get; }
        public List<BlocklistAppearance> BlocklistAppearances { get; }
    }
}