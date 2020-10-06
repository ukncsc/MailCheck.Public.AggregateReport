using System.Collections.Generic;
using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.IpToAsnParser.Dao
{
    public interface IIp4ToAsnDao
    {
        Task Save(List<IpToAsn> ipToAsns);
    }

    public class Ip4ToAsnDao : IpToAsnDao, IIp4ToAsnDao
    {
        protected override string InsertTpToAsnValueFormatString => Ip4ToAsnDaoResource.InsertTpToAsnValueFormatString;
        protected override string InsertIpToAsnCommand => Ip4ToAsnDaoResource.InsertIpToAsn;
        protected override string InsertIpToAsnOnConflict => Ip4ToAsnDaoResource.InsertIpToAsnOnConflict;
        protected override string TruncateCommand => Ip4ToAsnDaoResource.Truncate;
        protected override string RefreshMaterializedViewCommand => Ip4ToAsnDaoResource.RefreshMaterializedView;

        public Ip4ToAsnDao(IConnectionInfoAsync connectionInfo, ILogger<IpToAsnDao> log) : base(connectionInfo, log)
        {
        }
    }
}