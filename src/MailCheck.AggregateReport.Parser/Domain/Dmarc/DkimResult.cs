namespace MailCheck.AggregateReport.Parser.Domain.Dmarc
{
    public enum DkimResult
    {
        none,
        pass,
        fail,
        policy,
        neutral,
        temperror,
        permerror
    }
}