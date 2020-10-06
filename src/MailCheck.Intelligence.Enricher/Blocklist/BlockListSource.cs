using System.Collections.Generic;

namespace MailCheck.Intelligence.Enricher.Blocklist
{
    public class BlockListSource
    {
        public string Suffix { get; set; }

        public List<BlockListAddressData> Data { get; set; }
    }

    public class BlockListAddressData
    {
        public string IpAddress { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string Flag { get; set; }
    }
}