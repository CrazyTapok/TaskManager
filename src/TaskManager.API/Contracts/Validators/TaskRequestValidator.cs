using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class TaskRequestValidator : AbstractValidator<TaskRequest>
{
    public TaskRequestValidator()
    {
        RuleFor(task => task.Title)
            .NotEmpty().WithMessage("The task name is required.")
            .MaximumLength(100).WithMessage("The task name must not exceed 100 characters.");

        RuleFor(task => task.Description)
            .NotEmpty().WithMessage("The task description is required.")
            .MaximumLength(500).WithMessage("The task description should not exceed 500 characters.");

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