using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Intelligence.PublicSuffixParser.Dao;

namespace MailCheck.Intelligence.PublicSuffixParser
{
    public class PublicSuffixProcessor : IProcess
    {
        private readonly IPublicSuffixFetcher _fetcher;
        private readonly IPublicSuffixParser _parser;
        private readonly IPublicSuffixDao _dao;

        public PublicSuffixProcessor(IPublicSuffixFetcher fetcher,
            IPublicSuffixParser parser,
            IPublicSuffixDao dao)
        {
            _fetcher = fetcher;
            _parser = parser;
            _dao = dao;
        }

        public async Task<ProcessResult> Process()
        {
            Stream stream = await _fetcher.Fetch();

            List<string> publicSuffixEntries = await _parser.Parser(stream);

            await _dao.Save(publicSuffixEntries);

            return ProcessResult.Stop;
        }
    }
}
