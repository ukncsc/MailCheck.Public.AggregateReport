using System;
using System.Xml.Linq;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.AggregateReport.Parser.Utils;
using MailCheck.Common.Util;

namespace MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation
{
    public interface IPolicyPublishedDeserialiser
    {
        PolicyPublished Deserialise(XElement policyPublished);
    }

    public class PolicyPublishedDeserialiser : IPolicyPublishedDeserialiser
    {
        private readonly IDomainValidator _domainValidator;

        public PolicyPublishedDeserialiser(IDomainValidator domainValidator)
        {
            _domainValidator = domainValidator;
        }

        public PolicyPublished Deserialise(XElement policyPublished)
        {
            if (policyPublished.Name != "policy_published")
            {
                throw new ArgumentException("Root element must be policy_published");
            }

            string domain = policyPublished.Single("domain").Value;
            if (!_domainValidator.IsValidDomain(domain))
            {
                throw new ArgumentException("Invalid domain");
            }

            //nullable this deviates from spec
            Alignment? adkim = Enum.TryParse(policyPublished.SingleOrDefault("adkim")?.Value, true, out Alignment adkimCandidate) ? adkimCandidate : (Alignment?)null;

            //nullable this deviates from spec
            Alignment? aspf = Enum.TryParse(policyPublished.SingleOrDefault("aspf")?.Value, true, out Alignment aspfCandidate) ? aspfCandidate : (Alignment?)null;

            Disposition p = (Disposition)Enum.Parse(typeof(Disposition), policyPublished.Single("p").Value, true);

            //nullable this deviates from spec
            Disposition? sp = Enum.TryParse(policyPublished.SingleOrDefault("sp")?.Value, true, out Disposition spCandidate) ? spCandidate : (Disposition?)null;

            //nullable this deviates from spec
            int? pct = int.TryParse(policyPublished.SingleOrDefault("pct")?.Value, out var pctCandidate) ? pctCandidate : (int?)null;

            string fo = policyPublished.SingleOrDefault("fo")?.Value;

            return new PolicyPublished(domain, adkim, aspf, p, sp, pct, fo);
        }
    }
}