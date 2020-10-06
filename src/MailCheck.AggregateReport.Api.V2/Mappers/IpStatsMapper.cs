using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;

namespace MailCheck.AggregateReport.Api.V2.Mappers
{
    public interface IIpStatsMapper
    {
        IpStatsDto Map(IpStats ipStats, string providerAlias, string providerMarkdown);
    }

    public class IpStatsMapper : IIpStatsMapper
    {
        public IpStatsDto Map(IpStats ipStats, string providerAlias,  string providerMarkdown)
        {
            return new IpStatsDto(ipStats.Domain, ipStats.Provider, ipStats.Ip, ipStats.Hostname,
                ipStats.SpfPassDkimPassNone,
                ipStats.SpfPassDkimFailNone, ipStats.SpfFailDkimPassNone, ipStats.SpfFailDkimFailNone,
                ipStats.SpfPassDkimPassQuarantine, ipStats.SpfPassDkimFailQuarantine, ipStats.SpfFailDkimPassQuarantine,
                ipStats.SpfFailDkimFailQuarantine, ipStats.SpfPassDkimPassReject, ipStats.SpfPassDkimFailReject,
                ipStats.SpfFailDkimPassReject, ipStats.SpfFailDkimFailReject,
                ipStats.FullyTrusted, ipStats.PartiallyTrusted, ipStats.Untrusted, ipStats.Quarantined, ipStats.Rejected, 
                ipStats.Delivered,
                ipStats.TotalEmails,
                ipStats.PassSpfTotal, ipStats.PassDkimTotal, ipStats.FailSpfTotal, ipStats.FailDkimTotal,
                ipStats.SpfMisaligned, ipStats.DkimMisaligned,
                ipStats.ProxyBlockListCount, ipStats.SuspiciousNetworkBlockListCount,
                ipStats.HijackedetworkBlockListCount,
                ipStats.EndUserNetworkBlockListCount, ipStats.SpamSourceBlockListCount, ipStats.MalwareBlockListCount,
                ipStats.EndUserBlockListCount, ipStats.BounceReflectorBlockListCount, ipStats.Forwarded,
                ipStats.SampledOut,
                ipStats.TrustedForwarder, ipStats.MailingList, ipStats.LocalPolicy, ipStats.Arc,
                ipStats.OtherOverrideReason,
                providerAlias, providerMarkdown);
        }
    }
}