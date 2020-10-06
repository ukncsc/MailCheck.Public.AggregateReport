using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using MailCheck.Common.Api.AuthProviders;
using MailCheck.Common.Api.Utils;
using MailCheck.ReverseDns.Client.Config;
using MailCheck.ReverseDns.Client.Domain;
using Microsoft.Extensions.Logging;

namespace MailCheck.ReverseDns.Client
{
    public interface IReverseDnsClient
    {
        Task<ReverseDnsResponse> GetReverseDnsResponse(List<string> sourceIps, DateTime date);
    }

    public class ReverseDnsClient : IReverseDnsClient
    {
        private const string Info = "info";

        private readonly IReverseDnsClientConfig _config;
        private readonly IAuthenticationHeaderProvider _authenticationHeaderProvider;
        private readonly ILogger<ReverseDnsClient> _log;

        public ReverseDnsClient(IReverseDnsClientConfig config,
            IAuthenticationHeaderProvider authenticationHeaderProvider,
            ILogger<ReverseDnsClient> log)
        {
            _config = config;
            _authenticationHeaderProvider = authenticationHeaderProvider;
            _log = log;
        }

        public async Task<ReverseDnsResponse> GetReverseDnsResponse(List<string> sourceIps, DateTime date)
        {
            try
            {
                Dictionary<string, string> headers = await _authenticationHeaderProvider.GetAuthenticationHeaders();

                HttpResponseMessage httpResponseMessage = await _config.ReverseDnsApiEndpoint
                    .AllowAnyHttpStatus()
                    .AppendPathSegments(Info)
                    .WithHeaders(headers)
                    .PostJsonAsync(new ReverseDnsInfoRequest(sourceIps, date));

                List<ReverseDnsInfo> result = httpResponseMessage.IsSuccessStatusCode
                    ? await httpResponseMessage.ReceiveJson<List<ReverseDnsInfo>>()
                    : new List<ReverseDnsInfo>();

                return new ReverseDnsResponse(httpResponseMessage.StatusCode, result);
            }
            catch (Exception e)
            {
                _log.LogError(
                    $"The error {e.Message} occurred retrieving reverse dns info from {_config.ReverseDnsApiEndpoint}");
            }

            return new ReverseDnsResponse(HttpStatusCode.BadRequest, new List<ReverseDnsInfo>());
        }
    }
}