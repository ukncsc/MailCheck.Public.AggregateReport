using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Common.Aggregators;

namespace MailCheck.AggregateReport.DomainDateProviderIp.Dao
{
    public interface IDateDomainProviderIpAggregatorDao
    {
        Task Save(List<DomainDateProviderIpRecord> records);
    }

    public class DateDomainProviderIpAggregatorDao : IDateDomainProviderIpAggregatorDao
    {
        private static readonly AggregatorDao<DomainDateProviderIpRecord>.Settings _settings = new AggregatorDao<DomainDateProviderIpRecord>.Settings
        {
            TableName = "domain_date_provider_ip",
            FieldNames = new[]
                {
                    "domain",
                    "date",
                    "provider",
                    "original_provider",
                    "ip",
                    "hostname",
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
                    "spf_fail_dkim_fail_reject",
                    "spf_misaligned",
                    "dkim_misaligned",
                    "blocklists_proxy",
                    "blocklists_hijackednetwork",
                    "blocklists_suspiciousnetwork",
                    "blocklists_endusernetwork",
                    "blocklists_spamsource",
                    "blocklists_malware",
                    "blocklists_enduser",
                    "blocklists_bouncereflector",
                    "por_forwarded",
                    "por_sampledout",
                    "por_trustedforwarder",
                    "por_mailinglist",
                    "por_localpolicy",
                    "por_arc",
                    "por_other",
                },
            UpdateStatements = DateDomainProviderIpAggregatorDaoResources.UpdateStatements,
            ParameterValuesMapper = MapParameters
        };

        private readonly IAggregatorDao<DomainDateProviderIpRecord> _aggregatorDao;

        public DateDomainProviderIpAggregatorDao(IAggregatorDao<DomainDateProviderIpRecord> aggregatorDao)
        {
            _aggregatorDao = aggregatorDao;
        }

        public async Task Save(List<DomainDateProviderIpRecord> records)
        {
            await _aggregatorDao.Save(records, _settings);
        }

        private static IEnumerable<KeyValuePair<string, object>> MapParameters(DomainDateProviderIpRecord record)
        {
            yield return KeyValuePair.Create<string, object>("domain", record.Domain);
            yield return KeyValuePair.Create<string, object>("date", record.Date.ToString("yyyy-MM-dd"));
            yield return KeyValuePair.Create<string, object>("provider", record.Provider);
            yield return KeyValuePair.Create<string, object>("original_provider", record.Provider);
            yield return KeyValuePair.Create<string, object>("ip", record.Ip);
            yield return KeyValuePair.Create<string, object>("hostname", record.Hostname);
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
            yield return KeyValuePair.Create<string, object>("spf_misaligned", record.SpfMisaligned);
            yield return KeyValuePair.Create<string, object>("dkim_misaligned", record.DkimMisaligned);
            yield return KeyValuePair.Create<string, object>("blocklists_proxy", record.ProxyBlockListCount);
            yield return KeyValuePair.Create<string, object>("blocklists_hijackednetwork", record.HijackedNetworkBlockListCount);
            yield return KeyValuePair.Create<string, object>("blocklists_suspiciousnetwork", record.SuspiciousNetworkBlockListCount);
            yield return KeyValuePair.Create<string, object>("blocklists_endusernetwork", record.EndUserNetworkBlockListCount);
            yield return KeyValuePair.Create<string, object>("blocklists_spamsource", record.SpamSourceBlockListCount);
            yield return KeyValuePair.Create<string, object>("blocklists_malware", record.MalwareBlockListCount);
            yield return KeyValuePair.Create<string, object>("blocklists_enduser", record.EndUserBlockListCount);
            yield return KeyValuePair.Create<string, object>("blocklists_bouncereflector", record.BounceReflectorBlockListCount);
            yield return KeyValuePair.Create<string, object>("por_forwarded", record.Forwarded);
            yield return KeyValuePair.Create<string, object>("por_sampledout", record.SampledOut);
            yield return KeyValuePair.Create<string, object>("por_trustedforwarder", record.TrustedForwarder);
            yield return KeyValuePair.Create<string, object>("por_mailinglist", record.MailingList);
            yield return KeyValuePair.Create<string, object>("por_localpolicy", record.LocalPolicy);
            yield return KeyValuePair.Create<string, object>("por_arc", record.Arc);
            yield return KeyValuePair.Create<string, object>("por_other", record.OtherOverrideReason);
        }
    }
}