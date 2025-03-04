using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class ProjectRequestValidator : AbstractValidator<ProjectRequest>
{
    public ProjectRequestValidator()
    {
        RuleFor(project => project.Title)
            .NotEmpty().WithMessage("The project name is required.")
            .MinimumLength(2).WithMessage("The project name must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("The project name should be no longer than 100 characters.");

        RuleFor(project => project.ManagerId)
            .NotEmpty().WithMessage("The project must have a manager.");

        RuleFor(project => project.CompanyId)
            .NotEmpty().WithMessage("The project must have a company specified.");

    }
}