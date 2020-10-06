using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Lambda.S3Events;
using Amazon.Runtime.Internal;
using Amazon.S3.Util;
using MailCheck.Common.Messaging.Abstractions;

namespace MailCheck.AggregateReport.Parser.Domain
{
    public class S3EventMailCheckMessage : Message
    {
        public S3EventMailCheckMessage(string id, List<S3EventNotification.S3EventNotificationRecord> records) : base(id)
        {
            Records = records ?? new List<S3EventNotification.S3EventNotificationRecord>();
        }

        public List<S3EventNotification.S3EventNotificationRecord> Records { get; set; }
    }
}
