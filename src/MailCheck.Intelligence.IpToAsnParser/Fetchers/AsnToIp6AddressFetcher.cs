using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.IpToAsnParser.Fetchers
{
    public interface IAsnToIp6AddressFetcher
    {
        Task<Stream> Fetch(DateTime dateTime);
    }

    public class AsnToIp6AddressFetcher : AsnToIpAddressFetcher, IAsnToIp6AddressFetcher
    {
        public override string BaseUrl => "http://data.caida.org/datasets/routing/routeviews6-prefix2as/";

        public AsnToIp6AddressFetcher(ILogger<AsnToIp6AddressFetcher> logger) : base(logger)
        {
        }
    }
}