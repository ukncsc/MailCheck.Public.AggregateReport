using System.Linq;

namespace MailCheck.AggregateReport.Parser.Domain.Dmarc
{
    public class PolicyEvaluated
    {
        public PolicyEvaluated()
        {
        }

        public PolicyEvaluated(Disposition? disposition, DmarcResult? dkim, DmarcResult? spf,
            PolicyOverrideReason[] reasons)
        {
            Disposition = disposition;
            Dkim = dkim;
            Spf = spf;
            Reasons = reasons;
        }

        public Disposition? Disposition { get; set; }

        public DmarcResult? Dkim { get; set; }

        public DmarcResult? Spf { get; set; }

        public PolicyOverrideReason[] Reasons { get; set; }

        public bool HasForwardedReason => HasPolicy(PolicyOverride.forwarded);

        public bool HasSampledOutReason => HasPolicy(PolicyOverride.sampled_out);

        public bool HasTrustedForwarderReason => HasPolicy(PolicyOverride.trusted_forwarder);

        public bool HasMailingListReason => HasPolicy(PolicyOverride.mailing_list);

        public bool HasLocalPolicyReason => HasPolicy(PolicyOverride.local_policy);

        public bool HasArcReason => HasPolicy(PolicyOverride.arc);

        public bool HasOtherReason => HasPolicy(PolicyOverride.other);

        private bool HasPolicy(PolicyOverride policy)
        {
            return Reasons?.Any(_ => _.PolicyOverride == policy) ?? false;
        }
    }
}
