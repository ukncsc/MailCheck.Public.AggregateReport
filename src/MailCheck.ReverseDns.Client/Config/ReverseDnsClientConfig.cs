using MailCheck.Common.Environment.Abstractions;

namespace MailCheck.ReverseDns.Client.Config
{
    public interface IReverseDnsClientConfig
    {
        string ReverseDnsApiEndpoint { get; }
    }

    internal class ReverseDnsClientConfig : IReverseDnsClientConfig
    {
        public ReverseDnsClientConfig(IEnvironmentVariables environment)
        {
            ReverseDnsApiEndpoint = environment.Get("ReverseDnsApiEndpoint");
        }

        public string ReverseDnsApiEndpoint { get; }
    }
}
