using System.Collections.Generic;
using System.Linq;
using MailCheck.Common.Data;

namespace MailCheck.AggregateReport.Api.V2.Dao
{
    internal static class AggregateReportSqlBuilderExtensions
    {
        private static readonly IDictionary<string, Fieldset> FieldsByCategory = new Dictionary<string, Fieldset>
        {
            [CategoryNames.FullyTrusted] = Fieldsets.FullyTrusted,
            [CategoryNames.PartiallyTrusted] = Fieldsets.PartiallyTrusted,
            [CategoryNames.Untrusted] = Fieldsets.Untrusted,
            [CategoryNames.Quarantined] = Fieldsets.Quarantined,
            [CategoryNames.Rejected] = Fieldsets.Rejected,
        };

        public static ISqlBuilder AddAggregateReportDefaults(this ISqlBuilder builder)
        {
            builder.SetToken(TokenKeys.TotalEmailsSum, GetFieldListForTotal(null).ToSumSql());
            builder.SetToken(TokenKeys.PassSpfSum, GetFieldListFilteredByCategory(null, Fieldsets.PassSpf).ToSumSql());
            builder.SetToken(TokenKeys.PassDkimSum, GetFieldListFilteredByCategory(null, Fieldsets.PassDkim).ToSumSql());
            builder.SetToken(TokenKeys.FailSpfSum, GetFieldListFilteredByCategory(null, Fieldsets.FailSpf).ToSumSql());
            builder.SetToken(TokenKeys.FailDkimSum, GetFieldListFilteredByCategory(null, Fieldsets.FailDkim).ToSumSql());

            builder.SetToken(TokenKeys.TotalEmailsSumFiltered, GetFieldListForTotal(null).ToSumSql());
            builder.SetToken(TokenKeys.PassSpfSumFiltered, GetFieldListFilteredByCategory(null, Fieldsets.PassSpf).ToSumSql());
            builder.SetToken(TokenKeys.PassDkimSumFiltered, GetFieldListFilteredByCategory(null, Fieldsets.PassDkim).ToSumSql());
            builder.SetToken(TokenKeys.FailSpfSumFiltered, GetFieldListFilteredByCategory(null, Fieldsets.FailSpf).ToSumSql());
            builder.SetToken(TokenKeys.FailDkimSumFiltered, GetFieldListFilteredByCategory(null, Fieldsets.FailDkim).ToSumSql());

            builder.SetToken(TokenKeys.FullyTrustedSum, GetFieldListForCategory(CategoryNames.FullyTrusted).ToSumSql());
            builder.SetToken(TokenKeys.PartiallyTrustedSum, GetFieldListForCategory(CategoryNames.PartiallyTrusted).ToSumSql());
            builder.SetToken(TokenKeys.UntrustedSum, GetFieldListForCategory(CategoryNames.Untrusted).ToSumSql());
            builder.SetToken(TokenKeys.QuarantinedSum, GetFieldListForCategory(CategoryNames.Quarantined).ToSumSql());
            builder.SetToken(TokenKeys.RejectedSum, GetFieldListForCategory(CategoryNames.Rejected).ToSumSql());
            
            builder.SetToken(TokenKeys.DeliveredSum, Fieldsets.Delivered.ToSumSql());

            return builder;
        }

        public static ISqlBuilder AddCategoryFilter(this ISqlBuilder builder, string categoryFilter)
        {
            builder.SetToken(TokenKeys.TotalEmailsSumFiltered, GetFieldListForTotal(categoryFilter).ToSumSql());
            builder.SetToken(TokenKeys.PassSpfSumFiltered, GetFieldListFilteredByCategory(categoryFilter, Fieldsets.PassSpf).ToSumSql());
            builder.SetToken(TokenKeys.PassDkimSumFiltered, GetFieldListFilteredByCategory(categoryFilter, Fieldsets.PassDkim).ToSumSql());
            builder.SetToken(TokenKeys.FailSpfSumFiltered, GetFieldListFilteredByCategory(categoryFilter, Fieldsets.FailSpf).ToSumSql());
            builder.SetToken(TokenKeys.FailDkimSumFiltered, GetFieldListFilteredByCategory(categoryFilter, Fieldsets.FailDkim).ToSumSql());
            builder.SetToken(TokenKeys.CategoryFilter, GetCategoryFilter(categoryFilter));

            return builder;
        }

