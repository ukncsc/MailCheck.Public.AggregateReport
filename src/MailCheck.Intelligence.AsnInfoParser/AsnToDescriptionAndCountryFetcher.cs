using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.AsnInfoParser
{
    public interface IAsnToDescriptionAndCountryFetcher
    {
        Task<Stream> Fetch();
    }

    public class AsnToDescriptionAndCountryFetcher : IAsnToDescriptionAndCountryFetcher
    {
        private const string AsnToDescriptionAndCountryUrl = "ftp://ftp.ripe.net/ripe/asnames/asn.txt";

        private readonly ILogger<AsnToDescriptionAndCountryFetcher> _log;

        public AsnToDescriptionAndCountryFetcher(ILogger<AsnToDescriptionAndCountryFetcher> log)
        {
            _log = log;
        }

        public async Task<Stream> Fetch()
        {
            _log.LogInformation($"Fetching asn to name and country.");
            Stopwatch stopwatch = Stopwatch.StartNew();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(AsnToDescriptionAndCountryUrl);

            using (FtpWebResponse response = (FtpWebResponse) await request.GetResponseAsync())
            {
                MemoryStream stream = new MemoryStream();
                await response.GetResponseStream().CopyToAsync(stream);
                _log.LogInformation($"Fetched asn to name and country in {stopwatch.ElapsedMilliseconds} ms.");
                return stream;
            }
        }
    }
}