using System.Collections.Generic;
using System.Linq;
using MailCheck.Intelligence.Api.Domain;

namespace MailCheck.Intelligence.Api.Utilities
{
    public interface IBlocklistDetailsComparer : IEqualityComparer<BlockListDetail>
    {
    }

    public class BlocklistDetailsComparer : IBlocklistDetailsComparer
    {
        public bool Equals(BlockListDetail x, BlockListDetail y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return string.Equals(x.Source, y.Source)
                   && x.Flags.Count == y.Flags.Count
                   && x.Flags.Select(flag => flag.Name).All(y.Flags.Select(flag => flag.Name).Contains);
        }

        public int GetHashCode(BlockListDetail obj)
        {
            unchecked
            {
                return ((obj.Source != null ? obj.Source.GetHashCode() : 0) * 397) ^
                       (obj.Flags != null ? obj.Flags.GetHashCode() : 0);
            }
        }
    }
}