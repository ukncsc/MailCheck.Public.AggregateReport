namespace MailCheck.AggregateReport.Parser.Domain.Dmarc
{
    public enum SpfResult
    {
        none,
        neutral,
        pass,
        fail,
        softfail,
        temperror,
        permerror
    }
}