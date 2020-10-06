using System;
using System.Linq;
using System.Xml.Linq;
using MailCheck.AggregateReport.Parser.Domain.Dmarc;
using MailCheck.AggregateReport.Parser.Serialisation.AggregateReportDeserialisation;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    [TestFixture]
    public class DkimAuthResultDeserialiserTests
    {
        private DkimAuthResultDeserialiser _dkimAuthResultDeserialiser;

        [SetUp]
        public void SetUp()
        {
            _dkimAuthResultDeserialiser = new DkimAuthResultDeserialiser();
        }

        [Test]
        public void DomainTagMustExistIfResultIsntNone()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.NoDomainTag);
            var exception = Assert.Throws<ArgumentException>(() => _dkimAuthResultDeserialiser.Deserialise(new [] {xElement}));
            Assert.That(exception.Message, Is.EqualTo("Expected element 'domain' was not found"));
        }

        [Test]
        public void DomainTagIsOptionalIfResultIsNone()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.NoDomainTagResultNone);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults, Has.Length.Zero);
        }

        [Test]
        public void ResultTagMustExist()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.NoResultTag);
            var exception = Assert.Throws<ArgumentException>(() => _dkimAuthResultDeserialiser.Deserialise(new[] { xElement }));
            Assert.That(exception.Message, Is.EqualTo("Expected element 'result' was not found"));
        }

        [Test]
        public void HumanResultTagIsOptional()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.NoHumanResultTag);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().HumanResult, Is.Null);
        }

        [Test]
        public void SingleCorrectlyFormedDkimAuthResultGeneratesDkimAuthResult()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.StandardDkimAuthResult);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});

            Assert.That(dkimAuthResults.First().Domain, Is.EqualTo(TestConstants.ExpectedDomain));
            Assert.That(dkimAuthResults.First().Result, Is.EqualTo(TestConstants.ExpectedDkimResult));
            Assert.That(dkimAuthResults.First().HumanResult, Is.EqualTo(TestConstants.ExpectedHumanResult));
        }

        [Test]
        public void MultipleCorrectlyFormedDkimAuthResultGeneratesMultipleDkimAuthResults()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.StandardDkimAuthResult);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new[] { xElement, xElement });

            Assert.That(dkimAuthResults.Length, Is.EqualTo(2));
        }

        [Test]
        public void DomainValueOptional()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.NoDomainValue);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().Domain, Is.Empty);
        }

        [Test]
        public void ResultValueOptional()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.NoResultValue);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().Result, Is.Null);
        }

        [Test]
        public void HumanResultValueOptional()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.NoHumanResultValue);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().HumanResult, Is.Empty);
        }

        [Test]
        public void InvalidResultValueNull()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.InvalidResultValue);
            DkimAuthResult[] dkimAuthResults = _dkimAuthResultDeserialiser.Deserialise(new [] {xElement});
            Assert.That(dkimAuthResults.First().Result, Is.Null);
        }

        [Test]
        public void MustAllBeDkimAuthResults()
        {
            XElement xElement1 = XElement.Parse(DkimAuthResultDeserialiserTestsResources.StandardDkimAuthResult);
            XElement xElement2 = XElement.Parse(DkimAuthResultDeserialiserTestsResources.NotDkim);
            Assert.Throws<ArgumentException>(() => _dkimAuthResultDeserialiser.Deserialise(new[] { xElement1, xElement2 }));
        }

        [Test]
        public void TagsMustBeDirectDecendants()
        {
            XElement xElement = XElement.Parse(DkimAuthResultDeserialiserTestsResources.NotDirectDescendants);
            var exception = Assert.Throws<ArgumentException>(() => _dkimAuthResultDeserialiser.Deserialise(new[] { xElement }));
            Assert.That(exception.Message, Is.EqualTo("Expected element 'result' was not found"));
        }
    }
}
