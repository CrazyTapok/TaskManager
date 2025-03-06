using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class ProjectRequestValidator : AbstractValidator<ProjectRequest>
{
    public ProjectRequestValidator()
    {
        RuleFor(project => project.Title)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(project => project.ManagerId)
            .NotEmpty().WithMessage("The project must have a manager.");

        RuleFor(project => project.CompanyId)
            .NotEmpty().WithMessage("The project must have a company specified.");
    }
}