using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Util;
using MailCheck.Intelligence.Enricher.Asn;
using MailCheck.Intelligence.Enricher.Blocklist;
using MailCheck.Intelligence.Enricher.ReverseDns;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.Enricher
{
    public interface IIpAddressLookup
    {
        Task<List<IpAddressDetails>> Lookup(List<IpAddressDetailsRequest> ipAddressDetailsRequests);
    }

    public class IpAddressLookup : IIpAddressLookup
    {
        private readonly IAsInfoProvider _asInfoProvider;
        private readonly IBlocklistProvider _blocklistProvider;
        private readonly IReverseDnsProvider _reverseDnsProvider;
        private readonly IClock _clock;
        private readonly ILogger<IpAddressLookup> _log;
        private const int AsnDataAgeDays = -2;
        private const int BlocklistLookupWindowDays = -1;

        public IpAddressLookup(IAsInfoProvider asInfoProvider, IBlocklistProvider blocklistProvider, IReverseDnsProvider reverseDnsProvider, IClock clock, ILogger<IpAddressLookup> log)
        {
            _asInfoProvider = asInfoProvider;
            _blocklistProvider = blocklistProvider;
            _reverseDnsProvider = reverseDnsProvider;
            _clock = clock;
            _log = log;
        }

        public async Task<List<IpAddressDetails>> Lookup(List<IpAddressDetailsRequest> ipAddressDetailsRequests)
        {
            DateTime nowUtc = _clock.GetDateTimeUtc();

            List<string> distinctIpAddresses = ipAddressDetailsRequests
                .Select(x => x.IpAddress).Distinct().ToList();

            List<string> blockListIpAddressesToLookUp = ipAddressDetailsRequests
                .Where(x => x.Date >= nowUtc.Date.AddDays(BlocklistLookupWindowDays))
                .Select(x => x.IpAddress).Distinct().ToList();

            _log.LogDebug($"IpAddressLookup received request for {ipAddressDetailsRequests.Count} IpAddressDetails containing {distinctIpAddresses.Count} distinct IP addresses and {blockListIpAddressesToLookUp.Count} distinct IP addresses within blocklist window");

            Task<List<BlocklistResult>> blocklistResultsTask = _blocklistProvider.GetBlocklistAppearances(blockListIpAddressesToLookUp);
            Task<List<AsInfo>> asInfoTask = _asInfoProvider.GetAsInfo(distinctIpAddresses);
            Task<List<ReverseDnsResult>> reverseDnsResponsesTask = _reverseDnsProvider.GetReverseDnsResult(distinctIpAddresses);

            await Task.WhenAll(blocklistResultsTask, asInfoTask, reverseDnsResponsesTask);

            List<BlocklistResult> blocklistResults = await blocklistResultsTask;
            List<AsInfo> asInfos = await asInfoTask;
            List<ReverseDnsResult> reverseDnsResponses = await reverseDnsResponsesTask;

            Dictionary<string, BlocklistResult> blocklistResultsDictionary = blocklistResults.Where(x => x.IpAddress != null).ToDictionary(x => x.IpAddress);
            Dictionary<string, AsInfo> asInfoDictionary = asInfos.Where(x => x.IpAddress != null).ToDictionary(x => x.IpAddress);
            Dictionary<string, ReverseDnsResult> reverseDnsResponseDictionary = reverseDnsResponses.Where(x => x.OriginalIpAddress != null).ToDictionary(x => x.OriginalIpAddress);

            List<IpAddressDetails> responses = ipAddressDetailsRequests.Select(request =>
            {
                bool blockListResultHasValue = blocklistResultsDictionary.TryGetValue(request.IpAddress, out BlocklistResult blockListResult);
                bool asInfoHasValue = asInfoDictionary.TryGetValue(request.IpAddress, out AsInfo asInfo);
                bool reverseDnsResponseHasValue = reverseDnsResponseDictionary.TryGetValue(request.IpAddress, out ReverseDnsResult reverseDnsResult);
                return new IpAddressDetails(
                    request.IpAddress,
                    request.Date,
                    asInfoHasValue ? asInfo.AsNumber : 0,
                    asInfoHasValue ? asInfo.Description : "",
                    asInfoHasValue ? asInfo.CountryCode : "",
                    blockListResultHasValue ? blockListResult.BlocklistAppearances : null,
                    reverseDnsResponseHasValue ? reverseDnsResult.ForwardResponses : null,
                    nowUtc.AddDays(AsnDataAgeDays).Date,
                    nowUtc,
                    nowUtc,
                    !reverseDnsResponseHasValue || reverseDnsResult.IsInconclusive);
            }).ToList();

            _log.LogInformation($"Returning {responses.Count} IpAddressDetails from request for {ipAddressDetailsRequests.Count}");
            return responses;
        }
    }
}