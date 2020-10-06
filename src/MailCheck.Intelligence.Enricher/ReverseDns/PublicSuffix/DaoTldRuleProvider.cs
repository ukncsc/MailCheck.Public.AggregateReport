using System.Collections.Generic;
using System.Threading.Tasks;
using Louw.PublicSuffix;
using MailCheck.Intelligence.Enricher.ReverseDns.Dao;

namespace MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix
{
    public class DaoTldRuleProvider : ITldRuleProvider
    {
        private readonly IPublicSuffixDao _publicSuffixDao;

        public DaoTldRuleProvider(IPublicSuffixDao publicSuffixDao)
        {
            _publicSuffixDao = publicSuffixDao;
        }

        public async Task<IEnumerable<TldRule>> BuildAsync()
        {
            TldRuleParser parser = new TldRuleParser();
            List<string> publicSuffixList = await _publicSuffixDao.GetPublicSuffixList();
            return parser.ParseRules(publicSuffixList);
        }
    }
}