using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Util;

namespace MailCheck.Intelligence.Enricher.ReverseDns.Dao
{
    public interface IPublicSuffixDao
    {
        Task<List<string>> GetPublicSuffixList();
    }

    public class PublicSuffixDao : IPublicSuffixDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;

        public PublicSuffixDao(IConnectionInfoAsync connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task<List<string>> GetPublicSuffixList()
        {
            string connectionString = await _connectionInfo.GetConnectionStringAsync();

            List<string> suffixes = new List<string>();

            using (DbDataReader reader = await PostgreSqlHelper.ExecuteReaderAsync(connectionString, PublicSuffixDaoResource.SelectPublicSuffixList))
            {
                while (await reader.ReadAsync())
                {
                    suffixes.Add(reader.GetString("suffix"));
                }
            }

            return suffixes;
        }
    }
}
