using System;
using MailCheck.Common.Environment.Abstractions;

namespace MailCheck.Intelligence.Enricher.Config
{
    public interface IEnricherConfig
    {
        TimeSpan DnsRecordLookupTimeout { get; }
        string SnsTopicArn { get; }
        string NameServer { get;  }
    }

    public class EnricherConfig : IEnricherConfig
    {
        public EnricherConfig(IEnvironmentVariables environmentVariables)
        {
            DnsRecordLookupTimeout = TimeSpan.FromSeconds(environmentVariables.GetAsLong("DnsRecordLookupTimeoutSeconds"));
            SnsTopicArn = environmentVariables.Get("SnsTopicArn");
            NameServer = environmentVariables.Get("NameServer");
        }

        public TimeSpan DnsRecordLookupTimeout { get; }
        public string SnsTopicArn { get; }
        public string NameServer { get; }
    }
}
