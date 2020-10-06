using System;
using System.Collections.Generic;
using MailCheck.Common.Data;

namespace MailCheck.AggregateReport.Common.Aggregators
{
    public static class AggregatorSqlBuilderExtensions
    {
        public static SqlBuilder AddAggregatorTokens(this SqlBuilder builder, AggregatorSqlBuilderSettings settings, int recordCount)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.TableName is null)
            {
                throw new ArgumentException("TableName property required", nameof(settings));
            }

            if (settings.FieldNames is null)
            {
                throw new ArgumentException("FieldNames property required", nameof(settings));
            }

            var fields = new Fieldset(settings.FieldNames);

            builder.SetToken("TableName", settings.TableName);
            builder.SetToken("StoreTableName", settings.StoreTableName ?? $"{settings.TableName}_store");
            builder.SetToken("ColumnList", fields.ToFieldListSql());
            builder.SetToken("AliasedColumnList", fields.ToAliasedFieldListSql());
            builder.SetToken("UpdateClause", settings.UpdateStatements == null ? "" : $"ON DUPLICATE KEY UPDATE {settings.UpdateStatements}");

            builder.SetToken("ValuesList", fields.ToValuesParameterListSql(recordCount));

            return builder;
        }
    }

    public class AggregatorSqlBuilderSettings
    {
        public string TableName { get; set; }

        public string StoreTableName { get; set; }

        public IEnumerable<string> FieldNames { get; set; }

        public string UpdateStatements { get; set; }
    }
}
