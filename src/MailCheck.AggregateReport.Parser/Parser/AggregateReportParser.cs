using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation;
using MailCheck.AggregateReport.Parser.Domain.Report;
using MailCheck.AggregateReport.Parser.Factory;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MailCheck.AggregateReport.Parser.Parser
{
    public interface IAggregateReportParser
    {
        AggregateReportInfo Parse(EmailMessageInfo emailMessageInfo);
    }

    internal class AggregateReportParser : IAggregateReportParser
    {
        private readonly IMimeMessageFactory _mimeMessageFactory;
        private readonly IAttachmentStreamNormaliser _attachmentStreamNormaliser;
        private readonly IAggregateReportDeserialiser _aggregateReportDeserialiser;
        private readonly ILogger<AggregateReportParser> _log;

        public AggregateReportParser(IMimeMessageFactory mimeMessageFactory,
            IAttachmentStreamNormaliser attachmentStreamNormaliser,
            IAggregateReportDeserialiser aggregateReportDeserialiser,
            ILogger<AggregateReportParser> log)
        {
            _mimeMessageFactory = mimeMessageFactory;
            _attachmentStreamNormaliser = attachmentStreamNormaliser;
            _aggregateReportDeserialiser = aggregateReportDeserialiser;
            _log = log;
        }

        public AggregateReportInfo Parse(EmailMessageInfo messageInfo)
        {
            var mimeMessage = _mimeMessageFactory.Create(messageInfo.EmailStream);

            _log.LogInformation($"Successfully parsed S3 object as MimeMessage. From: {mimeMessage.From} Subject: {mimeMessage.Subject}");

            List<AttachmentInfo> attachments = mimeMessage
                .BodyParts.OfType<MimePart>()
                .Select(_attachmentStreamNormaliser.Normalise)
                .Where(_ => !_.Equals(AttachmentInfo.EmptyAttachmentInfo))
                .ToList();

            string logString = $"{messageInfo.EmailMetadata.OriginalUri}, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}";
            
            if (attachments.Count == 0)
            {
                throw new ApplicationException(
                    $"Failed to parse: no attachment found where one was expected for {logString}.");
            }

            if (attachments.Count > 1)
            {
                var attachmentFilenames = attachments.Select(attachment => attachment.AttachmentMetadata.Filename).ToArray();
                var attachmentFilenamesString = string.Join(", ", attachmentFilenames);
                
                throw new ApplicationException(
                    $"Failed to parse: multiple attachments found where only one was expected for {logString}. {Environment.NewLine} Attachment filenames: {attachmentFilenamesString}");
            }
            
            AttachmentInfo attachmentInfo = attachments[0];

            AggregateReportInfo aggregateReportInfo = _aggregateReportDeserialiser.Deserialise(attachmentInfo, messageInfo.EmailMetadata);

            _log.LogInformation(
                $"Successfully processed attachment for {logString}.");
            
            return aggregateReportInfo;
        }
    }
}