using System.Collections.Generic;

namespace MailCheck.Intelligence.Enricher.Blocklist
{
    public class BlocklistSource
    {
        public string Suffix { get; set; }

        public List<BlocklistAddressData> Data { get; set; }
    }

    public class BlocklistAddressData
    {
        public string IpAddress { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string Flag { get; set; }
    }
}