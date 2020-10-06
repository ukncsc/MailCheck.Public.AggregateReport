using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.PublicSuffixParser
{
    public interface IPublicSuffixFetcher
    {
        Task<Stream> Fetch();
    }

    public class PublicSuffixFetcher : IPublicSuffixFetcher
    {
        private readonly ILogger<PublicSuffixFetcher> _log;
        private const string PublicSuffixUrl = "https://publicsuffix.org/list/public_suffix_list.dat";

        public PublicSuffixFetcher(ILogger<PublicSuffixFetcher> log)
        {
            _log = log;
        }

        public async Task<Stream> Fetch()
        {
            _log.LogInformation("Fetching public suffix list.");
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (HttpClient client  = new HttpClient())
            {
                Stream stream = await client.GetStreamAsync(PublicSuffixUrl);
                _log.LogInformation($"Fetched public suffix list in {stopwatch.ElapsedMilliseconds} ms.");
                return stream;
            }
        }
    }
}