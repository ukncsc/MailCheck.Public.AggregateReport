using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.SimpleNotificationService.Model.Internal.MarshallTransformations;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.Parser.Domain;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.AggregateReport.Parser.Domain.Report;
using MailCheck.AggregateReport.Parser.Mapping;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Alignment = MailCheck.AggregateReport.Parser.Domain.Dmarc.Alignment;
using DmarcResult = MailCheck.AggregateReport.Parser.Domain.Dmarc.DmarcResult;

namespace MailCheck.AggregateReport.Parser.Test.Factory
{
    [TestFixture]
    public class AggregateReportSpfDkimAuthResultsTests
    {
   
        [Test]
        public void ParserShouldParseSingleDkimAuthResultCorrectly()
        {

            DkimAuthResult dkimAuthResult = new DkimAuthResult
            {
                Domain = "abc.gov.uk",
                Result = DkimResult.permerror,
                Selector = "selector"
            };
            
            List<AggregateReportRecord> result = CreateReportInfo(new List<DkimAuthResult>(){dkimAuthResult}).ToAggregateReportRecords();

            var firstRecord = result.First();
            
            Assert.That(firstRecord.DkimAuthResults.Count, Is.EqualTo(1));
            Assert.That(firstRecord.DkimAuthResults.First(), Is.EqualTo($"{dkimAuthResult.Domain}:{dkimAuthResult.Selector}:{dkimAuthResult.Result}"));

        }
        
        [Test]
        public void ParserShouldParseMultipleDkimAuthResultsCorrectly()
        {

            DkimAuthResult dkimAuthResult = new DkimAuthResult
            {
                Domain = "abc.gov.uk",
                Result = DkimResult.permerror,
                Selector = "selector"
            };
            
            DkimAuthResult dkimAuthResult2 = new DkimAuthResult
            {
                Domain = "cba.gov.uk",
                Result = DkimResult.pass,
                Selector = "selector"
            };
            
            List<AggregateReportRecord> result = CreateReportInfo(new List<DkimAuthResult>(){dkimAuthResult, dkimAuthResult2}).ToAggregateReportRecords();

            var firstRecord = result.First();
            
            Assert.That(firstRecord.DkimAuthResults.Count, Is.EqualTo(2));
            Assert.That(firstRecord.DkimAuthResults.First(), Is.EqualTo($"{dkimAuthResult.Domain}:{dkimAuthResult.Selector}:{dkimAuthResult.Result}"));
            Assert.That(firstRecord.DkimAuthResults.Last(), Is.EqualTo($"{dkimAuthResult2.Domain}:{dkimAuthResult.Selector}:{dkimAuthResult2.Result}"));

        }
        
        [Test]
        public void ParserShouldParseSingleSpfAuthResultCorrectly()
        {

            SpfAuthResult spfAuthResult = new SpfAuthResult
            {
                Domain = "abc.gov.uk",
                Result = SpfResult.permerror
            };
            
            List<AggregateReportRecord> result = CreateReportInfo(new List<SpfAuthResult>(){spfAuthResult}).ToAggregateReportRecords();

            var firstRecord = result.First();
            
            Assert.That(firstRecord.SpfAuthResults.Count, Is.EqualTo(1));
            Assert.That(firstRecord.SpfAuthResults.First(), Is.EqualTo($"{spfAuthResult.Domain}:{spfAuthResult.Result}"));

        }
        
        [Test]
        public void ParserShouldParseMultipleSpfAuthResultsCorrectly()
        {

            SpfAuthResult spfAuthResult = new SpfAuthResult
            {
                Domain = "abc.gov.uk",
                Result = SpfResult.permerror
            };
            
            SpfAuthResult spfAuthResult2 = new SpfAuthResult
            {
                Domain = "cba.gov.uk",
                Result = SpfResult.pass
            };
            
            List<AggregateReportRecord> result = CreateReportInfo(new List<SpfAuthResult>(){spfAuthResult, spfAuthResult2}).ToAggregateReportRecords();

            var firstRecord = result.First();
            
            Assert.That(firstRecord.SpfAuthResults.Count, Is.EqualTo(2));
            Assert.That(firstRecord.SpfAuthResults.First(), Is.EqualTo($"{spfAuthResult.Domain}:{spfAuthResult.Result}"));
            Assert.That(firstRecord.SpfAuthResults.Last(), Is.EqualTo($"{spfAuthResult2.Domain}:{spfAuthResult2.Result}"));

        }

        private AggregateReportInfo CreateReportInfo(List<DkimAuthResult> dkimAuthResults)
        {
            return CreateReportInfo(dkimAuthResults: dkimAuthResults.ToArray());
        }

        private AggregateReportInfo CreateReportInfo(List<SpfAuthResult> spfAuthResults)
        {
            return CreateReportInfo(spfAuthResults: spfAuthResults.ToArray());
        }

        private AggregateReportInfo CreateReportInfo(DkimAuthResult[] dkimAuthResults = null, SpfAuthResult[] spfAuthResults = null)
        {
            Domain.Dmarc.AggregateReport aggregateReport = new Domain.Dmarc.AggregateReport(1.0, 
                new ReportMetadata("org", "a.b@c.org", "extra", "reportid", new DateRange(1, 2), new []{"error"} ),
                new PolicyPublished("abc.com", Alignment.r, Alignment.r, Disposition.none, Disposition.none, 0, "fo"), 
                new Record[]
                {
                    new Record(new Row("source", 1, 
                            new PolicyEvaluated(Disposition.none, DmarcResult.pass, DmarcResult.pass, new PolicyOverrideReason[0])), 
                        new Identifier("envelopeTo", "envelopeFrom", "headerFrom"),
                        new AuthResult(  dkimAuthResults ?? new DkimAuthResult[]{}, spfAuthResults ?? new SpfAuthResult[] {}))
                });
            
            EmailMetadata emailMetadata = new EmailMetadata("org", "filename", 10);
            
            AttachmentInfo attachmentInfo = new AttachmentInfo(new AttachmentMetadata("filename"), Stream.Null);
            
            return new AggregateReportInfo(aggregateReport, emailMetadata, attachmentInfo);
        }
        
    }
}