using MailCheck.Common.Data;
using NUnit.Framework;

namespace MailCheck.AggregateReport.Common.Aggregators
{
    [TestFixture]
    public class AggregatorSqlBuilderExtensionsTests
    {
        [Test]
        public void AddAggregatorTokens_Requireds()
        {
            AggregatorSqlBuilderSettings settings = new AggregatorSqlBuilderSettings
            {
                TableName = "TableName",
                FieldNames = new[] {"field1", "field2", "field3"},
            };
            
            int recordCount = 2;

            var builder = new SqlBuilder()
                .AddAggregatorTokens(settings, recordCount);

            Assert.That(builder.Build("{TableName}"), Is.EqualTo("TableName"));
            Assert.That(builder.Build("{StoreTableName}"), Is.EqualTo("TableName_store"));
            Assert.That(builder.Build("{ColumnList}"), Is.EqualTo("field1, field2, field3"));
            Assert.That(builder.Build("{AliasedColumnList}"), Is.EqualTo("field1 as col0, field2 as col1, field3 as col2"));
            Assert.That(builder.Build("{UpdateClause}"), Is.EqualTo(""));
            Assert.That(builder.Build("{ValuesList}"), Is.EqualTo("( @field1_0, @field2_0, @field3_0 ), ( @field1_1, @field2_1, @field3_1 )"));
        }

        [Test]
        public void AddAggregatorTokens_Optionals()
        {
            AggregatorSqlBuilderSettings settings = new AggregatorSqlBuilderSettings
            {
                TableName = "TableName",
                FieldNames = new[] { "field1", "field2", "field3" },
                StoreTableName = "OtherStoreTable",
                UpdateStatements = "field1 = field1 + values(field1), field2 = field2 - values(field2)"
            };

            int recordCount = 2;

            var builder = new SqlBuilder()
                .AddAggregatorTokens(settings, recordCount);

            Assert.That(builder.Build("{TableName}"), Is.EqualTo("TableName"));
            Assert.That(builder.Build("{StoreTableName}"), Is.EqualTo("OtherStoreTable"));
            Assert.That(builder.Build("{UpdateClause}"), Is.EqualTo("ON DUPLICATE KEY UPDATE field1 = field1 + values(field1), field2 = field2 - values(field2)"));            
        }
    }
}

