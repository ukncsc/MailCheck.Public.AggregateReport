namespace MailCheck.AggregateReport.DomDatProIpDkim
{
    public static class DomDatProIpDkimRecordExtensions
    {
        public static DomDatProIpDkimRecord CloneWithDifferentProvider(this DomDatProIpDkimRecord record,
            string provider, string originalProvider)
        {
            return new DomDatProIpDkimRecord(record.Id, record.Domain, record.Date,
                provider, originalProvider, record.Ip, record.DkimDomain, record.DkimSelector, record.DkimPass, record.DkimFail);
        }
    }
}