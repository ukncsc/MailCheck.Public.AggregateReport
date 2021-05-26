using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MailCheck.AggregateReport.Parser.Exceptions
{
    [Serializable]
    public class DuplicateAggregateReportException : AggregateReportParserException
    {
        public DuplicateAggregateReportException()
        {
        }

        public DuplicateAggregateReportException(string message) : base(message)
        {
        }

        public DuplicateAggregateReportException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DuplicateAggregateReportException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
