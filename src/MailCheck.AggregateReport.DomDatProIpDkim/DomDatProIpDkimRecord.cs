using MailCheck.AggregateReport.Common.Aggregators;
using System;

namespace MailCheck.AggregateReport.DomDatProIpDkim
{
    public class DomDatProIpDkimRecord : IRecord
    {
        public DomDatProIpDkimRecord(
            long id,
            string domain,
            DateTime date,
            string provider,
            string ip,
            string dkimDomain,
            string dkimSelector,
            long dkimPass,
            long dkimFail
            )
        {
            Id = id;
            Domain = domain;
            Date = date;
            Provider = provider;
            Ip = ip;
            DkimDomain = dkimDomain;
            DkimSelector = dkimSelector;
            DkimPass = dkimPass;
            DkimFail = dkimFail;
        }
        public DateTime Date { get; }
        public string Provider { get; }
        public long Id { get; }
        public string Domain { get; }
        public string Ip { get; }
        public string DkimDomain { get; }
        public string DkimSelector { get; }
        public long DkimPass { get; }
        public long DkimFail { get; }
    }
}
