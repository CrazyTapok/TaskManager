using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
{
    public EmployeeRequestValidator()
    {
        RuleFor(employee => employee.Name)
            .NotEmpty().WithMessage("The employee's name is required.")
            .MaximumLength(10).WithMessage("The employee's name must be no longer than 10 characters.");

        RuleFor(employee => employee.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("The email must be correct.");

        RuleFor(employee => employee.Password)
            .NotEmpty().WithMessage("A password is required.")
            .MaximumLength(15).WithMessage("The password must be no more than 15 characters long.")
            .MinimumLength(6).WithMessage("The password must be at least 6 characters long.");

        RuleFor(employee => employee.Role)
            .IsInEnum().WithMessage("The employee's role is incorrect.");
    }
}
