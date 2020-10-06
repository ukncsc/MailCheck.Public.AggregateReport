using MailCheck.Common.Data;

namespace MailCheck.AggregateReport.Api.V2.Dao
{
    internal static class Fieldsets
    {
        public static readonly Fieldset All = new Fieldset
        {
            FieldNames.spf_pass_dkim_pass_none,
            FieldNames.spf_pass_dkim_fail_none,
            FieldNames.spf_fail_dkim_pass_none,
            FieldNames.spf_fail_dkim_fail_none,
            FieldNames.spf_pass_dkim_pass_quarantine,
            FieldNames.spf_pass_dkim_fail_quarantine,
            FieldNames.spf_fail_dkim_pass_quarantine,
            FieldNames.spf_fail_dkim_fail_quarantine,
            FieldNames.spf_pass_dkim_pass_reject,
            FieldNames.spf_pass_dkim_fail_reject,
            FieldNames.spf_fail_dkim_pass_reject,
            FieldNames.spf_fail_dkim_fail_reject,
        };

        public static readonly Fieldset Delivered = new Fieldset { FieldNames.spf_pass_dkim_pass_none, FieldNames.spf_fail_dkim_pass_none, FieldNames.spf_pass_dkim_fail_none, FieldNames.spf_fail_dkim_fail_none };

        public static readonly Fieldset FullyTrusted = new Fieldset { FieldNames.spf_pass_dkim_pass_none };
        public static readonly Fieldset PartiallyTrusted = new Fieldset { FieldNames.spf_fail_dkim_pass_none, FieldNames.spf_pass_dkim_fail_none };
        public static readonly Fieldset Untrusted = new Fieldset { FieldNames.spf_fail_dkim_fail_none };
        public static readonly Fieldset Quarantined = new Fieldset { FieldNames.spf_pass_dkim_pass_quarantine, FieldNames.spf_pass_dkim_fail_quarantine, FieldNames.spf_fail_dkim_pass_quarantine, FieldNames.spf_fail_dkim_fail_quarantine };
        public static readonly Fieldset Rejected = new Fieldset { FieldNames.spf_pass_dkim_pass_reject, FieldNames.spf_pass_dkim_fail_reject, FieldNames.spf_fail_dkim_pass_reject, FieldNames.spf_fail_dkim_fail_reject };

        public static readonly Fieldset PassSpf = new Fieldset { FieldNames.spf_pass_dkim_pass_none, FieldNames.spf_pass_dkim_fail_none, FieldNames.spf_pass_dkim_pass_quarantine, FieldNames.spf_pass_dkim_fail_quarantine, FieldNames.spf_pass_dkim_pass_reject, FieldNames.spf_pass_dkim_fail_reject };
        public static readonly Fieldset PassDkim = new Fieldset { FieldNames.spf_pass_dkim_pass_none, FieldNames.spf_fail_dkim_pass_none, FieldNames.spf_pass_dkim_pass_quarantine, FieldNames.spf_fail_dkim_pass_quarantine, FieldNames.spf_pass_dkim_pass_reject, FieldNames.spf_fail_dkim_pass_reject };
        public static readonly Fieldset FailSpf = new Fieldset { FieldNames.spf_fail_dkim_pass_none, FieldNames.spf_fail_dkim_fail_none, FieldNames.spf_fail_dkim_pass_quarantine, FieldNames.spf_fail_dkim_fail_quarantine, FieldNames.spf_fail_dkim_pass_reject, FieldNames.spf_fail_dkim_fail_reject };
        public static readonly Fieldset FailDkim = new Fieldset { FieldNames.spf_pass_dkim_fail_none, FieldNames.spf_fail_dkim_fail_none, FieldNames.spf_pass_dkim_fail_quarantine, FieldNames.spf_fail_dkim_fail_quarantine, FieldNames.spf_pass_dkim_fail_reject, FieldNames.spf_fail_dkim_fail_reject };
    }
}