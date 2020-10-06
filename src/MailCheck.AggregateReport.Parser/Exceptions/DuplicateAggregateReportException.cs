using System;
using System.Collections.Generic;
using System.Text;

namespace MailCheck.AggregateReport.Parser.Exceptions
{
    public class DuplicateAggregateReportException : Exception
    {
        public DuplicateAggregateReportException(string message) : base(message)
        {
        }
    }
}
