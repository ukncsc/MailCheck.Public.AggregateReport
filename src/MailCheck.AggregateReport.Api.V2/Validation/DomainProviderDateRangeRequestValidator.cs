using FluentValidation;
using MailCheck.AggregateReport.Api.V2.Domain;
using MailCheck.Common.Util;

namespace MailCheck.AggregateReport.Api.V2.Validation
{
    public class DomainProviderDateRangeRequestValidator : AbstractValidator<DomainProviderDateRangeRequest>
    {
        public DomainProviderDateRangeRequestValidator(IDomainValidator domainValidator)
        {
            RuleFor(_ => _.Domain)
                .NotNull()
                .WithMessage("A domain is required.")
                .NotEmpty()
                .WithMessage("A domain cannot be empty.")
                .Must(domainValidator.IsValidDomain)
                .WithMessage("A domain must be a valid domain.");

            RuleFor(_ => _.Provider)
                .NotNull()
                .WithMessage("A provider is required.")
                .NotEmpty()
                .WithMessage("A provider cannot be empty.");

            RuleFor(_ => _.EndDate)
                .GreaterThanOrEqualTo(_ => _.StartDate)
                .WithMessage("An end date must be greater than the start date.");
        }
    }
}