using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MailCheck.AggregateReport.Api.V2.Provider
{
    public interface IProviderDetailsProvider
    {
        string GetProviderMarkdown(string provider);
        string GetProviderAliasOut(string provider);
        string GetProviderAliasIn(string alias);
    }

    public class ProviderDetailsProvider : IProviderDetailsProvider
    {
        public readonly Dictionary<string, string> ProviderAlias;
        public readonly Dictionary<string, string> AliasProvider;
        public readonly Dictionary<string, string> ProviderMarkdown;

        private readonly string _feedbackMarkdown = 
        $"{Environment.NewLine}***{Environment.NewLine}### Did this help?{Environment.NewLine}Email [mailcheck@digital.ncsc.gov.uk](mailto:mailcheck@digital.ncsc.gov.uk) if you note any errors or have any tips you would like to share.";
        
        public ProviderDetailsProvider()
        {
            string sources = File.ReadAllText("Provider/ProviderRules.json");
            ProviderRules providerRules = JsonConvert.DeserializeObject<ProviderRules>(sources);

            ProviderAlias = providerRules.ProviderAliases.ToDictionary(x => x.Provider, y => y.Mapping);
            AliasProvider = providerRules.ProviderAliases.ToDictionary(x => x.Mapping, y => y.Provider);
            ProviderMarkdown = providerRules.ProviderMarkdowns.ToDictionary(x => x.Provider, y => y.Mapping);
        }

        public string GetProviderMarkdown(string provider)
        {
            if (ProviderMarkdown.ContainsKey(provider))
            {
                return string.Concat(ProviderMarkdown[provider], _feedbackMarkdown);
            }

            return null;
        }

        public string GetProviderAliasOut(string provider)
        {
            if (ProviderAlias.ContainsKey(provider))
            {
                return ProviderAlias[provider];
            }

            return null;
        }

        public string GetProviderAliasIn(string alias)
        {
            if (AliasProvider.ContainsKey(alias))
            {
                return AliasProvider[alias];
            }

            return alias;
        }
    }
}