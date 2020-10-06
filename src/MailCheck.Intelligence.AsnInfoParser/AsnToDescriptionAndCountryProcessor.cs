using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Intelligence.AsnInfoParser.Dao;

namespace MailCheck.Intelligence.AsnInfoParser
{
    public class AsnToDescriptionAndCountryProcessor : IProcess
    {
        private readonly IAsnToDescriptionAndCountryFetcher _asnToDescriptionAndCountryFetcher;
        private readonly IAsnToDescriptionAndCountryParser _asnToDescriptionAndCountryParser;
        private readonly IAsnToDescriptionAndCountryDao _asnToDescriptionAndCountryDao;

        public AsnToDescriptionAndCountryProcessor(
            IAsnToDescriptionAndCountryFetcher asnToDescriptionAndCountryFetcher,
            IAsnToDescriptionAndCountryParser asnToDescriptionAndCountryParser,
            IAsnToDescriptionAndCountryDao asnToDescriptionAndCountryDao)
        {
            _asnToDescriptionAndCountryFetcher = asnToDescriptionAndCountryFetcher;
            _asnToDescriptionAndCountryParser = asnToDescriptionAndCountryParser;
            _asnToDescriptionAndCountryDao = asnToDescriptionAndCountryDao;
        }

        public async Task<ProcessResult> Process()
        {
            Stream stream = await _asnToDescriptionAndCountryFetcher.Fetch();

            List<AsnDescriptionCountryInfo> asnNameCountryInfos = await _asnToDescriptionAndCountryParser.Parse(stream);

            await _asnToDescriptionAndCountryDao.Save(asnNameCountryInfos);

            return ProcessResult.Stop;
        }
    }
}
