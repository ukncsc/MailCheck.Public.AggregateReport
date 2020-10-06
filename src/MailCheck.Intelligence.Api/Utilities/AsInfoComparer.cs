using System.Collections.Generic;
using MailCheck.Intelligence.Api.Domain;

namespace MailCheck.Intelligence.Api.Utilities
{
    public interface IAsInfoComparer : IEqualityComparer<AsInfo>
    {
        new bool Equals(AsInfo x, AsInfo y);
        new int GetHashCode(AsInfo obj);
    }

    public class AsInfoComparer : IAsInfoComparer
    {
        public bool Equals(AsInfo x, AsInfo y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.AsNumber == y.AsNumber && string.Equals(x.Description, y.Description) &&
                   string.Equals(x.CountryCode, y.CountryCode);
        }

        public int GetHashCode(AsInfo obj)
        {
            unchecked
            {
                var hashCode = obj.AsNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Description != null ? obj.Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.CountryCode != null ? obj.CountryCode.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}