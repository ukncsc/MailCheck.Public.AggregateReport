using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.AggregateReport.Parser.Utils;

namespace MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation
{
    public interface ISpfAuthResultDeserialiser
    {
        SpfAuthResult[] Deserialise(IEnumerable<XElement> spfAuthResults);
    }

    public class SpfAuthResultDeserialiser : ISpfAuthResultDeserialiser
    {
        public SpfAuthResult[] Deserialise(IEnumerable<XElement> spfAuthResults)
        {
            if (spfAuthResults.Any(_ => _.Name != "spf"))
            {
                throw new ArgumentException("All elements must be spf.");
            }

            return spfAuthResults
                .Select(Deserialise)
                .Where(result => result.Domain != null)
                .ToArray();
        }

        private SpfAuthResult Deserialise(XElement element)
        {
            var domain = element.SingleOrDefault("domain")?.Value;
            var result = element.SingleOrDefault("result")?.Value;
            var scope = element.SingleOrDefault("scope")?.Value;

            if (result == null)
            {
                throw new ArgumentException("Expected element 'result' was not found");
            }

            //Single as expecting to get the element, nullable as the result from the element might not be in the enum from the spec.
            SpfResult? spfResult = Enum.TryParse(result, true, out SpfResult candidateResult) ? candidateResult : (SpfResult?)null;

            SpfDomainScope? spfDomainScope = Enum.TryParse(scope, true, out SpfDomainScope candidateSpfDomainScope)
                    ? candidateSpfDomainScope 
                    : (SpfDomainScope?) null;

            // For a result of "none" some implementations omit the domain element
            if (domain is null && spfResult != SpfResult.none)
            {
                throw new ArgumentException("Expected element 'domain' was not found");
            }

            return new SpfAuthResult(domain, spfDomainScope, spfResult);
        }
    }
}