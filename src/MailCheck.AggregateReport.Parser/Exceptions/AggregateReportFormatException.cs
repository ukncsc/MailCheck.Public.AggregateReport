using System;
using System.Runtime.Serialization;

namespace MailCheck.AggregateReport.Parser.Exceptions
{
    [Serializable]
    public class AggregateReportFormatException : AggregateReportParserException
    {
        public AggregateReportFormatException()
        {
        }

        public AggregateReportFormatException(string message) : base(message)
        {
        }

        public AggregateReportFormatException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AggregateReportFormatException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}