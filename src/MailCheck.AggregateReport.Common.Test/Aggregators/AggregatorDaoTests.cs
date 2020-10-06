using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace MailCheck.AggregateReport.Common.Aggregators
{
    [TestFixture]
    public class AggregatorDaoTests
    {
        [Test]
        public async Task Save_WithNoRecords_ReturnsSuccess()
        {
            var fakeLogger = A.Fake<ILogger<AggregatorDao<ExampleRecord>>>();
            var fakeSaveOperation = A.Fake<Func<string, IDictionary<string, object>, Task<int>>>();

            var dao = new AggregatorDao<ExampleRecord>(
                fakeLogger,
                null,
                fakeSaveOperation
            );

            var records = new List<ExampleRecord>
            {
            };

            var settings = new AggregatorDao<ExampleRecord>.Settings
            {
                FieldNames = new[] { "domain", "some_number" },
                TableName = "example",
                UpdateStatements = "some_number = some_number + values(some_number)",
                ParameterValuesMapper = r => new Dictionary<string, object>
                {
                    ["domain"] = r.Domain,
                    ["some_number"] = r.SomeNumber
                }
            };

            var outcome = await dao.Save(records, settings);

            Assert.That(outcome, Is.SameAs(Outcome.EmptyRecords));

            A.CallTo(() => fakeSaveOperation(A<string>.Ignored, A<IDictionary<string, object>>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Save_WithRecords_SavesAndReturnsSuccess()
        {
            string capturedSql = null;
            IDictionary<string, object> capturedParameters = null;

            var fakeLogger = A.Fake<ILogger<AggregatorDao<ExampleRecord>>>();
            var fakeSaveOperation = A.Fake<Func<string, IDictionary<string, object>, Task<int>>>();
            A.CallTo(() => fakeSaveOperation.Invoke(A<string>.Ignored, A<IDictionary<string, object>>.Ignored))
                .Invokes((string sql, IDictionary<string, object>  parameters) =>
                {
                    capturedSql = sql;
                    capturedParameters = parameters;
                })
                .Returns(Task.FromResult(10));

            var dao = new AggregatorDao<ExampleRecord>(
                fakeLogger,
                null,
                fakeSaveOperation
            );

            var records = new List<ExampleRecord>
            {
                new ExampleRecord
                {
                    Id = 1,
                    Domain = "blah.com",
                    SomeNumber = 567
                },
                new ExampleRecord
                {
                    Id = 1,
                    Domain = "blah2.com",
                    SomeNumber = 5678
                },
                new ExampleRecord
                {
                    Id = 1,
                    Domain = "blah3.com",
                    SomeNumber = 56789
                },
            };

            var settings = new AggregatorDao<ExampleRecord>.Settings 
            {
                FieldNames = new[] { "domain", "some_number" },
                TableName = "example",
                UpdateStatements = "some_number = some_number + values(some_number)",
                ParameterValuesMapper = r => new Dictionary<string, object>
                {
                    ["domain"] = r.Domain,
                    ["some_number"] = r.SomeNumber
                }
            };

            var expectedParameters = new Dictionary<string, object>
            {
                ["domain_0"] = "blah.com",
                ["some_number_0"] = 567,
                ["domain_1"] = "blah2.com",
                ["some_number_1"] = 5678,
                ["domain_2"] = "blah3.com",
                ["some_number_2"] = 56789,
                ["record_id"] = 1,
            };

            var outcome = await dao.Save(records, settings);

            Assert.That(outcome, Is.SameAs(Outcome.SavedSuccessfully));

            A.CallTo(() => fakeSaveOperation(A<string>.Ignored, A<IDictionary<string, object>>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.That(capturedSql, Is.EqualTo(expectedSql));
            Assert.That(capturedParameters, Is.EquivalentTo(expectedParameters));
        }

        private static readonly string expectedSql = @"
-- This must be wrapped in a transaction

-- Set up a temp table for our new rows, cloning the target table
CREATE TEMPORARY TABLE rows_to_upsert
SELECT *
FROM example
LIMIT 0;

-- Insert our ""new"" rows into the temp table
INSERT INTO rows_to_upsert
(
  domain, some_number
)
VALUES
( @domain_0, @some_number_0 ), ( @domain_1, @some_number_1 ), ( @domain_2, @some_number_2 );

-- Select out the existing record ID if we have a match
SELECT record_id
INTO @existing_record
FROM example_store
WHERE record_id = @record_id
LIMIT 1
FOR UPDATE;

-- Insert our rows into the actual table only if existing is null (we haven't seen this record before)
INSERT INTO example
(
  domain, some_number
)
SELECT * FROM (
  SELECT
    domain as col0, some_number as col1
  FROM rows_to_upsert
  WHERE @existing_record IS NULL
) as dt
ON DUPLICATE KEY UPDATE some_number = some_number + values(some_number);

-- Insert our record ID into the store table
INSERT INTO example_store (record_id)
SELECT @record_id
WHERE @existing_record IS NULL;

DROP TEMPORARY TABLE rows_to_upsert;
";

        public class ExampleRecord : IRecord
        {
            public long Id { get; set; }

            public string Domain { get; set; }

            public int SomeNumber { get; set; }
        }
    }
}
