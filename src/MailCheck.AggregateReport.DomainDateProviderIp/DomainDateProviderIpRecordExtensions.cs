namespace MailCheck.AggregateReport.DomainDateProviderIp
{
    public static class DomainDateProviderIpRecordExtensions
    {
        public static DomainDateProviderIpRecord CloneWithDifferentProvider(this DomainDateProviderIpRecord record,
            string provider)
        {
            return new DomainDateProviderIpRecord(record.Id, record.Domain, record.Date,
                provider, record.OriginalProvider, record.Ip, record.Hostname, record.SpfPassDkimPassNone,
                record.SpfPassDkimFailNone, record.SpfFailDkimPassNone, record.SpfFailDkimFailNone,
                record.SpfPassDkimPassQuarantine, record.SpfPassDkimFailQuarantine, record.SpfFailDkimPassQuarantine,
                record.SpfFailDkimFailQuarantine, record.SpfPassDkimPassReject, record.SpfPassDkimFailReject,
                record.SpfFailDkimPassReject, record.SpfFailDkimFailReject, record.SpfMisaligned, record.DkimMisaligned,
                record.ProxyBlockListCount, record.SuspiciousNetworkBlockListCount, record.HijackedNetworkBlockListCount, 
                record.EndUserNetworkBlockListCount, record.SpamSourceBlockListCount, record.MalwareBlockListCount,
                record.EndUserBlockListCount, record.BounceReflectorBlockListCount, record.Forwarded, record.SampledOut,
                record.TrustedForwarder, record.MailingList, record.LocalPolicy, record.Arc, record.OtherOverrideReason);
        }
    }
}