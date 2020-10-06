using System;
using System.Collections.Generic;

namespace MailCheck.ReverseDns.Client.Domain
{
    public class ReverseDnsInfoRequest
    {
        public ReverseDnsInfoRequest(List<string> ipAddresses, DateTime date)
        {
            IpAddresses = ipAddresses;
            Date = date;
        }

        public List<string> IpAddresses { get; }

        public DateTime Date { get; }
    }
}