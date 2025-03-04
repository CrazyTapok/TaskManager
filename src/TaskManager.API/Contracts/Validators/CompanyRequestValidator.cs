using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class CompanyRequestValidator : AbstractValidator<CompanyRequest>
{
    public CompanyRequestValidator()
    {
        RuleFor(company => company.Title)
            .NotEmpty().WithMessage("The company name is required.")
            .MaximumLength(120).WithMessage("The company name must be no longer than 120 characters.");
    }
}
