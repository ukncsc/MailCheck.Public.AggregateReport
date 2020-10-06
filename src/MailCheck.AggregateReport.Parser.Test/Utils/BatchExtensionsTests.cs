using System;
using System.Collections.Generic;
using System.Linq;
using MailCheck.AggregateReport.Contracts;
using MailCheck.AggregateReport.Parser.Publisher;
using MailCheck.Common.Messaging.Common.Exception;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Parser.Test.Utils
{
    [TestFixture]
    public class BatchExtensionsTests
    {
        [Test]
        public void BatchByJsonBytesDoesNotExceedSize()
        {
            const int batchBytes = 200000;
            const int recordCount = 350;

            IEnumerable<AggregateReportRecord> aggregateReportRecords = Enumerable.Range(0, recordCount).Select(x => CreateAggregateReportRecord(x.ToString()));
            List<IEnumerable<AggregateReportRecord>> batches = aggregateReportRecords.BatchByJsonBytes(batchBytes).ToList();

            foreach (IEnumerable<AggregateReportRecord> batch in batches)
            {
                string serializedBatch = JsonConvert.SerializeObject(batch);
                int actualBatchBytes = System.Text.Encoding.Unicode.GetByteCount(serializedBatch);
                Assert.That(actualBatchBytes <= batchBytes);
            }

            IEnumerable<int> allIds = batches.SelectMany(x => x).Select(x => Convert.ToInt32(x.Id)).ToList();
            bool allRecordsPresent = true;
            for (int i = 0; i < recordCount; i++)
            {
                if (!allIds.Contains(i)) allRecordsPresent = false;
            }
            Assert.True(allRecordsPresent);
            Assert.AreEqual(recordCount, allIds.Count());
        }

        [Test]
        public void BatchByJsonBytesThrowsIfElementLargerThanBatch()
        {
            const int maxSizeBytes = 1;

            IEnumerable<AggregateReportRecord> aggregateReportRecords = new List<AggregateReportRecord> { CreateAggregateReportRecord("xyz") };

            MailCheckException exception = Assert.Throws<MailCheckException>(() => aggregateReportRecords.BatchByJsonBytes(maxSizeBytes).ToList());
            Assert.AreEqual("Element xyz too large to batch", exception.Message);
        }

        private AggregateReportRecord CreateAggregateReportRecord(string id)
        {
            return new AggregateReportRecord(id, "testReporterOrgName", "testReportId", DateTime.UnixEpoch,
                "testDomainFrom", Alignment.r, Alignment.r, Policy.none, Policy.none, 100, "testFo", "testHostSourceIp",
                123, Policy.none, DmarcResult.fail, DmarcResult.fail, "testEnvelopeTo", "testEnvelopeFrom",
                "testHeaderFrom", new List<string>() { "testSpfAuthResult1", "testSpfAuthResult2" }, 123, 123,
                new List<string>() { "testDkimAuthResult1", "testDkimAuthResult2" }, 123, 123, true, true, true, false,
                false, false, false);
        }
    }
}
