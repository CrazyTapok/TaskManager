using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator()
    {
        RuleFor(employee => employee.Name)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(employee => employee.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(employee => employee.Password)
            .NotEmpty()
            .MaximumLength(15)
            .MinimumLength(6);

        RuleFor(employee => employee.Role)
            .IsInEnum().WithMessage("The employee's role is incorrect.");
    }
}
