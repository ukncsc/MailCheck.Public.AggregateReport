using System.Data.Common;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.Common.Data.Util;

namespace MailCheck.AggregateReport.Api.V2.Dao
{
    public interface IAggregateReportExportStatsFactory
    {
        AggregateReportExportStats Create(DbDataReader reader);
    }
    public class AggregateReportExportStatsFactory : IAggregateReportExportStatsFactory
    {
        public AggregateReportExportStats Create(DbDataReader reader)
        {
            AggregateReportExportStats result = new AggregateReportExportStats(
                reader.GetDateTime("effective_date").ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                reader.GetString("domain"),
                reader.GetString("provider"),
                reader.GetString("original_provider"),
                reader.GetString("reporter_org_name"),
                reader.GetString("ip"),
                reader.GetInt32("count"),
                reader.GetString("disposition"),
                reader.GetString("dkim"),
                reader.GetString("spf"),
                reader.GetString("envelope_to"),
                reader.GetString("envelope_from"),
                reader.GetString("header_from"),
                reader.GetString("organisation_domain_from"),
                reader.GetString("spf_auth_results"),
                reader.GetInt32("spf_pass_count"),
                reader.GetInt32("spf_fail_count"),
                reader.GetString("dkim_auth_results"),
                reader.GetInt32("dkim_pass_count"),
                reader.GetInt32("dkim_fail_count"),
                reader.GetInt32("forwarded"),
                reader.GetInt32("sampled_out"),
                reader.GetInt32("trusted_forwarder"),
                reader.GetInt32("mailing_list"),
                reader.GetInt32("local_policy"),
                reader.GetInt32("arc"),
                reader.GetInt32("other_override_reason"),
                reader.GetString("host_name"),
                reader.GetString("host_org_domain"),
                reader.GetString("host_provider"),
                reader.GetInt32("host_as_number"),
                reader.GetString("host_as_description"),
                reader.GetString("host_country"),
                reader.GetInt32("proxy_blocklist"),
                reader.GetInt32("suspicious_network_blocklist"),
                reader.GetInt32("hijacked_network_blocklist"),
                reader.GetInt32("enduser_network_blocklist"),
                reader.GetInt32("spam_source_blocklist"),
                reader.GetInt32("malware_blocklist"),
                reader.GetInt32("enduser_blocklist"),
                reader.GetInt32("bounce_reflector_blocklist")
            );
            return result;
        }
    }
}
