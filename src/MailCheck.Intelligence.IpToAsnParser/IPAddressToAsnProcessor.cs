using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Intelligence.IpToAsnParser.Dao;
using MailCheck.Intelligence.IpToAsnParser.Fetchers;

namespace MailCheck.Intelligence.IpToAsnParser
{
    public class IpAddressToAsnProcessor : IProcess
    {
        private readonly IAsnToIpAddressParser _asnToIpAddressParser;
        private readonly IAsnToIp6AddressFetcher _asnToIp6AddressFetcher;
        private readonly IAsnToIp4AddressFetcher _asnToIp4AddressFetcher;
        private readonly IIp6ToAsnDao _ip6ToAsnDao;
        private readonly IIp4ToAsnDao _ipIp4ToAsnDao;


        public IpAddressToAsnProcessor(IAsnToIpAddressParser asnToIpAddressParser, IAsnToIp6AddressFetcher asnToIp6AddressFetcher, IAsnToIp4AddressFetcher asnToIp4AddressFetcher, IIp6ToAsnDao ip6ToAsnDao, IIp4ToAsnDao ipIp4ToAsnDao)
        {
            _asnToIpAddressParser = asnToIpAddressParser;
            _asnToIp6AddressFetcher = asnToIp6AddressFetcher;
            _asnToIp4AddressFetcher = asnToIp4AddressFetcher;
            _ip6ToAsnDao = ip6ToAsnDao;
            _ipIp4ToAsnDao = ipIp4ToAsnDao;
        }

        public async Task<ProcessResult> Process()
        {
            DateTime dateTime = DateTime.UtcNow;

            Stream ip6Stream = await _asnToIp6AddressFetcher.Fetch(dateTime);
            await _ip6ToAsnDao.Save(await ParseStream(ip6Stream));

            Stream ip4Stream = await _asnToIp4AddressFetcher.Fetch(dateTime);
            await _ipIp4ToAsnDao.Save(await ParseStream(ip4Stream));

            return ProcessResult.Stop;
        }

        private async Task<List<IpToAsn>> ParseStream(Stream stream)
        {
            List<IpToAsn> ipToAsns;
            using (stream)
            {
                ipToAsns = await _asnToIpAddressParser.Parse(stream);
            }

            return ipToAsns;
        }
    }
}