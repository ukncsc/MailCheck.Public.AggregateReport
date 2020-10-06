using System.Xml.Linq;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.AggregateReport.Parser.Utils;

namespace MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation
{
    public interface IAuthResultDeserialiser
    {
        AuthResult Deserialise(XElement authResults);
    }

    internal class AuthResultDeserialiser : IAuthResultDeserialiser
    {
        private readonly IDkimAuthResultDeserialiser _dkimAuthResultDeserialiser;
        private readonly ISpfAuthResultDeserialiser _spfAuthResultDeserialiser;

        public AuthResultDeserialiser(IDkimAuthResultDeserialiser dkimAuthResultDeserialiser, 
            ISpfAuthResultDeserialiser spfAuthResultDeserialiser)
        {
            _dkimAuthResultDeserialiser = dkimAuthResultDeserialiser;
            _spfAuthResultDeserialiser = spfAuthResultDeserialiser;
        }

        public AuthResult Deserialise(XElement authResults)
        {
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(authResults.Where("dkim"));

            //according to spec this should contain at least 1 element
            //however have seen that this isnt always the case
            SpfAuthResult[] spfAuthResults = _spfAuthResultDeserialiser.Deserialise(authResults.Where("spf"));

            return new AuthResult(dkimAuthResults, spfAuthResults);
        }
    }
}