using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Intelligence.Api.Dao;
using MailCheck.Intelligence.Api.Domain;
using MailCheck.Intelligence.Api.Utilities;

namespace MailCheck.Intelligence.Api.Services
{
    public interface IIpAddressDetailsService
    {
        Task<IpAddressDateRangeDetails> GetIpAddressDetails(string ipAddress, DateTime startDate, DateTime endDate);
    }

    public class IpAddressDetailsService : IIpAddressDetailsService
    {
        private readonly IIpAddressDetailsApiDao _ipAddressDetailsApiDao;
        private readonly IIpAddressDateRangeDetailsBuilder _ipAddressDateRangeDetailsBuilder;

        public IpAddressDetailsService(
            IIpAddressDetailsApiDao ipAddressDetailsApiDao,
            IIpAddressDateRangeDetailsBuilder ipAddressDateRangeDetailsBuilder)
        {
            _ipAddressDetailsApiDao = ipAddressDetailsApiDao;
            _ipAddressDateRangeDetailsBuilder = ipAddressDateRangeDetailsBuilder;
        }
        
        public async Task<IpAddressDateRangeDetails> GetIpAddressDetails(string ipAddress, DateTime startDate, DateTime endDate)
        {
            List<IpAddressDetails> ipAddressDetailsList = await _ipAddressDetailsApiDao.GetIpAddressDetails(ipAddress, startDate, endDate);

            _ipAddressDateRangeDetailsBuilder.SetIpAddress(ipAddress);
            
            foreach (IpAddressDetails ipAddressDetails in ipAddressDetailsList.OrderBy(x => x.Date))
            { 
                // Add this date's AS Info
                _ipAddressDateRangeDetailsBuilder.AddAsInfo(ipAddressDetails.Date, ipAddressDetails.AsNumber.GetValueOrDefault(0), ipAddressDetails.Description, ipAddressDetails.CountryCode);

                // Add this date's BlockList info
                ipAddressDetails.BlockListOccurrences
                    .GroupBy(x => x.Source)
                    .Select(sourceGroup => new
                    {
                        Source = sourceGroup.Key,
                        Flags = sourceGroup.Select(x => new Flag(x.Flag, x.Description)).ToList()
                    })
                    .ToList()
                    .ForEach(source => _ipAddressDateRangeDetailsBuilder.AddBlockListDetail(ipAddressDetails.Date, source.Source, source.Flags));

                // Add this date's ReverseDNS info (using forwardMatch if available)
                if (ipAddressDetails.ReverseDnsResponses.Any())
                {
                    List<ReverseDnsResponse> orderedByHost = ipAddressDetails.ReverseDnsResponses
                        .OrderBy(x => x.Host)
                        .ToList();

                    List<ReverseDnsResponse> matchingHosts = orderedByHost
                        .Where(x => x.IpAddresses.Contains(ipAddress))
                        .ToList();
                        
                    ReverseDnsResponse bestHost = matchingHosts
                        .Concat(orderedByHost)
                        .First();
                    
                    _ipAddressDateRangeDetailsBuilder.AddReverseDnsDetail(ipAddressDetails.Date, bestHost.Host, bestHost.OrganisationalDomain, matchingHosts.Any());
                }
            }

            return _ipAddressDateRangeDetailsBuilder.GetDetails();
        }
    }
}        