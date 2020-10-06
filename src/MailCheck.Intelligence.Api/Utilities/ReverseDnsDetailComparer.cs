using System;
using System.Collections.Generic;
using MailCheck.Intelligence.Api.Domain;

namespace MailCheck.Intelligence.Api.Utilities
{
    public interface IReverseDnsDetailComparer : IEqualityComparer<ReverseDnsDetail>
    {
    }

    public sealed class ReverseDnsDetailComparer : IReverseDnsDetailComparer
    {
        public bool Equals(ReverseDnsDetail x, ReverseDnsDetail y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return String.Equals(x.Host, y.Host) && String.Equals(x.OrganisationalDomain, y.OrganisationalDomain) && x.ForwardMatches == y.ForwardMatches;
        }

        public int GetHashCode(ReverseDnsDetail obj)
        {
            unchecked
            {
                var hashCode = (obj.Host != null ? obj.Host.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.OrganisationalDomain != null ? obj.OrganisationalDomain.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.ForwardMatches.GetHashCode();
                return hashCode;
            }
        }
    }
}