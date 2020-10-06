namespace MailCheck.AggregateReport.Parser.Domain.Dmarc
{
    public enum PolicyOverride
    {
        forwarded,
        sampled_out,
        trusted_forwarder,
        mailing_list,
        local_policy,
        other,
        arc
    }
}