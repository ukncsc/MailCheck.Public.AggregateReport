using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailCheck.Intelligence.Enricher.Dao;

namespace MailCheck.Intelligence.Enricher.Asn
{
    public interface IAsInfoProvider
    {
        Task<List<AsInfo>> GetAsInfo(List<string> ipAddresses);
    }

    public class AsInfoProvider : IAsInfoProvider
    {
        private readonly IAsnDao _asnDao;

        public AsInfoProvider(IAsnDao asnDao)
        {
            _asnDao = asnDao;
        }

        public async Task<List<AsInfo>> GetAsInfo(List<string> ipAddresses)
        {
            List<string> ipv4Addresses = new List<string>();
            List<string> ipv6Addresses = new List<string>();

            ipAddresses.ForEach(x =>
            {
                if (!IPAddress.TryParse(x, out IPAddress ipAddress)) return;
                switch (ipAddress.AddressFamily)
                {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                        ipv4Addresses.Add(x);
                        break;
                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        ipv6Addresses.Add(x);
                        break;
                }
            });

            Task<List<AsInfo>[]> getInfoTasks = Task.WhenAll(_asnDao.GetIp4(ipv4Addresses), _asnDao.GetIp6(ipv6Addresses));

            return (await getInfoTasks).SelectMany(x=>x).ToList();
        }
    }
}
