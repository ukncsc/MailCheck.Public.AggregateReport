using MailCheck.AggregateReport.Common.Aggregators;
using System;

namespace MailCheck.AggregateReport.RollupDomainDateProvider
{
    public class DomainDateProviderRecord : IRecord
    {
        public DomainDateProviderRecord(
            long id,
            string domain,
            DateTime date,
            string provider,
            string originalProvider,
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
            OriginalProvider = originalProvider;
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
        public string OriginalProvider { get; }
        public long Id { get; }
        public string Domain { get; }
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
