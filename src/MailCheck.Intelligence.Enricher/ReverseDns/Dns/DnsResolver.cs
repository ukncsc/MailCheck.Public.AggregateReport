using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;

namespace MailCheck.Intelligence.Enricher.ReverseDns.Dns
{
    public interface IDnsResolver
    {
        Task<ReverseDnsQueryResponse> QueryPtrAsync(IPAddress originalAddress);
        Task<ReverseDnsQueryResponse> QueryAddressAsync<T>(string host, QueryType queryType) where T : AddressRecord;
    }

    public class DnsResolver : IDnsResolver
    {
        private readonly ILookupClient _lookupClient;

        public DnsResolver(ILookupClient lookupClient)
        {
            _lookupClient = lookupClient;
        }

        public async Task<ReverseDnsQueryResponse> QueryPtrAsync(IPAddress originalAddress)
        {
            IDnsQueryResponse response = await _lookupClient.QueryAsync(originalAddress.GetArpaName(), QueryType.PTR);
            
            return new ReverseDnsQueryResponse(response.HasError, response.ErrorMessage, response.AuditTrail,
                response.HasError 
                    ? null
                    : response.Answers.OfType<PtrRecord>().Select(_ => _.PtrDomainName.Original).Distinct().ToList());
        }

        public async Task<ReverseDnsQueryResponse> QueryAddressAsync<T>(string host, QueryType queryType)
            where T : AddressRecord
        {
            IDnsQueryResponse forward = await _lookupClient.QueryAsync(host, queryType);

            return new ReverseDnsQueryResponse(forward.HasError, forward.ErrorMessage, forward.AuditTrail,
                forward.HasError
                    ? null
                    : forward.Answers.OfType<T>().Select(x => x.Address.ToString()).ToList());
        }
    }

    public class ReverseDnsQueryResponse
    {
        public bool HasError { get; }
        public string ErrorMessage { get; }
        public string AuditTrail { get; set; }
        public List<string> Results { get; }

        public ReverseDnsQueryResponse(bool hasError, string errorMessage, string auditTrail, List<string> results)
        {
            HasError = hasError;
            ErrorMessage = errorMessage;
            AuditTrail = auditTrail;
            Results = results ?? new List<string>();
        }
    }
}
