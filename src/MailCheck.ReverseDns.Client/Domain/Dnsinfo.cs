using System.Collections.Generic;

namespace MailCheck.ReverseDns.Client.Domain
{
    public class DnsInfo
    {
        public DnsInfo(string host, IList<string> ipAddresses)
        {
            Host = host;
            IpAddresses = ipAddresses ?? new List<string>();
        }

        public string Host { get; }

        public IList<string> IpAddresses { get; }
    }
}