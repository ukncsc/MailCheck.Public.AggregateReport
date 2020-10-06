using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.IpToAsnParser
{
    public interface IAsnToIpAddressParser
    {
        Task<List<IpToAsn>> Parse(Stream stream);
    }

    public class AsnToIpAddressParser : IAsnToIpAddressParser
    {
        private readonly ILogger<AsnToIpAddressParser> _log;

        public AsnToIpAddressParser(ILogger<AsnToIpAddressParser> log)
        {
            _log = log;
        }

        public async Task<List<IpToAsn>> Parse(Stream stream)
        {
            _log.LogInformation("Parsing ip to asn file");
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<IpToAsn> ipToAsns = new List<IpToAsn>();
            using (stream)
            {
                using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    using (StreamReader streamReader = new StreamReader(gzStream))
                    {
                        string line = string.Empty;
                        while ((line = await streamReader.ReadLineAsync()) != null)
                        {
                            string[] x = line.Split("\t");

                            string ipAddress = x[0];
                            string cidrBlock = x[1];
                            string[] asns = x[2].Split('_', ',');

                            foreach (string asn in asns)
                            {
                                ipToAsns.Add(new IpToAsn($"{ipAddress}/{cidrBlock}", long.Parse(asn)));
                            }
                        }
                    }
                }
            }

            _log.LogInformation($"Parsed ip to asn file in {stopwatch.ElapsedMilliseconds} ms.");
            return ipToAsns;
        }
    }
}