        public static ISqlBuilder AddProviderFilter(this ISqlBuilder builder, string providerFilter)
        {
            builder.SetToken(TokenKeys.ProviderFilter, string.IsNullOrWhiteSpace(providerFilter) ? string.Empty : AggregateReportDaoV2.FilterByProvider);

            return builder;
        }

        public static ISqlBuilder AddIpFilter(this ISqlBuilder builder, string providerFilter)
        {
            builder.SetToken(TokenKeys.IpFilter, string.IsNullOrWhiteSpace(providerFilter) ? string.Empty : AggregateReportDaoV2.FilterByIp);

            return builder;
        }

        private static string GetCategoryFilter(string categoryFilter)
        {
            if (string.IsNullOrWhiteSpace(categoryFilter)) return string.Empty;

            switch (categoryFilter)
            {
                case CategoryNames.FullyTrusted:
                    return AggregateReportDaoV2.FilterByFullyTrusted;
                case CategoryNames.PartiallyTrusted:
                    return AggregateReportDaoV2.FilterByPartiallyTrusted;
                case CategoryNames.Untrusted:
                    return AggregateReportDaoV2.FilterByUntrused;
                case CategoryNames.Quarantined:
                    return AggregateReportDaoV2.FilterByQuarantined;
                case CategoryNames.Rejected:
                    return AggregateReportDaoV2.FilterByRejected;
            }

            return string.Empty;
        }

        private static Fieldset GetFieldListForTotal(string categoryFilter)
        {
            if (string.IsNullOrWhiteSpace(categoryFilter))
            {
                return Fieldsets.All;
            }

            Fieldset fields;
            return FieldsByCategory.TryGetValue(categoryFilter, out fields) ? fields : Fieldsets.All;
        }

        private static Fieldset GetFieldListForCategory(string categoryFilter)
        {
            if (string.IsNullOrWhiteSpace(categoryFilter))
            {
                return Fieldset.Empty;
            }

            Fieldset categoryFields;
            if (!FieldsByCategory.TryGetValue(categoryFilter, out categoryFields))
            {
                return Fieldset.Empty;
            }

            return categoryFields;
        }

        private static Fieldset GetFieldListFilteredByCategory(string categoryFilter, Fieldset fields)
        {
            if (string.IsNullOrWhiteSpace(categoryFilter))
            {
                return fields;
            }

            Fieldset categoryFields;
            if (FieldsByCategory.TryGetValue(categoryFilter, out categoryFields))
            {
                fields = new Fieldset(fields.Intersect(categoryFields));
            }

            return fields;
        }

        static class CategoryNames
        {
            public const string FullyTrusted = "fullyTrusted";
            public const string PartiallyTrusted = "partiallyTrusted";
            public const string Untrusted = "untrusted";
            public const string Quarantined = "quarantined";
            public const string Rejected = "rejected";
        }

        static class TokenKeys
        {
            public const string TotalEmailsSum = "TotalEmailsSum";
            public const string PassSpfSum = "PassSpfSum";
            public const string PassDkimSum = "PassDkimSum";
            public const string FailSpfSum = "FailSpfSum";
            public const string FailDkimSum = "FailDkimSum";
            public const string TotalEmailsSumFiltered = "TotalEmailsSumFiltered";
            public const string PassSpfSumFiltered = "PassSpfSumFiltered";
            public const string PassDkimSumFiltered = "PassDkimSumFiltered";
            public const string FailSpfSumFiltered = "FailSpfSumFiltered";
            public const string FailDkimSumFiltered = "FailDkimSumFiltered";
            public const string FullyTrustedSum = "FullyTrustedSum";
            public const string PartiallyTrustedSum = "PartiallyTrustedSum";
            public const string UntrustedSum = "UntrustedSum";
            public const string QuarantinedSum = "QuarantinedSum";
            public const string RejectedSum = "RejectedSum";
            public const string DeliveredSum = "DeliveredSum";
            public const string CategoryFilter = "CategoryFilter";
            public const string ProviderFilter = "ProviderFilter";
            public const string IpFilter = "IpFilter";
        }
    }
}