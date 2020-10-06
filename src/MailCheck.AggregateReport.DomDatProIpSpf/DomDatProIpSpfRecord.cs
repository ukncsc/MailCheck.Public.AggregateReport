using MailCheck.AggregateReport.Common.Aggregators;
using System;

namespace MailCheck.AggregateReport.DomDatProIpSpf
{
    public class DomDatProIpSpfRecord : IRecord
    {
        public DomDatProIpSpfRecord(
            long id,
            string domain,
            DateTime date,
            string provider,
            string ip,
            string spfDomain,
            long spfPass,
            long spfFail
            )
        {
            Id = id;
            Domain = domain;
            Date = date;
            Provider = provider;
            Ip = ip;
            SpfDomain = spfDomain;
            SpfPass = spfPass;
            SpfFail = spfFail;
        }
        public DateTime Date { get; }
        public string Provider { get; }
        public long Id { get; }
        public string Domain { get; }
        public string Ip { get; }
        public string SpfDomain { get; }
        public long SpfPass { get; }
        public long SpfFail { get; }
    }
}
