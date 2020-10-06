using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.PublicSuffixParser
{
    public interface IPublicSuffixParser
    {
        Task<List<string>> Parser(Stream stream);
    }

    public class PublicSuffixParser : IPublicSuffixParser
    {
        private readonly ILogger<PublicSuffixParser> _log;
        private const string CommentToken = "//";

        public PublicSuffixParser(ILogger<PublicSuffixParser> log)
        {
            _log = log;
        }

        public async Task<List<string>> Parser(Stream stream)
        {
            _log.LogInformation("Parsing public suffix list.");
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<string> results = new List<string>();

            using (stream)
            {
                using(StreamReader reader  = new StreamReader(stream))
                {
                    string line = string.Empty;

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith(CommentToken))
                        {
                            results.Add(line);
                        }
                    }
                }
            }

            _log.LogInformation($"Parsed public suffix list in {stopwatch.ElapsedMilliseconds} ms.");
            return results;
        }
    }
}