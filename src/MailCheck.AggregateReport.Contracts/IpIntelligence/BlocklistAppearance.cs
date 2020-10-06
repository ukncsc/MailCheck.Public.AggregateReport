namespace MailCheck.AggregateReport.Contracts.IpIntelligence
{
    public class BlocklistAppearance
    {
        public BlocklistAppearance(string flag, string source, string description)
        {
            Flag = flag;
            Source = source;
            Description = description;
        }

        public string Flag { get; }
        public string Source { get; }
        public string Description { get; }
    }
}