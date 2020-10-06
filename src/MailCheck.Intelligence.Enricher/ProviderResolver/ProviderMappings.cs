namespace MailCheck.Intelligence.Enricher.ProviderResolver
{
    namespace MailCheck.Intelligence.Enricher.ProviderResolver
    {
        public class ProviderMappings
        {
            public ProviderMapping[] IpAddressProviderMappings { get; set; }
            public ProviderMapping[] OrgDomainProviderMappings { get; set; }
        }

        public class ProviderMapping
        {
            public string[] Mappings { get; set; }
            public string Provider { get; set; }
        }
    }
}
