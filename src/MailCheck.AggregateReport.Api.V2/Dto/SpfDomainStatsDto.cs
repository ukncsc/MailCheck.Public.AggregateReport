namespace MailCheck.AggregateReport.Api.V2.Dto
{
    public class SpfDomainStatsDto
    {
        public SpfDomainStatsDto(
            string domain, 
            string provider, 
            string ip,
            string spfDomain,
            long spfPass,
            long spfFail,
            string providerAlias,
            string providerMarkdown)
        {
            Domain = domain;
            Provider = provider;
            Ip = ip;
            SpfDomain = spfDomain;
            SpfPass = spfPass;
            SpfFail = spfFail;
            ProviderAlias = providerAlias;
            ProviderMarkdown = providerMarkdown;
        }

        public string Domain { get; }
        public string Provider { get; }
        public string Ip { get; }
        public string SpfDomain { get; }
        public long SpfPass { get; }
        public long SpfFail { get; }
        public string ProviderAlias { get; }
        public string ProviderMarkdown { get; }
    }
}