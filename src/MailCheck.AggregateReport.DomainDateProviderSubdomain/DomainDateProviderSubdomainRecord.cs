using MailCheck.AggregateReport.Common.Aggregators;
using System;

namespace MailCheck.AggregateReport.DomainDateProviderSubdomain
{
    public class DomainDateProviderSubdomainRecord : IRecord
    {
        public DomainDateProviderSubdomainRecord(
            long id,
            string domain,
            DateTime date,
            string provider,
            string subdomain,
            long spfPassDkimPassNone,
            long spfPassDkimFailNone,
            long spfFailDkimPassNone,
            long spfFailDkimFailNone,
            long spfPassDkimPassQuarantine,
            long spfPassDkimFailQuarantine,
            long spfFailDkimPassQuarantine,
            long spfFailDkimFailQuarantine,
            long spfPassDkimPassReject,
            long spfPassDkimFailReject,
            long spfFailDkimPassReject,
            long spfFailDkimFailReject)
        {
            Id = id;
            Domain = domain;
            Date = date;
            Provider = provider;
            Subdomain = subdomain;
            SpfPassDkimPassNone = spfPassDkimPassNone;
            SpfPassDkimFailNone = spfPassDkimFailNone;
            SpfFailDkimPassNone = spfFailDkimPassNone;
            SpfFailDkimFailNone = spfFailDkimFailNone;
            SpfPassDkimPassQuarantine = spfPassDkimPassQuarantine;
            SpfPassDkimFailQuarantine = spfPassDkimFailQuarantine;
            SpfFailDkimPassQuarantine = spfFailDkimPassQuarantine;
            SpfFailDkimFailQuarantine = spfFailDkimFailQuarantine;
            SpfPassDkimPassReject = spfPassDkimPassReject;
            SpfPassDkimFailReject = spfPassDkimFailReject;
            SpfFailDkimPassReject = spfFailDkimPassReject;
            SpfFailDkimFailReject = spfFailDkimFailReject;
        }
        public DateTime Date { get; }
        public string Provider { get; }
        public long Id { get; }
        public string Domain { get; }
        public string Subdomain { get; }
        public long SpfPassDkimPassNone { get; }
        public long SpfPassDkimFailNone { get; }
        public long SpfFailDkimPassNone { get; }
        public long SpfFailDkimFailNone { get; }
        public long SpfPassDkimPassQuarantine { get; }
        public long SpfPassDkimFailQuarantine { get; }
        public long SpfFailDkimPassQuarantine { get; }
        public long SpfFailDkimFailQuarantine { get; }
        public long SpfPassDkimPassReject { get; }
        public long SpfPassDkimFailReject { get; }
        public long SpfFailDkimPassReject { get; }
        public long SpfFailDkimFailReject { get; }
    }
}
