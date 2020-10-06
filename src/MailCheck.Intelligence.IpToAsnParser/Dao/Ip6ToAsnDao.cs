using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.IpToAsnParser.Dao
{
    public interface IIp6ToAsnDao
    {
        Task Save(List<IpToAsn> ipToAsns);
    }

    public class Ip6ToAsnDao : IpToAsnDao, IIp6ToAsnDao
    {
        protected override string InsertTpToAsnValueFormatString => Ip6ToAsnDaoResource.InsertTpToAsnValueFormatString;
        protected override string InsertIpToAsnCommand => Ip6ToAsnDaoResource.InsertIpToAsn;
        protected override string InsertIpToAsnOnConflict => Ip6ToAsnDaoResource.InsertIpToAsnOnConflict;
        protected override string TruncateCommand => Ip6ToAsnDaoResource.Truncate;
        protected override string RefreshMaterializedViewCommand => Ip6ToAsnDaoResource.RefreshMaterializedView;

        public Ip6ToAsnDao(IConnectionInfoAsync connectionInfo, ILogger<IpToAsnDao> log) : base(connectionInfo, log)
        {
        }
    }
}