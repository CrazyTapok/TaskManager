using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class EmailNotificationRequestValidator : AbstractValidator<EmailNotificationRequest>
{
    public EmailNotificationRequestValidator()
    {
        RuleFor(request => request.Subject)
            .MaximumLength(140);

        RuleFor(request => request.RecipientName)
            .NotEmpty()
            .MaximumLength(70);

        RuleFor(request => request.RecipientEmail)
            .NotEmpty()
            .EmailAddress();
    }
}