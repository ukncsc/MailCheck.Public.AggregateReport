using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.AggregateReport.Contracts.IpIntelligence;
using MailCheck.Common.Util;
using MailCheck.Intelligence.Enricher.ReverseDns.Domain;
using MailCheck.Intelligence.Enricher.ReverseDns.Processor;
using MailCheck.Intelligence.Enricher.ReverseDns.PublicSuffix;

namespace MailCheck.Intelligence.Enricher.ReverseDns
{
    public interface IReverseDnsProvider
    {
        Task<List<ReverseDnsResult>> GetReverseDnsResult(List<string> ipAddresses);
    }

    public class ReverseDnsProvider : IReverseDnsProvider
    {
        private readonly IReverseDnsLookup _reverseDnsLookup;
        private readonly IOrganisationalDomainProvider _organisationalDomainProvider;

        public ReverseDnsProvider(IReverseDnsLookup reverseDnsLookup,
            IOrganisationalDomainProvider organisationalDomainProvider)
        {
            _reverseDnsLookup = reverseDnsLookup;
            _organisationalDomainProvider = organisationalDomainProvider;
        }

        public async Task<List<ReverseDnsResult>> GetReverseDnsResult(List<string> ipAddresses)
        {
            List<Task<ReverseDnsResult>> lookupTasks = ipAddresses.Select(Lookup).ToList();
            List<ReverseDnsResult> completedResults = new List<ReverseDnsResult>();
            foreach (IEnumerable<Task<ReverseDnsResult>> lookupTaskBatch in lookupTasks.Batch(8))
            {
                completedResults.AddRange(await Task.WhenAll(lookupTaskBatch));
            }

            return completedResults;
        }

        private async Task<ReverseDnsResult> Lookup(string ipAddress)
        {
            ReverseDnsResult reverseDnsResult = await _reverseDnsLookup.Lookup(ipAddress);

            if (!reverseDnsResult.ForwardResponses.Any())
            {
                return new ReverseDnsResult(ipAddress, new List<ReverseDnsResponse> { new ReverseDnsResponse("Unknown", null, "Unknown") });
            }

            if (!reverseDnsResult.ForwardResponses.Any(x => x.IpAddresses.Contains(ipAddress)))
            {
                List<ReverseDnsResponse> responses = reverseDnsResult.ForwardResponses.Select(x => new ReverseDnsResponse("Mismatch", x.IpAddresses, "Mismatch")).ToList();
                return new ReverseDnsResult(ipAddress, responses);
            }

            foreach (ReverseDnsResponse forwardResponse in reverseDnsResult.ForwardResponses)
            {
                OrganisationalDomain organisationalDomain = await _organisationalDomainProvider.GetOrganisationalDomain(forwardResponse.Host);
                forwardResponse.OrganisationalDomain = organisationalDomain.OrgDomain;
            }

            return reverseDnsResult;
        }
    }
}