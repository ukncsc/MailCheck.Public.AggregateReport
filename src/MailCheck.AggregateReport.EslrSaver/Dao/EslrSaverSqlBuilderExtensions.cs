using System;
using MailCheck.Common.Data;

namespace MailCheck.AggregateReport.EslrSaver.Dao
{
    public static class EslrSaverSqlBuilderExtensions
    {
        public static SqlBuilder AddEslrSaverTokens(this SqlBuilder builder, EslrSaverSqlBuilderSettings settings)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.FieldNames is null)
            {
                throw new ArgumentException("FieldNames property required", nameof(settings));
            }

            var fields = new Fieldset(settings.FieldNames);

            builder.SetToken("ColumnList", fields.ToFieldListSql());
            builder.SetToken("AliasedParameterList", fields.ToAliasedParameterListSql());

            return builder;
        }
    }
}