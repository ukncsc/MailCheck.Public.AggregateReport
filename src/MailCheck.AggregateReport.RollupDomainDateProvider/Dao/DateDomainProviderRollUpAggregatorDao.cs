using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Common.Aggregators;


namespace MailCheck.AggregateReport.RollupDomainDateProvider.Dao
{
    public interface IDateDomainProviderRollUpAggregatorDao
    {
        Task Save(List<DomainDateProviderRecord> records);
    }

    public class DateDomainProviderRollUpAggregatorDao : IDateDomainProviderRollUpAggregatorDao
    {
        private static readonly AggregatorDao<DomainDateProviderRecord>.Settings _settings = new AggregatorDao<DomainDateProviderRecord>.Settings
        {
            TableName = "domain_date_provider_rollup",
            FieldNames = new[]
                {
                    "domain",
                    "date",
                    "provider",
                    "spf_pass_dkim_pass_none",
                    "spf_pass_dkim_fail_none",
                    "spf_fail_dkim_pass_none",
                    "spf_fail_dkim_fail_none",
                    "spf_pass_dkim_pass_quarantine",
                    "spf_pass_dkim_fail_quarantine",
                    "spf_fail_dkim_pass_quarantine",
                    "spf_fail_dkim_fail_quarantine",
                    "spf_pass_dkim_pass_reject",
                    "spf_pass_dkim_fail_reject",
                    "spf_fail_dkim_pass_reject",
                    "spf_fail_dkim_fail_reject"
                },
            UpdateStatements = DateDomainProviderRollUpAggregatorDaoResources.UpdateStatements,
            ParameterValuesMapper = MapParameters
        };

        private readonly IAggregatorDao<DomainDateProviderRecord> _aggregatorDao;

        public DateDomainProviderRollUpAggregatorDao(IAggregatorDao<DomainDateProviderRecord> aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Save(List<DomainDateProviderRecord> records)
        {
            await _aggregatorDao.Save(records, _settings);
        }

        private static IEnumerable<KeyValuePair<string, object>> MapParameters(DomainDateProviderRecord record)
        {
            yield return KeyValuePair.Create<string, object>("domain", record.Domain);
            yield return KeyValuePair.Create<string, object>("date", record.Date.ToString("yyyy-MM-dd"));
            yield return KeyValuePair.Create<string, object>("provider", record.Provider);
            yield return KeyValuePair.Create<string, object>("spf_pass_dkim_pass_none", record.SpfPassDkimPassNone);
            yield return KeyValuePair.Create<string, object>("spf_pass_dkim_fail_none", record.SpfPassDkimFailNone);
            yield return KeyValuePair.Create<string, object>("spf_fail_dkim_pass_none", record.SpfFailDkimPassNone);
            yield return KeyValuePair.Create<string, object>("spf_fail_dkim_fail_none", record.SpfFailDkimFailNone);
            yield return KeyValuePair.Create<string, object>("spf_pass_dkim_pass_quarantine", record.SpfPassDkimPassQuarantine);
            yield return KeyValuePair.Create<string, object>("spf_pass_dkim_fail_quarantine", record.SpfPassDkimFailQuarantine);
            yield return KeyValuePair.Create<string, object>("spf_fail_dkim_pass_quarantine", record.SpfFailDkimPassQuarantine);
            yield return KeyValuePair.Create<string, object>("spf_fail_dkim_fail_quarantine", record.SpfFailDkimFailQuarantine);
            yield return KeyValuePair.Create<string, object>("spf_pass_dkim_pass_reject", record.SpfPassDkimPassReject);
            yield return KeyValuePair.Create<string, object>("spf_pass_dkim_fail_reject", record.SpfPassDkimFailReject);
            yield return KeyValuePair.Create<string, object>("spf_fail_dkim_pass_reject", record.SpfFailDkimPassReject);
            yield return KeyValuePair.Create<string, object>("spf_fail_dkim_fail_reject", record.SpfFailDkimFailReject);
        }
    }
}