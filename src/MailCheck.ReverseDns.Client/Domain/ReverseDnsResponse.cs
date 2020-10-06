using System.Collections.Generic;
using System.Net;

namespace MailCheck.ReverseDns.Client.Domain
{
    public class ReverseDnsResponse
    {
        public ReverseDnsResponse(HttpStatusCode statusCode, List<ReverseDnsInfo> reverseDnsInfo)
        {
            StatusCode = statusCode;
            ReverseDnsInfo = reverseDnsInfo;
        }

        public HttpStatusCode StatusCode { get; }
        public List<ReverseDnsInfo> ReverseDnsInfo { get; }
    }
}