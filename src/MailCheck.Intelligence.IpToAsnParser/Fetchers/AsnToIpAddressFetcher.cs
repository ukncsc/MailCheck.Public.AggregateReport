using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.IpToAsnParser.Fetchers
{
    public abstract class AsnToIpAddressFetcher
    {
        public abstract string BaseUrl { get;  }

        private readonly ILogger<AsnToIpAddressFetcher> _log;

        protected AsnToIpAddressFetcher(ILogger<AsnToIpAddressFetcher> log)
        {
            _log = log;
        }

        public async Task<Stream> Fetch(DateTime dateTime)
        {
            _log.LogInformation($"Fetching ip to asn file for: {dateTime:yyyy-MM-dd}");
            Stopwatch stopwatch = Stopwatch.StartNew();

            string fileListUrl = GetFileListUrl(dateTime);
            _log.LogInformation($"Fetching ip to asn filename from: {fileListUrl}");

            string fileUrl = await GetFileNameUrl(fileListUrl);

            _log.LogInformation($"Fetching ip to asn file from: {fileUrl}");

            using (HttpClient client = new HttpClient())
            {
                Stream stream = await client.GetStreamAsync(fileUrl);
                _log.LogInformation($"Fetched ip to asn file in {stopwatch.ElapsedMilliseconds} ms.");
                return stream;
            }
        }

        private string GetFileListUrl(DateTime dateTime)
        {
            DateTime newDateTime = dateTime.AddDays(-2);
            string resource = $"{newDateTime:yyyy/MM}";
            return $"{BaseUrl}/{resource}";
        }

        private async Task<string> GetFileNameUrl(string fullUrl)
        {
            _log.LogInformation($"Getting file name url from {fullUrl}");

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(fullUrl);

            string latestFileName = doc.DocumentNode
                .SelectNodes("//body/pre/a")
                .Last()
                .Attributes["href"].Value;

            string fileUrl = $"{fullUrl}/{latestFileName}";
            return fileUrl;
        }
    }
}