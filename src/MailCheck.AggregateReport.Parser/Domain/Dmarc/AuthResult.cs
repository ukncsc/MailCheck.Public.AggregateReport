using System.Collections.Generic;
using System.Linq;

namespace MailCheck.AggregateReport.Parser.Domain.Dmarc
{
    public class AuthResult
    {
        public AuthResult()
        {
        }

        public AuthResult(DkimAuthResult[] dkim, SpfAuthResult[] spf)
        {
            Dkim = dkim;
            Spf = spf;
        }

        public DkimAuthResult[] Dkim { get; set; }

        public SpfAuthResult[] Spf { get; set; }

        public int SpfPassCount => Spf?.Count(x => x.Result == SpfResult.pass) ?? 0;

        public int SpfFailCount => Spf?.Count(x => x.Result == SpfResult.fail) ?? 0;

        public int DkimPassCount => Dkim?.Count(x => x.Result == DkimResult.pass) ?? 0;

        public int DkimFailCount => Dkim?.Count(x => x.Result == DkimResult.fail) ?? 0;
    }
}