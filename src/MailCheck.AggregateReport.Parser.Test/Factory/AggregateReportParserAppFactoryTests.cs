using System.Collections.Generic;
using MailCheck.AggregateReport.Parser.Factory;
using MailCheck.AggregateReport.Parser.Processor;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Parser.Test.Factory
{
    [TestFixture]
    internal class AggregateReportParserAppFactoryTests
    {
        [TestCaseSource(nameof(CreateTestData))]
        public void TestCreation(CommandLineArgs commandLineArgs)
        {
            IFileAggregateReportProcessor fileEmailMessageProcessor = FileAggregateReportProcessorFactory.Create(commandLineArgs);
            Assert.That(fileEmailMessageProcessor, Is.Not.Null);
        }

        public static IEnumerable<TestCaseData> CreateTestData()
        {
            yield return new TestCaseData(new CommandLineArgs(null, null, null, null)).SetName("create file aggregate report parser with null commandline args");
            yield return new TestCaseData(new CommandLineArgs("C:\\", null, null, null)).SetName("create file aggregate report parser with directory commandline args");
            yield return new TestCaseData(new CommandLineArgs(null, "C:\\", null, null)).SetName("create file aggregate report parser with xml directory commandline args");
            yield return new TestCaseData(new CommandLineArgs(null, null, "C:\\", null)).SetName("create file aggregate report parser with csv file commandline args");
            yield return new TestCaseData(new CommandLineArgs(null, null, null, "C:\\")).SetName("create file aggregate report parser with sql file commandline args");
        }
    }
}
