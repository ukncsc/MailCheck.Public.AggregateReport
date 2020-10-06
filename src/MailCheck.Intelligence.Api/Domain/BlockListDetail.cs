using System;
using System.Collections.Generic;

namespace MailCheck.Intelligence.Api.Domain
{
    public class BlockListDetail
    {
        public BlockListDetail(DateTime date, string source, List<Flag> flags)
        {
            Source = source;
            StartDate = date;
            EndDate = date;
            Flags = flags;
        }

        public string Source { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Flag> Flags { get; set; }
    }
}