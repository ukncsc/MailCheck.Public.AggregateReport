﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MailCheck.AggregateReport.Parser.Domain.Report
{
    public class EmailMetadata
    {
        public EmailMetadata(string orginalUri, string filename, long fileSizeKb)
            : this(string.Empty, string.Empty, orginalUri, filename, fileSizeKb)
        {
        }

        public EmailMetadata(string requestId, string messageId, string originalUri, string filename, long fileSizeKb)
        {
            RequestId = requestId;
            MessageId = messageId;
            OriginalUri = originalUri;
            Filename = filename;
            FileSizeKb = fileSizeKb;
        }

        public string RequestId { get; }
        public string MessageId { get; }
        public string OriginalUri { get; }
        public string Filename { get; }
        public long FileSizeKb { get; }
    }
}
