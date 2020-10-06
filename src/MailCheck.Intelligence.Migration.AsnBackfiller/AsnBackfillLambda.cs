using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.Migration;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Intelligence.Enricher.Asn;
using MailCheck.Intelligence.Enricher.Dao;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.AsnBackfiller
{
    public class AsnBackfillLambda : IHandle<AsnBackfillBatch>
    {
        private readonly IAsInfoProvider _asInfoProvider;
        private readonly IIpAddressDetailsDao _ipAddressAddressDetailsDao;

        private readonly ILogger<AsnBackfillLambda> _log;

        public AsnBackfillLambda(IAsInfoProvider asInfoProvider, ILogger<AsnBackfillLambda> log, IIpAddressDetailsDao ipAddressAddressDetailsDao)
        {
            _log = log;
            _ipAddressAddressDetailsDao = ipAddressAddressDetailsDao;
            _asInfoProvider = asInfoProvider;
        }

        public async Task Handle(AsnBackfillBatch message)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            _log.LogInformation($"Received batch of {message.IpsToBackfill?.Count ?? 0} reverse dns entries to backfill will asn info");

            if (message.IpsToBackfill == null || message.IpsToBackfill.Count == 0)
            {
                return;
            }

            List<AsInfo> results = await _asInfoProvider.GetAsInfo(message.IpsToBackfill);
            _log.LogInformation($"Got results for {results.Count} of batch of {message.IpsToBackfill?.Count} in batch");

            if (results.Any())
            {
                await _ipAddressAddressDetailsDao.UpdateAsnInfo(results);
            }

            _log.LogInformation(
                $"Processed batch of {message.IpsToBackfill?.Count} reverse dns entries in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
