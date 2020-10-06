using MailCheck.AggregateReport.Common.Aggregators;
using System;

namespace MailCheck.AggregateReport.RollupDomainDate
{
    public class DomainDateRecord : IRecord
    {
        public DomainDateRecord(
            long id,
            string domain, 
            DateTime date, 
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

        public long Id { get; }
        public string Domain { get; }
        public DateTime Date { get; }
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