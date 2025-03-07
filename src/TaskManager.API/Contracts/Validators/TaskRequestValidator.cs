using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class TaskRequestValidator : AbstractValidator<TaskRequest>
{
    public TaskRequestValidator()
    {
        RuleFor(task => task.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(task => task.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(task => task.Status)
            .IsInEnum().WithMessage("The issue status is incorrect.");

        RuleFor(task => task.ProjectId)
            .NotEmpty().WithMessage("The task must be linked to the project.");

        RuleFor(task => task.CreateEmployeeId)
            .NotEmpty().WithMessage("The creator of the issue is required.");

        RuleFor(task => task.AssignedEmployeeId)
            .NotEmpty().WithMessage("The task must be assigned to an employee.");

        RuleFor(task => task.CreatedDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("The creation date cannot be in the future.");

        RuleFor(task => task.UpdatedDate)
            .GreaterThanOrEqualTo(task => task.CreatedDate).WithMessage("The update date cannot be earlier than the creation date.");
    }
}