using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Common.Aggregators;

namespace MailCheck.AggregateReport.DomDatProIpSpf.Dao
{
    public interface IDomDatProIpSpfAggregatorDao
    {
        Task Save(List<DomDatProIpSpfRecord> records);
    }

    public class DomDatProIpSpfAggregatorDao : IDomDatProIpSpfAggregatorDao
    {
        private static readonly AggregatorDao<DomDatProIpSpfRecord>.Settings settings = new AggregatorDao<DomDatProIpSpfRecord>.Settings
        {
            TableName = "domain_date_provider_ip_spf",
            FieldNames = new[]
                {
                    "domain",
                    "date",
                    "provider",
                    "original_provider",
                    "ip",
                    "spf_domain",
                    "spf_pass",
                    "spf_fail",
                },
            UpdateStatements = @"spf_pass = spf_pass + VALUES(spf_pass), spf_fail = spf_fail + VALUES(spf_fail)",
            ParameterValuesMapper = MapParameters
        };

        private readonly IAggregatorDao<DomDatProIpSpfRecord> _aggregatorDao;

        public DomDatProIpSpfAggregatorDao(IAggregatorDao<DomDatProIpSpfRecord> aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Save(List<DomDatProIpSpfRecord> records)
        {
            await _aggregatorDao.Save(records, settings);
        }

        private static IEnumerable<KeyValuePair<string, object>> MapParameters(DomDatProIpSpfRecord record)
        {
            yield return KeyValuePair.Create<string, object>("domain", record.Domain);
            yield return KeyValuePair.Create<string, object>("date", record.Date.ToString("yyyy-MM-dd"));
            yield return KeyValuePair.Create<string, object>("provider", record.Provider);
            yield return KeyValuePair.Create<string, object>("original_provider", record.OriginalProvider);
            yield return KeyValuePair.Create<string, object>("ip", record.Ip);
            yield return KeyValuePair.Create<string, object>("spf_domain", record.SpfDomain);
            yield return KeyValuePair.Create<string, object>("spf_pass", record.SpfPass);
            yield return KeyValuePair.Create<string, object>("spf_fail", record.SpfFail);
        }
    }
}