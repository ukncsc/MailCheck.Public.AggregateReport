using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Enricher.Dao;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.Enricher
{
    public interface IIpAddressProcessor
    {
        Task<List<IpAddressDetails>> Process(List<IpAddressDetailsRequest> requests);
    }

    public class IpAddressProcessor : IIpAddressProcessor
    {
        private readonly IIpAddressDetailsDao _ipAddressDetailsDao;
        private readonly IIpAddressLookup _ipAddressLookup;
        private readonly ILogger<IpAddressLookup> _log;

        public IpAddressProcessor(IIpAddressDetailsDao ipAddressDetailsDao, IIpAddressLookup ipAddressLookup, ILogger<IpAddressLookup> log)
        {
            _ipAddressDetailsDao = ipAddressDetailsDao;
            _ipAddressLookup = ipAddressLookup;
            _log = log;
        }

        public async Task<List<IpAddressDetails>> Process(List<IpAddressDetailsRequest> requests)
        {
            _log.LogInformation($"IpAddressProcessor received request for {requests.Count} ip address details");
            List<IpAddressDetails> cachedDetails = await _ipAddressDetailsDao.GetIpAddressDetails(requests);

            List<IpAddressDetailsRequest> cacheMisses = requests
                .Where(request => !cachedDetails.Any(response => response.IpAddress == request.IpAddress && response.Date == request.Date))
                .ToList();

            if (cacheMisses.Any())
            {
                _log.LogInformation($"Looking up {cacheMisses.Count} IpAddressDetails not found in database");

                List<IpAddressDetails> lookedUpDetails = await _ipAddressLookup.Lookup(cacheMisses);
                List<IpAddressDetails> validLookedUpDetails = lookedUpDetails.Where(x => !x.ReverseDnsInconclusive).ToList();

                await _ipAddressDetailsDao.SaveIpAddressDetails(validLookedUpDetails);
                cachedDetails.AddRange(lookedUpDetails);
            }

            _log.LogInformation($"IpAddressProcessor returned {cachedDetails.Count} IpAddressDetails of {requests.Count} requested");

            return cachedDetails;
        }
    }
}
