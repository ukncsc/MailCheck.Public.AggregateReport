using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.IpToAsnParser.Fetchers
{
    public interface IAsnToIp4AddressFetcher
    {
        Task<Stream> Fetch(DateTime dateTime);
    }

    public class AsnToIp4AddressFetcher : AsnToIpAddressFetcher, IAsnToIp4AddressFetcher
    {
        public override string BaseUrl => "http://data.caida.org/datasets/routing/routeviews-prefix2as/";

        public AsnToIp4AddressFetcher(ILogger<AsnToIp4AddressFetcher> logger) : base(logger)
        {
        }
    }
}