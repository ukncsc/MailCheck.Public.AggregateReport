namespace MailCheck.Intelligence.IpToAsnParser
{
    public class IpToAsn
    {
        public IpToAsn(string ipAddr, long asn)
        {
            IpAddr = ipAddr;
            Asn = asn;
        }

        public string IpAddr { get; }
        public long Asn { get; }
    }
}