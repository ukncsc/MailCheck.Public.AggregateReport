using System;
using System.Threading.Tasks;
using Louw.PublicSuffix;
using MailCheck.Intelligence.Enricher.ReverseDns.Domain;

namespace MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix
{
    public interface IOrganisationalDomainProvider
    {
        Task<OrganisationalDomain> GetOrganisationalDomain(string domain);
    }

    public class OrganisationalDomainProvider : IOrganisationalDomainProvider
    {
        private readonly ITldRuleProvider _tldRuleProvider;
        private readonly Lazy<DomainParser> _domainParser;

        public OrganisationalDomainProvider(ITldRuleProvider tldRuleProvider)
        {
            _tldRuleProvider = tldRuleProvider;
            _domainParser = new Lazy<DomainParser>(() => new DomainParser(_tldRuleProvider));
        }

        public async Task<OrganisationalDomain> GetOrganisationalDomain(string domain)
        {
            domain = domain.Trim().TrimEnd('.');

            DomainInfo domainInfo = await _domainParser.Value.ParseAsync(domain); return domainInfo == null ?
                new OrganisationalDomain(null, domain, true) :
                new OrganisationalDomain(domainInfo.RegistrableDomain, domain);
        }
    }
}
