using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;

namespace MailCheck.AggregateReport.DomDatProIpDkim
{
    public static class AggregateReportRecordExtensions
    {
        public static List<DomDatProIpDkimRecord> ToDomDatProIpDkimRecord(
            this AggregateReportRecordEnriched aggregateReportRecord)
        {
            long id = long.Parse(aggregateReportRecord.RecordId);
            string domain = aggregateReportRecord.HeaderFrom?.Trim().Trim('.').ToLower() ?? aggregateReportRecord.DomainFrom.ToLower();
            string ip = aggregateReportRecord.HostSourceIp;
            string provider = aggregateReportRecord.GetProvider();
            string originalProvider = aggregateReportRecord.HostProvider;

            DateTime date = aggregateReportRecord.EffectiveDate.Date;
            int count = aggregateReportRecord.Count;
            List<string> dkimAuthResults = aggregateReportRecord.DkimAuthResults;
            
            List<Tuple<string, string, string>> dkimDomainResults = new List<Tuple<string, string, string>> {};

            if (dkimAuthResults.Any())
            {
                foreach(string dkimDomainResult in dkimAuthResults)
                {
                    string dkimDomain = dkimDomainResult.Split(':')[0];
                    string dkimSelector = dkimDomainResult.Split(':')[1];
                    string dkimResult = dkimDomainResult.Split(':')[2].ToLower();
                    dkimDomainResults.Add(Tuple.Create(dkimDomain, dkimSelector, dkimResult));
                }   
            }

            List<DomDatProIpDkimRecord> resultSets = dkimDomainResults.Select(x =>
                    CreateDomDatProIpDkim(id, domain, date, provider, originalProvider, ip, count, x.Item1, x.Item2, x.Item3))
                .ToList();

            List<DomDatProIpDkimRecord> allProviderResultSets = resultSets.Select(x => x.CloneWithDifferentProvider("All Providers", null)).ToList();
            resultSets.AddRange(allProviderResultSets);
            
            
            return resultSets;
        }
        
        private static DomDatProIpDkimRecord CreateDomDatProIpDkim(long recordId, string domain, DateTime date, 
                string provider, string originalProvider, string ip, int count, string dkimDomain, string dkimSelector, string dkimResult)
        {
            if (dkimResult == "pass")
            {
                return new DomDatProIpDkimRecord(recordId, domain, date, provider, originalProvider, ip, dkimDomain, dkimSelector, count, 0);
            }

            return new DomDatProIpDkimRecord(recordId, domain, date, provider, originalProvider, ip, dkimDomain, dkimSelector, 0, count);
        }
    }
}