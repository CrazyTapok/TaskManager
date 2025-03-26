using FluentValidation;
using TaskManager.API.Contracts.Requests;

namespace TaskManager.API.Contracts.Validators;

public class EmailNotificationRequestValidator : AbstractValidator<EmailNotificationRequest>
{
    public EmailNotificationRequestValidator()
    {
        RuleFor(request => request.EmailList)
            .NotEmpty()
            .Must(list => list.All(email => !string.IsNullOrWhiteSpace(email))).WithMessage("Email list contains invalid emails.");

        RuleFor(request => request.Subject)
            .NotEmpty()
            .MaximumLength(140);

        RuleFor(request => request.Body)
            .NotEmpty();

        RuleFor(request => request.CronExpression)
            .NotEmpty()
            .Matches(@"^(?:\d+|\*|\?|\w+)$").WithMessage("Invalid cron expression format.");

        RuleFor(request => request.CreatedByName)
            .NotEmpty()
            .MaximumLength(70);

        RuleFor(request => request.CreatedByEmail)
            .NotEmpty()
            .EmailAddress();
    }
}