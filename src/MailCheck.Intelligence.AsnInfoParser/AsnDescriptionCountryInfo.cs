namespace MailCheck.Intelligence.AsnInfoParser
{
    public class AsnDescriptionCountryInfo
    {
        public AsnDescriptionCountryInfo(long asn, string description, string country)
        {
            Asn = asn;
            Description = description;
            Country = country;
        }

        public long Asn { get; }
        public string Description { get; }
        public string Country { get; }
    }
}