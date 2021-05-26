using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Common.Aggregators;

namespace MailCheck.AggregateReport.DomDatProIpDkim.Dao
{
    public interface IDomDatProIpDkimAggregatorDao
    {
        Task Save(List<DomDatProIpDkimRecord> records);
    }

    public class DomDatProIpDkimAggregatorDao : IDomDatProIpDkimAggregatorDao
    {
        private static readonly AggregatorDao<DomDatProIpDkimRecord>.Settings settings = new AggregatorDao<DomDatProIpDkimRecord>.Settings
        {
            TableName = "domain_date_provider_ip_dkim",
            FieldNames = new[]
                {
                    "domain",
                    "date",
                    "provider",
                    "original_provider",
                    "ip",
                    "dkim_domain",
                    "dkim_selector",
                    "dkim_pass",
                    "dkim_fail",
                },
            UpdateStatements = @"dkim_pass = dkim_pass + VALUES(dkim_pass), dkim_fail = dkim_fail + VALUES(dkim_fail)",
            ParameterValuesMapper = MapParameters
        };

        private readonly IAggregatorDao<DomDatProIpDkimRecord> _aggregatorDao;

        public DomDatProIpDkimAggregatorDao(IAggregatorDao<DomDatProIpDkimRecord> aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Save(List<DomDatProIpDkimRecord> records)
        {
            await _aggregatorDao.Save(records, settings);
        }

        private static IEnumerable<KeyValuePair<string, object>> MapParameters(DomDatProIpDkimRecord record)
        {
            yield return KeyValuePair.Create<string, object>("domain", record.Domain);
            yield return KeyValuePair.Create<string, object>("date", record.Date.ToString("yyyy-MM-dd"));
            yield return KeyValuePair.Create<string, object>("provider", record.Provider);
            yield return KeyValuePair.Create<string, object>("original_provider", record.OriginalProvider);
            yield return KeyValuePair.Create<string, object>("ip", record.Ip);
            yield return KeyValuePair.Create<string, object>("dkim_domain", record.DkimDomain);
            yield return KeyValuePair.Create<string, object>("dkim_selector", record.DkimSelector);
            yield return KeyValuePair.Create<string, object>("dkim_pass", record.DkimPass);
            yield return KeyValuePair.Create<string, object>("dkim_fail", record.DkimFail);
        }
    }
}