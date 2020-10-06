﻿namespace MailCheck.AggregateReport.RollupDomainDateProvider
{
    public static class DomainDateProviderRecordExtensions
    {
        public static DomainDateProviderRecord CloneWithDifferentProvider(this DomainDateProviderRecord record,
            string provider)
        {
            return new DomainDateProviderRecord(record.Id, record.Domain, record.Date,
                provider, record.SpfPassDkimPassNone,
                record.SpfPassDkimFailNone, record.SpfFailDkimPassNone, record.SpfFailDkimFailNone,
                record.SpfPassDkimPassQuarantine, record.SpfPassDkimFailQuarantine, record.SpfFailDkimPassQuarantine,
                record.SpfFailDkimFailQuarantine, record.SpfPassDkimPassReject, record.SpfPassDkimFailReject,
                record.SpfFailDkimPassReject, record.SpfFailDkimFailReject);
        }
    }
}