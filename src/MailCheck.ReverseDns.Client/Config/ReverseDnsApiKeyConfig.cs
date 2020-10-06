using MailCheck.Common.Environment.Abstractions;

namespace MailCheck.ReverseDns.Client.Config
{
    internal interface IReverseDnsApiKeyConfig
    {
        string ReverseDnsClaimsName { get; }
    }

    internal class ReverseDnsApiKeyConfig : IReverseDnsApiKeyConfig
    {
        public ReverseDnsApiKeyConfig(IEnvironmentVariables environment)
        {
            ReverseDnsClaimsName = environment.Get("ReverseDnsClaimsName");
        }

        public string ReverseDnsClaimsName { get; }
    }
}