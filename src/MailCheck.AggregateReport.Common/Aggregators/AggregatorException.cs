using System;
using System.Runtime.Serialization;

namespace MailCheck.AggregateReport.Common.Aggregators
{
    [Serializable]
    internal class AggregatorException : Exception
    {
        public AggregatorException()
        {
        }

        public AggregatorException(string message) : base(message)
        {
        }

        public AggregatorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AggregatorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}