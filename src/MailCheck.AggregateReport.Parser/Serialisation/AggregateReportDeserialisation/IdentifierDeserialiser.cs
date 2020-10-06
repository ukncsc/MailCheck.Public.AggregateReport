using System;
using System.Xml.Linq;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.AggregateReport.Parser.Utils;

namespace MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation
{
    public interface IIdentifiersDeserialiser
    {
        Identifier Deserialise(XElement identifiers);
    }

    public class IdentifiersDeserialiser : IIdentifiersDeserialiser
    {
        public Identifier Deserialise(XElement identifiers)
        {
            if (identifiers.Name != "identifiers")
            {
                throw new ArgumentException("Root element must be identifiers");
            }

            string envelopeTo = identifiers.SingleOrDefault("envelope_to")?.Value;
            string envelopeFrom = identifiers.SingleOrDefault("envelope_from")?.Value;
            string headerFrom = identifiers.Single("header_from").Value;

            return new Identifier(envelopeTo, envelopeFrom, headerFrom);
        }
    }
}