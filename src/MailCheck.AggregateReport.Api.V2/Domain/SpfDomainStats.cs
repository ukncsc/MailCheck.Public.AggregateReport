namespace MailCheck.AggregateReport.Api.V2.Domain
{
    public class SpfDomainStats
    {
        public SpfDomainStats(
            string domain, 
            string provider, 
            string ip,
            string spfDomain,
            long spfPass,
            long spfFail)
        {
            Domain = domain;
            Provider = provider;
            Ip = ip;
            SpfDomain = spfDomain;
            SpfPass = spfPass;
            SpfFail = spfFail;
        }

        public string Domain { get; }
        public string Provider { get; }
        public string Ip { get; }
        public string SpfDomain { get; }
        public long SpfPass { get; }
        public long SpfFail { get; }
    }
}