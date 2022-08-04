using System;
using System.Collections.Generic;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.Contracts
{
    public class AggregateReportRecord : Message
    {
        public AggregateReportRecord(
            string id,
            string reporterOrgName,
            string reportId,
            DateTime effectiveDate,
            string domainFrom,
            Alignment? adkim,
            Alignment? aspf,
            Policy p,
            Policy? sp,
            int? pct,
            string fo,
            string hostSourceIp,
            int count,
            Policy? disposition,
            DmarcResult? dkim,
            DmarcResult? spf,
            string envelopeTo,
            string envelopeFrom,
            string headerFrom,
            List<string> spfAuthResults,
            int spfPassCount,
            int spfFailCount,
            List<string> dkimAuthResults,
            int dkimPassCount,
            int dkimFailCount,
            bool forwarded,
            bool sampledOut,
            bool trustedForwarder,
            bool mailingList,
            bool localPolicy,
            bool arc,
            bool otherOverrideReason) : base(id)
        {
            ReporterOrgName = reporterOrgName;
            ReportId = reportId;
            EffectiveDate = effectiveDate;
            DomainFrom = domainFrom;
            Adkim = adkim;
            Aspf = aspf;
            P = p;
            Sp = sp;
            Pct = pct;
            Fo = fo;
            HostSourceIp = hostSourceIp;
            Count = count;
            Disposition = disposition;
            Dkim = dkim;
            Spf = spf;
            EnvelopeTo = envelopeTo;
            EnvelopeFrom = envelopeFrom;
            HeaderFrom = headerFrom;
            SpfAuthResults = spfAuthResults;
            SpfPassCount = spfPassCount;
            SpfFailCount = spfFailCount;
            DkimAuthResults = dkimAuthResults;
            DkimPassCount = dkimPassCount;
            DkimFailCount = dkimFailCount;
            Forwarded = forwarded;
            SampledOut = sampledOut;
            TrustedForwarder = trustedForwarder;
            MailingList = mailingList;
            LocalPolicy = localPolicy;
            Arc = arc;
            OtherOverrideReason = otherOverrideReason;
        }

        public string ReporterOrgName { get; }
        public string ReportId { get; }
        public DateTime EffectiveDate { get; }
        public string DomainFrom { get; }
        public Alignment? Adkim { get; }
        public Alignment? Aspf { get; }
        public Policy P { get; }
        public Policy? Sp { get; }
        public int? Pct { get; }
        public string Fo { get; }
        public string HostSourceIp { get; }
        public int Count { get; }
        public Policy? Disposition { get; }
        public DmarcResult? Dkim { get; }
        public DmarcResult? Spf { get; }
        public string EnvelopeTo { get; }
        public string EnvelopeFrom { get; }
        public string HeaderFrom { get; }
        public List<string> SpfAuthResults { get; }
        public int SpfPassCount { get; }
        public int SpfFailCount { get; }
        public List<string> DkimAuthResults { get; }
        public int DkimPassCount { get; }
        public int DkimFailCount { get; }
        public bool Forwarded { get; }
        public bool SampledOut { get; }
        public bool TrustedForwarder { get; }
        public bool MailingList { get; }
        public bool LocalPolicy { get; }

        public bool Arc { get; }
        public bool OtherOverrideReason { get; }
    }
}