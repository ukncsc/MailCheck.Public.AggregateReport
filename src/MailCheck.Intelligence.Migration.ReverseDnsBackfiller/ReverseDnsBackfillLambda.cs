using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.AggregateReport.Contracts.Migration;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Intelligence.Enricher.Dao;
using MailCheck.Intelligence.Enricher.ReverseDns;
using Microsoft.Extensions.Logging;

namespace MailCheck.Intelligence.ReverseDnsBackfiller
{
    public class ReverseDnsBackfillLambda : IHandle<ReverseDnsBackfillBatch>
    {
        private readonly IIpAddressDetailsDao _ipAddressAddressDetailsDao;
        private readonly IReverseDnsProvider _reverseDnsProvider;

        private readonly ILogger<ReverseDnsBackfillLambda> _log;

        public ReverseDnsBackfillLambda(IIpAddressDetailsDao ipAddressAddressDetailsDao, IReverseDnsProvider reverseDnsProvider, ILogger<ReverseDnsBackfillLambda> log)
        {
            _log = log;
            _ipAddressAddressDetailsDao = ipAddressAddressDetailsDao;
            _reverseDnsProvider = reverseDnsProvider;
        }

        private async Task<ReverseDnsResult> Lookup(string ip)
        {
            return (await _reverseDnsProvider.GetReverseDnsResult(new List<string> { ip })).FirstOrDefault();
        }

        public async Task Handle(ReverseDnsBackfillBatch message)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            _log.LogInformation($"Received batch of {message.IpsToBackfill?.Count ?? 0} reverse dns entries to backfill with reverseDns lookups");

            if (message.IpsToBackfill == null || message.IpsToBackfill.Count == 0)
            {
                return;
            }

            foreach (string ipAddress in message.IpsToBackfill)
            {
                DateTime now = DateTime.UtcNow;
                ReverseDnsResult currentReverseDnsResult = await Lookup(ipAddress);

                List<IpAddressDetails> entries = await _ipAddressAddressDetailsDao.GetIpAddressDetails(ipAddress);
                List<IpAddressDetails> entriesWithReverseDns = entries.Where(x => x.ReverseDnsResponses != null).ToList();
                List<IpAddressDetails> entriesWithoutReverseDns = entries.Where(x => x.ReverseDnsResponses == null).ToList();

                List<IpAddressDetailsUpdateDto> entriesToUpdate = new List<IpAddressDetailsUpdateDto>();

                foreach (IpAddressDetails existingEntry in entriesWithoutReverseDns)
                {
                    if (entriesWithReverseDns.Any())
                    {
                        IpAddressDetails nearestNeighbour = entriesWithReverseDns.OrderBy(x => Math.Abs(x.Date.Ticks - existingEntry.Date.Ticks)).First();

                        long timeBetweenEntryAndNow = Math.Abs(now.Ticks - existingEntry.Date.Ticks);
                        long timeBetweenEntryAndNearestNeighbour = Math.Abs(nearestNeighbour.Date.Ticks - existingEntry.Date.Ticks);

                        if (timeBetweenEntryAndNearestNeighbour < timeBetweenEntryAndNow)
                        {
                            IpAddressDetailsUpdateDto entryToUpdate = new IpAddressDetailsUpdateDto(
                                existingEntry.IpAddress,
                                existingEntry.Date,
                                nearestNeighbour.ReverseDnsResponses,
                                nearestNeighbour.ReverseDnsLookupTimestamp);

                            entriesToUpdate.Add(entryToUpdate);
                        }
                        else
                        {
                            IpAddressDetailsUpdateDto entryToUpdate = new IpAddressDetailsUpdateDto(
                                existingEntry.IpAddress,
                                existingEntry.Date,
                                currentReverseDnsResult.ForwardResponses,
                                now);

                            entriesToUpdate.Add(entryToUpdate);
                        }
                    }
                    else
                    {
                        IpAddressDetailsUpdateDto entryToUpdate = new IpAddressDetailsUpdateDto(
                            existingEntry.IpAddress,
                            existingEntry.Date,
                            currentReverseDnsResult.ForwardResponses,
                            now);

                        entriesToUpdate.Add(entryToUpdate);
                    }
                }

                await _ipAddressAddressDetailsDao.UpdateReverseDns(entriesToUpdate);
            }

            _log.LogInformation(
                $"Processed batch of {message.IpsToBackfill?.Count} reverse dns entries in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
