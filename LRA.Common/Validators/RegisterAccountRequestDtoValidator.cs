using FluentValidation;
using LRA.Common.DTOs;

namespace LRA.Common.Validators;

public class RegisterAccountRequestDtoValidator : AbstractValidator<RegisterAccountRequestDto>
{
    public RegisterAccountRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is reuired")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Maximum length 255 characters");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]").WithMessage("Password must contain at least one special character")
            .Matches("^[a-zA-Z0-9!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]*$")
            .WithMessage("Password can only contain English letters, numbers and special characters");
        
        RuleFor(x => x.PasswordConfirmation)
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}
