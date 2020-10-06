namespace MailCheck.AggregateReport.DomDatProIpSpf
{
    public static class DomDatProIpSpfRecordExtensions
    {
        public static DomDatProIpSpfRecord CloneWithDifferentProvider(this DomDatProIpSpfRecord record,
            string provider)
        {
            return new DomDatProIpSpfRecord(record.Id, record.Domain, record.Date,
                provider, record.Ip, record.SpfDomain, record.SpfPass, record.SpfFail);
        }
    }
}