using FluentValidation;
using MailCheck.Common.Util;
using MailCheck.AggregateReport.Contracts.IpIntelligence;

namespace MailCheck.Intelligence.Api.Validation
{
    public class IpAddressDateRangeRequestValidator : AbstractValidator<IpAddressDateRangeRequest>
    {
        public IpAddressDateRangeRequestValidator()
        {
            RuleFor(_ => _.IpAddress)
                .NotNull()
                .WithMessage("An IP is required.")
                .NotEmpty()
                .WithMessage("An IP cannot be empty.");

            RuleFor(_ => _.EndDate)
                .GreaterThanOrEqualTo(_ => _.StartDate)
                .WithMessage("An end date must be greater than the start date.");
        }
    }
}