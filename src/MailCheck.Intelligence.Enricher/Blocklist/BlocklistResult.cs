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
        public bool IsInconclusive { get; private set; }
        public string ErrorMessage { get; private set; }

        public static BlocklistResult Inconclusive(string ipAddress, string errorMessage)
        {
            return new BlocklistResult(ipAddress)
            {
                IsInconclusive = true,
                ErrorMessage = errorMessage,
            };
        }
    }
}