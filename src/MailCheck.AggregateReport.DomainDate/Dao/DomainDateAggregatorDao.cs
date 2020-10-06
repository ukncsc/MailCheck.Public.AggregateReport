using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Common.Aggregators;

namespace MailCheck.AggregateReport.DomainDate.Dao
{
    public interface IDomainDateAggregatorDao
    {
        Task Save(params DomainDateRecord[] record);
    }

    public class DomainDateAggregatorDao : IDomainDateAggregatorDao
    {
        private static readonly AggregatorDao<DomainDateRecord>.Settings _settings = new AggregatorDao<DomainDateRecord>.Settings
        {
            TableName = "domain_date",
            FieldNames = new[]
                {
                    "domain",
                    "date",
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
            UpdateStatements = DomainDateAggregatorDaoResource.UpdateStatements,
            ParameterValuesMapper = MapParameters
        };

        private readonly IAggregatorDao<DomainDateRecord> _aggregatorDao;

        public DomainDateAggregatorDao(IAggregatorDao<DomainDateRecord> aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Save(params DomainDateRecord[] records)
        {
            await _aggregatorDao.Save(records, _settings);
        }

        private static IEnumerable<KeyValuePair<string, object>> MapParameters(DomainDateRecord record)
        {
            yield return KeyValuePair.Create<string, object>("domain", record.Domain);
            yield return KeyValuePair.Create<string, object>("date", record.Date.ToString("yyyy-MM-dd"));
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