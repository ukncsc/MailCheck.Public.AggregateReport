using FluentValidation;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.Common.Util;

namespace MailCheck.AggregateReport.Api.V2.Validation
{
    public class DataExportRequestValidator : AbstractValidator<DataExportRequest>
    {
        public DataExportRequestValidator(IDomainValidator domainValidator)
        {
            RuleFor(_ => _.Domain)
                .NotNull()
                .WithMessage("A domain is required.")
                .NotEmpty()
                .WithMessage("A domain cannot be empty.")
                .Must(domainValidator.IsValidDomain)
                .WithMessage("A domain must be a valid domain.");

            RuleFor(_ => _.EndDate)
                .GreaterThanOrEqualTo(_ => _.StartDate)
                .WithMessage("An end date must be greater than the start date.");
        }
    }
}