using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.Intelligence.Api.Domain;

namespace MailCheck.Intelligence.Api.Utilities
{
    public interface IIpAddressDateRangeDetailsBuilder
    {
        void AddAsInfo(DateTime date, int asNumber, string description, string countryCode);
        
        void AddBlockListDetail(DateTime date, string source, List<Flag> flags);
        
        void AddReverseDnsDetail(DateTime date, string host, string orgDomain, bool forwardMatches);

        void SetIpAddress(string ipAddress);
        
        IpAddressDateRangeDetails GetDetails();
    }

    public class IpAddressDateRangeDetailsBuilder : IIpAddressDateRangeDetailsBuilder
    {
        private IpAddressDateRangeDetails _ipAddressDateRangeDetails;
        private IAsInfoComparer _asInfoComparer;
        private IBlocklistDetailsComparer _blocklistDetailsComparer;
        private IReverseDnsDetailComparer _reverseDnsDetailComparer;

        public IpAddressDateRangeDetailsBuilder(IAsInfoComparer asInfoComparer, IBlocklistDetailsComparer blocklistDetailsComparer, IReverseDnsDetailComparer reverseDnsDetailComparer)
        {
            this.Reset();
            this._asInfoComparer = asInfoComparer;
            this._blocklistDetailsComparer = blocklistDetailsComparer;
            this._reverseDnsDetailComparer = reverseDnsDetailComparer;
        }

        public void Reset()
        {
            this._ipAddressDateRangeDetails = new IpAddressDateRangeDetails();
        }
        
        public void AddAsInfo(DateTime date, int asNumber, string description, string countryCode)
        {
            AsInfo newAsInfo = new AsInfo(date, asNumber, description, countryCode);

            if (this._ipAddressDateRangeDetails.AsInfo.Any() && 
                this._asInfoComparer.Equals(this._ipAddressDateRangeDetails.AsInfo.Last(), newAsInfo))
            {
                this._ipAddressDateRangeDetails.AsInfo.Last().EndDate = newAsInfo.EndDate;
            }
            else
            {
                this._ipAddressDateRangeDetails.AsInfo.Add(newAsInfo);
            }
        }

        public void AddBlockListDetail(DateTime date, string source, List<Flag> flags)
        {
            BlockListDetail newBlocklistDetail = new BlockListDetail(date, source, flags);

            var lastMatchingBlockListDetail = this._ipAddressDateRangeDetails.BlockListDetails.LastOrDefault(x => x.Source == source);

            if (this._blocklistDetailsComparer.Equals(lastMatchingBlockListDetail, newBlocklistDetail))
            {
                // ReSharper disable once PossibleNullReferenceException
                lastMatchingBlockListDetail.EndDate = newBlocklistDetail.EndDate;
            }
            else
            {
                this._ipAddressDateRangeDetails.BlockListDetails.Add(newBlocklistDetail);
            }
        }

        public void AddReverseDnsDetail(DateTime date, string host, string orgDomain, bool forwardMatches)
        {
            ReverseDnsDetail newReverseDnsDetail = new ReverseDnsDetail(date, host, orgDomain, forwardMatches);
            
            if (this._ipAddressDateRangeDetails.ReverseDnsDetails.Any() && 
                this._reverseDnsDetailComparer.Equals(this._ipAddressDateRangeDetails.ReverseDnsDetails.Last(), newReverseDnsDetail))
            {
                this._ipAddressDateRangeDetails.ReverseDnsDetails.Last().EndDate = newReverseDnsDetail.EndDate;
            }
            else
            {
                this._ipAddressDateRangeDetails.ReverseDnsDetails.Add(newReverseDnsDetail);
            }
        }

        public void SetIpAddress(string ipAddress)
        {
            this._ipAddressDateRangeDetails.IpAddress = ipAddress;
        }

        public IpAddressDateRangeDetails GetDetails()
        {
            IpAddressDateRangeDetails ipAddressDateRangeDetails = this._ipAddressDateRangeDetails;
            this.Reset();
            return ipAddressDateRangeDetails;
        }
    }
}