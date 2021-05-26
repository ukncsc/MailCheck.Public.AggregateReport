using System;
using System.Runtime.Serialization;

namespace MailCheck.AggregateReport.Parser.Exceptions
{
    [Serializable]
    public class AggregateReportParserException : Exception
    {
        public AggregateReportParserException()
        {
        }

        public AggregateReportParserException(string message) : base(message)
        {
        }

        public AggregateReportParserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AggregateReportParserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}