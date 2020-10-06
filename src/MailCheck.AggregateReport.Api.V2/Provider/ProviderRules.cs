namespace MailCheck.AggregateReport.Api.V2.Provider
{
    public class ProviderRules
    {
        public ProviderRule[] ProviderMarkdowns { get; set; }
        public ProviderRule[] ProviderAliases { get; set; }
    }

    public class ProviderRule
    {
        public string Provider { get; set; }
        public string Mapping { get; set; }
    }
}