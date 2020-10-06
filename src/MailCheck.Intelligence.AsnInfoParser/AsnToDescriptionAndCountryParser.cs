using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.AsnInfoParser
{
    public interface IAsnToDescriptionAndCountryParser
    {
        Task<List<AsnDescriptionCountryInfo>> Parse(Stream stream);
    }

    public class AsnToDescriptionAndCountryParser : IAsnToDescriptionAndCountryParser
    {
        private readonly ILogger<AsnToDescriptionAndCountryParser> _log;

        public AsnToDescriptionAndCountryParser(ILogger<AsnToDescriptionAndCountryParser> log)
        {
            _log = log;
        }

        public async Task<List<AsnDescriptionCountryInfo>> Parse(Stream stream)
        {
            _log.LogInformation("Parsing asn to name and country.");
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<AsnDescriptionCountryInfo> asnNameCountryInfos = new List<AsnDescriptionCountryInfo>();
            using (stream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    string line = string.Empty;
                    while ((line = await streamReader.ReadLineAsync()) != null)
                    {
                        int firstSeparatorIndex = line.IndexOf(' ');
                        int lastSeparatorIndex = line.LastIndexOf(", ", StringComparison.Ordinal);

                        lastSeparatorIndex = lastSeparatorIndex == -1
                            ? line.LastIndexOf("; ", StringComparison.Ordinal)
                            : lastSeparatorIndex;

                        if (firstSeparatorIndex != -1 && lastSeparatorIndex != -1)
                        {
                            long asn = long.Parse(line.Substring(0, firstSeparatorIndex));
                            string description = line.Substring(firstSeparatorIndex + 1, lastSeparatorIndex - (firstSeparatorIndex + 1));
                            string country = line.Substring(lastSeparatorIndex + 2, line.Length - (lastSeparatorIndex + 2));

                            asnNameCountryInfos.Add(new AsnDescriptionCountryInfo(asn, description, country));
                        }
                    }
                }
                _log.LogInformation($"Parsed asn to name and country in {stopwatch.ElapsedMilliseconds} ms.");
                return asnNameCountryInfos;
            }
        }
    }
}