using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(login => login.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(login => login.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(18);
    }
}