namespace MailCheck.AggregateReport.Api.V2.Dto
{
    public class DkimDomainStatsDto
    {
        public DkimDomainStatsDto(
            string domain, 
            string provider, 
            string ip,
            string dkimDomain,
            string dkimSelector,
            long dkimPass,
            long dkimFail,
            string providerAlias,
            string providerMarkdown)
        {
            Domain = domain;
            Provider = provider;
            Ip = ip;
            DkimDomain = dkimDomain;
            DkimSelector = dkimSelector;
            DkimPass = dkimPass;
            DkimFail = dkimFail;
            ProviderAlias = providerAlias;
            ProviderMarkdown = providerMarkdown;
        }

        public string Domain { get; }
        public string Provider { get; }
        public string Ip { get; }
        public string DkimDomain { get; }
        public string DkimSelector { get; }
        public long DkimPass { get; }
        public long DkimFail { get; }
        public string ProviderAlias { get; }
        public string ProviderMarkdown { get; }
    }
}