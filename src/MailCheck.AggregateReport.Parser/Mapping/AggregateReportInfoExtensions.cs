using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Util;
using ContractPolicy = MailCheck.AggregateReport.Contracts.Policy;
using ContractDmarcResult = MailCheck.AggregateReport.Contracts.DmarcResult;
using ContractAlignment = MailCheck.AggregateReport.Contracts.Alignment;
using DmarcPolicy = MailCheck.AggregateReport.Parser.Domain.Dmarc.Disposition;
using DmarcDmarcResult = MailCheck.AggregateReport.Parser.Domain.Dmarc.DmarcResult;
using DmarcAlignment = MailCheck.AggregateReport.Parser.Domain.Dmarc.Alignment;

namespace MailCheck.AggregateReport.Parser.Mapping
{
    public static class AggregateReportInfoExtensions
    {
        public static List<Message> ToDkimSelectorsSeenMessages(this AggregateReportInfo aggregateReportInfo)
        {
            return aggregateReportInfo.AggregateReport.Records
                .SelectMany(_ => _.AuthResults.Dkim.Where(x => !string.IsNullOrEmpty(x.Selector)))
                .GroupBy(_ => _.Domain, _ => _.Selector)
                .Select(_ => new DkimSelectorsSeen(aggregateReportInfo.EmailMetadata.RequestId,
                    aggregateReportInfo.EmailMetadata.MessageId, _.Key, _.Distinct().ToList()))
                .Cast<Message>()
                .ToList();
        }

        public static List<DenormalisedRecord> ToDenormalisedRecord(this AggregateReportInfo aggregateReportInfo)
        {
            Domain.Dmarc.AggregateReport aggregateReport = aggregateReportInfo.AggregateReport;
            string originalUri = aggregateReportInfo.EmailMetadata.OriginalUri;

            return aggregateReport.Records.Select(record => new DenormalisedRecord(
                originalUri,
                aggregateReport.ReportMetadata?.OrgName,
                aggregateReport.ReportMetadata?.Email,
                aggregateReport.ReportMetadata?.ExtraContactInfo,
                aggregateReport.ReportMetadata.Range.Begin.ToDateTime(),
                aggregateReport.ReportMetadata.Range.End.ToDateTime(),
                aggregateReport.PolicyPublished?.Domain,
                aggregateReport.PolicyPublished?.Adkim,
                aggregateReport.PolicyPublished?.Aspf,
                aggregateReport.PolicyPublished.P,
                aggregateReport.PolicyPublished?.Sp,
                aggregateReport.PolicyPublished?.Pct,
                record.Row?.SourceIp,
                record.Row.Count,
                record.Row?.PolicyEvaluated?.Disposition,
                record.Row?.PolicyEvaluated?.Dkim,
                record.Row.PolicyEvaluated.Spf,
                record.Row?.PolicyEvaluated?.Reasons != null ? string.Join(",", record.Row?.PolicyEvaluated?.Reasons.Select(_ => _.PolicyOverride.ToString())) : null,
                record.Row?.PolicyEvaluated?.Reasons != null ? string.Join(",", record.Row?.PolicyEvaluated?.Reasons.Where(_ => _.Comment != null).Select(_ => _.Comment.ToString())) : null,
                record.Identifiers?.EnvelopeTo,
                record.Identifiers?.HeaderFrom,
                record.AuthResults?.Dkim != null ? string.Join(",", record.AuthResults?.Dkim.Where(_ => _.Domain != null).Select(_ => _.Domain)) : null,
                record.AuthResults?.Dkim != null ? string.Join(",", record.AuthResults?.Dkim.Select(_ => _.Result)) : null,
                record.AuthResults?.Dkim != null ? string.Join(",", record.AuthResults?.Dkim.Where(_ => _.HumanResult != null).Select(_ => _.HumanResult)) : null,
                record.AuthResults?.Spf != null ? string.Join(",", record.AuthResults?.Spf.Where(_ => _.Domain != null).Select(_ => _.Domain)) : null,
                record.AuthResults?.Spf != null ? string.Join(",", record.AuthResults?.Spf.Select(_ => _.Result)) : null)).ToList();
        }

