using System;
using System.Collections.Generic;
using System.Linq;

namespace MailCheck.AggregateReport.Contracts.IpIntelligence
{
    public class ReverseDnsResponse
    {
        public ReverseDnsResponse(string host, List<string> ipAddresses, string organisationalDomain = null)
        {
            Host = host;
            IpAddresses = ipAddresses ?? new List<string>();
            OrganisationalDomain = organisationalDomain;
        }

        public string Host { get; }

        public string OrganisationalDomain { get; set; }

        public List<string> IpAddresses { get; }

        protected bool Equals(ReverseDnsResponse other)
        {
            return string.Equals(Host, other.Host) && IpAddresses.SequenceEqual(other.IpAddresses);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReverseDnsResponse)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Host != null ? Host.GetHashCode() : 0) * 397) ^ (IpAddresses != null ? IpAddresses.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ReverseDnsResponse left, ReverseDnsResponse right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ReverseDnsResponse left, ReverseDnsResponse right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{nameof(Host)}: {Host}, {nameof(IpAddresses)}: {string.Join($"{Environment.NewLine}\t", IpAddresses)}";
        }
    }
}
