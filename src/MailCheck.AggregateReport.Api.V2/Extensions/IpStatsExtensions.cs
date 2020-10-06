using System;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.AggregateReport.Api.V2.Dto;

namespace MailCheck.AggregateReport.Api.V2.Extensions
{
    public static class IpStatsExtensions
    {
        public static IpStats CloneWithDifferentHostname(this IpStats ipstats, string hostname)
        {
            return new IpStats(ipstats.Domain, ipstats.Provider, ipstats.Ip, hostname,
                ipstats.SpfPassDkimPassNone, ipstats.SpfPassDkimFailNone, ipstats.SpfFailDkimPassNone, ipstats.SpfFailDkimFailNone,
                ipstats.SpfPassDkimPassQuarantine, ipstats.SpfPassDkimFailQuarantine, ipstats.SpfFailDkimPassQuarantine, ipstats.SpfFailDkimFailQuarantine, 
                ipstats.SpfPassDkimPassReject, ipstats.SpfPassDkimFailReject, ipstats.SpfFailDkimPassReject, ipstats.SpfFailDkimFailReject, 
                ipstats.FullyTrusted, ipstats.PartiallyTrusted, ipstats.Untrusted, ipstats.Quarantined, ipstats.Rejected,
                ipstats.Delivered,
                ipstats.TotalEmails,
                ipstats.PassSpfTotal, ipstats.PassDkimTotal, ipstats.FailSpfTotal, ipstats.FailDkimTotal,
                ipstats.SpfMisaligned, ipstats.DkimMisaligned,
                ipstats.ProxyBlockListCount, ipstats.SuspiciousNetworkBlockListCount, ipstats.HijackedetworkBlockListCount,
                ipstats.EndUserNetworkBlockListCount, ipstats.SpamSourceBlockListCount, ipstats.MalwareBlockListCount,
                ipstats.EndUserBlockListCount, ipstats.BounceReflectorBlockListCount, ipstats.Forwarded, ipstats.SampledOut,
                ipstats.TrustedForwarder, ipstats.MailingList, ipstats.LocalPolicy, ipstats.Arc, ipstats.OtherOverrideReason);
        }
    }
}