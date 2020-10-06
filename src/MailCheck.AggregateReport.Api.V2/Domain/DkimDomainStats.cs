namespace MailCheck.AggregateReport.Api.V2.Domain
{
    public class DkimDomainStats
    {
        public DkimDomainStats(
            string domain, 
            string provider, 
            string ip,
            string dkimDomain,
            string dkimSelector,
            long dkimPass,
            long dkimFail)
        {
            Domain = domain;
            Provider = provider;
            Ip = ip;
            DkimDomain = dkimDomain;
            DkimSelector = dkimSelector;
            DkimPass = dkimPass;
            DkimFail = dkimFail;
        }

        public string Domain { get; }
        public string Provider { get; }
        public string Ip { get; }
        public string DkimDomain { get; }
        public string DkimSelector { get; }
        public long DkimPass { get; }
        public long DkimFail { get; }
    }
}