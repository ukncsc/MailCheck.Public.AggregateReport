using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.AggregateReport.Parser.Utils;
using MailCheck.AggregateReport.Parser.Domain.Report;

namespace MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation
{
    public interface IAggregateReportDeserialiser
    {
        AggregateReportInfo Deserialise(AttachmentInfo attachment, EmailMetadata emailMetadata);
    }

    public class AggregateReportDeserialiser : IAggregateReportDeserialiser
    {
        private readonly IReportMetadataDeserialiser _reportMetadataDeserialiser;
        private readonly IPolicyPublishedDeserialiser _policyPublishedDeserialiser;
        private readonly IRecordDeserialiser _recordDeserialiser;

        public AggregateReportDeserialiser(
            IReportMetadataDeserialiser reportMetadataDeserialiser,
            IPolicyPublishedDeserialiser policyPublishedDeserialiser,
            IRecordDeserialiser recordDeserialiser)
        {
            _reportMetadataDeserialiser = reportMetadataDeserialiser;
            _policyPublishedDeserialiser = policyPublishedDeserialiser;
            _recordDeserialiser = recordDeserialiser;
        }

        public AggregateReportInfo Deserialise(AttachmentInfo attachment, EmailMetadata emailMetadata)
        {
            attachment.AttachmentStream.Seek(0, SeekOrigin.Begin);
            using (StreamReader streamReader = new StreamReader(attachment.AttachmentStream, Encoding.UTF8, true, 1024, true))
            {
                using (XmlReader reader = XmlReader.Create(streamReader))
                {
                    XDocument document = XDocument.Load(reader);

                    XElement feedback = document.Root;
                    if (document.Root != null && document.Root.Name != "feedback")
                    {
                        throw new ArgumentException("Root of aggregate report must be feedback.");
                    }

                    XElement versionElement = feedback.SingleOrDefault("version");
                    double? version = double.TryParse(versionElement?.Value, out var candidateVersion) 
                        ? candidateVersion 
                        : (double?)null;

                    ReportMetadata reportMetadata = _reportMetadataDeserialiser.Deserialise(feedback.Single("report_metadata"));
                    PolicyPublished policyPublished = _policyPublishedDeserialiser.Deserialise(feedback.Single("policy_published"));

                    IEnumerable<XElement> recordElements = feedback.Where("record").ToList();

                    if (!recordElements.Any())
                    {
                        throw new ArgumentException("Aggregate report must contain at least 1 record.");
                    }

                    Record[] records = _recordDeserialiser.Deserialise(recordElements);

                    Domain.Dmarc.AggregateReport aggregateReport = new Domain.Dmarc.AggregateReport(version, reportMetadata, policyPublished, records);
                    return new AggregateReportInfo(aggregateReport, emailMetadata, attachment);
                }
            }
        }
    }
}