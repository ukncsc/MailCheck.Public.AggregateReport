using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.AggregateReport.Parser.Utils;

namespace MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation
{
    public interface IDkimAuthResultDeserialiser
    {
        DkimAuthResult[] Deserialise(IEnumerable<XElement> element);
    }

    public class DkimAuthResultDeserialiser : IDkimAuthResultDeserialiser
    {
        public DkimAuthResult[] Deserialise(IEnumerable<XElement> dkimAuthResults)
        {
            if (dkimAuthResults.Any(_ => _.Name != "dkim"))
            {
                throw new ArgumentException("All elements must be dkim.");
            }

            return dkimAuthResults
                .Select(Deserialise)
                .Where(result => result.Domain != null)
                .ToArray();
        }

        private DkimAuthResult Deserialise(XElement element)
        {
            var domain = element.SingleOrDefault("domain")?.Value;
            var result = element.SingleOrDefault("result")?.Value;
            var selector = element.SingleOrDefault("selector")?.Value;
            var human_result = element.SingleOrDefault("human_result")?.Value;

            if (result == null)
            {
                throw new ArgumentException("Expected element 'result' was not found");
            }

            //nullable this deviates from spec
            //but some providers provide values not in the spec
            DkimResult? dkimResult = Enum.TryParse(result, true, out DkimResult dkimResultCandidate) ? dkimResultCandidate : (DkimResult?)null;

            // For a result of "none" some implementations omit the domain element
            if (domain is null && dkimResult != DkimResult.none)
            {
                throw new ArgumentException("Expected element 'domain' was not found");
            }

            return new DkimAuthResult(domain, selector, dkimResult, human_result);
        }
    }
}