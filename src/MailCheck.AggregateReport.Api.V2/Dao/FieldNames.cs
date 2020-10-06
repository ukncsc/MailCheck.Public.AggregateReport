namespace MailCheck.AggregateReport.Api.V2.Dao
{
    internal static class FieldNames
    {
        public const string spf_pass_dkim_pass_none = "spf_pass_dkim_pass_none";
        public const string spf_pass_dkim_fail_none = "spf_pass_dkim_fail_none";
        public const string spf_fail_dkim_pass_none = "spf_fail_dkim_pass_none";
        public const string spf_fail_dkim_fail_none = "spf_fail_dkim_fail_none";
        public const string spf_pass_dkim_pass_quarantine = "spf_pass_dkim_pass_quarantine";
        public const string spf_pass_dkim_fail_quarantine = "spf_pass_dkim_fail_quarantine";
        public const string spf_fail_dkim_pass_quarantine = "spf_fail_dkim_pass_quarantine";
        public const string spf_fail_dkim_fail_quarantine = "spf_fail_dkim_fail_quarantine";
        public const string spf_pass_dkim_pass_reject = "spf_pass_dkim_pass_reject";
        public const string spf_pass_dkim_fail_reject = "spf_pass_dkim_fail_reject";
        public const string spf_fail_dkim_pass_reject = "spf_fail_dkim_pass_reject";
        public const string spf_fail_dkim_fail_reject = "spf_fail_dkim_fail_reject";
    }
}