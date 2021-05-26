using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;

namespace MailCheck.AggregateReport.DomDatProIpSpf
{
    public static class AggregateReportRecordExtensions
    {
        public static List<DomDatProIpSpfRecord> ToDomDatProIpSpfRecord(
            this AggregateReportRecordEnriched aggregateReportRecord)
        {
            long id = long.Parse(aggregateReportRecord.RecordId);
            string domain = aggregateReportRecord.HeaderFrom?.Trim().Trim('.').ToLower() ?? aggregateReportRecord.DomainFrom.ToLower();
            string ip = aggregateReportRecord.HostSourceIp;
            string provider = aggregateReportRecord.GetProvider();
            string originalProvider = aggregateReportRecord.HostProvider;
            DateTime date = aggregateReportRecord.EffectiveDate.Date;
            int count = aggregateReportRecord.Count;
            List<string> spfAuthResults = aggregateReportRecord.SpfAuthResults;
            
            List<Tuple<string, string>> spfDomainResults = new List<Tuple<string, string>> {};

            if (spfAuthResults.Any())
            {
                foreach(string spfDomainResult in spfAuthResults)
                {
                    string spfDomain = spfDomainResult.Split(':')[0];
                    string spfResult = spfDomainResult.Split(':')[1].ToLower();
                    spfDomainResults.Add(Tuple.Create(spfDomain, spfResult));
                }   
            }

            List<DomDatProIpSpfRecord> resultSets = spfDomainResults.Select(x =>
                    CreateDomDatProIpSpf(id, domain, date, provider, originalProvider, ip, count, x.Item1, x.Item2))
                .ToList();

            List<DomDatProIpSpfRecord> allProviderResultSets = resultSets.Select(x => x.CloneWithDifferentProvider("All Providers", null)).ToList();
            resultSets.AddRange(allProviderResultSets);
            
            return resultSets;
        }
        
        private static DomDatProIpSpfRecord CreateDomDatProIpSpf(long recordId, string domain, DateTime date, string provider, string originalProvider, string ip, int count, string spfDomain, string spfResult)
        {
            if (spfResult == "pass")
            {
                return new DomDatProIpSpfRecord(recordId, domain, date, provider, originalProvider, ip, spfDomain, count, 0);
            }

            return new DomDatProIpSpfRecord(recordId, domain, date, provider, originalProvider, ip, spfDomain, 0, count);
        }
    }
}