namespace MailCheck.AggregateReport.DomainDateProviderSubdomain
{
    public static class DomainDateProviderSubdomainRecordExtensions
    {
        public static DomainDateProviderSubdomainRecord CloneWithDifferentProvider(this DomainDateProviderSubdomainRecord record,
            string provider)
        {
            return new DomainDateProviderSubdomainRecord(record.Id, record.Domain, record.Date,
                provider, record.Subdomain, record.SpfPassDkimPassNone,
                record.SpfPassDkimFailNone, record.SpfFailDkimPassNone, record.SpfFailDkimFailNone,
                record.SpfPassDkimPassQuarantine, record.SpfPassDkimFailQuarantine, record.SpfFailDkimPassQuarantine,
                record.SpfFailDkimFailQuarantine, record.SpfPassDkimPassReject, record.SpfPassDkimFailReject,
                record.SpfFailDkimPassReject, record.SpfFailDkimFailReject);
        }
    }
}