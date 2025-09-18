using FluentValidation;
using LRA.Common.DTOs;

namespace LRA.Common.Validators;

public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
{
    public ChangeEmailRequestValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("Email is reuired")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Maximum length 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