        public static List<AggregateReportRecord> ToAggregateReportRecords(this AggregateReportInfo aggregateReportInfo)
        {
            return aggregateReportInfo.AggregateReport.Records
                .Select(_ => _.ToAggregateReportRecord(aggregateReportInfo))
                .ToList();
        }

        private static AggregateReportRecord ToAggregateReportRecord(this Record record,
            AggregateReportInfo aggregateReportInfo)
        {
            return new AggregateReportRecord(
                record.Id.ToString(),
                aggregateReportInfo.AggregateReport.ReportMetadata.OrgName,
                aggregateReportInfo.AggregateReport.ReportMetadata.ReportId,
                aggregateReportInfo.AggregateReport.ReportMetadata.Range.EffectiveDate.ToDateTime(),
                aggregateReportInfo.AggregateReport.PolicyPublished?.Domain,
                aggregateReportInfo.AggregateReport.PolicyPublished?.Adkim?.ToAlignment(),
                aggregateReportInfo.AggregateReport.PolicyPublished?.Aspf?.ToAlignment(),
                aggregateReportInfo.AggregateReport.PolicyPublished?.P.ToPolicy() ?? ContractPolicy.none,
                aggregateReportInfo.AggregateReport.PolicyPublished?.Sp?.ToPolicy(),
                aggregateReportInfo.AggregateReport.PolicyPublished?.Pct,
                aggregateReportInfo.AggregateReport.PolicyPublished?.Fo,
                record.Row.SourceIp,
                record.Row.Count,
                record.Row.PolicyEvaluated.Disposition?.ToPolicy(),
                record.Row.PolicyEvaluated.Dkim?.ToDmarcResult(),
                record.Row.PolicyEvaluated.Spf?.ToDmarcResult(),
                record.Identifiers.EnvelopeTo,
                record.Identifiers.EnvelopeFrom,
                record.Identifiers.HeaderFrom,
                record.AuthResults.Spf.Select(x => $"{x.Domain}:{x.Result}").ToList(),
                record.AuthResults.SpfPassCount,
                record.AuthResults.SpfFailCount,
                record.AuthResults.Dkim.Select(x => $"{x.Domain}:{x.Selector}:{x.Result}").ToList(),
                record.AuthResults.DkimPassCount,
                record.AuthResults.DkimFailCount,
                record.Row.PolicyEvaluated.HasForwardedReason,
                record.Row.PolicyEvaluated.HasSampledOutReason,
                record.Row.PolicyEvaluated.HasTrustedForwarderReason,
                record.Row.PolicyEvaluated.HasMailingListReason,
                record.Row.PolicyEvaluated.HasLocalPolicyReason,
                record.Row.PolicyEvaluated.HasArcReason,
                record.Row.PolicyEvaluated.HasOtherReason);
        }

        private static ContractAlignment ToAlignment(this DmarcAlignment alignment)
        {
            switch (alignment)
            {
                case DmarcAlignment.r:
                    return ContractAlignment.r;
                case DmarcAlignment.s:
                    return ContractAlignment.s;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
            }
        }

        private static ContractPolicy ToPolicy(this DmarcPolicy disposition)
        {
            switch (disposition)
            {
                case DmarcPolicy.none:
                    return ContractPolicy.none;
                case DmarcPolicy.quarantine:
                    return ContractPolicy.quarantine;
                case DmarcPolicy.reject:
                    return ContractPolicy.reject;
                default:
                    throw new ArgumentOutOfRangeException(nameof(disposition), disposition, null);
            }
        }

        private static ContractDmarcResult ToDmarcResult(this DmarcDmarcResult dmarcResult)
        {
            switch (dmarcResult)
            {
                case DmarcDmarcResult.pass:
                    return ContractDmarcResult.pass;
                case DmarcDmarcResult.fail:
                    return ContractDmarcResult.fail;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dmarcResult), dmarcResult, null);
            }
        }

        public static DateTime ToDateTime(this int intDateTime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(intDateTime);
            return dtDateTime;
        }
    }
}
