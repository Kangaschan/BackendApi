using FluentValidation;
using LRA.Common.DTOs;

namespace LRA.Common.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("New Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("New Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("New Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("New Password must contain at least one number")
            .Matches("[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]").WithMessage("New Password must contain at least one special character")
            .Matches("^[a-zA-Z0-9!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]*$")
            .WithMessage("New Password can only contain English letters, numbers and special characters")
            .Must((model, newPassword) => newPassword != model.OldPassword)
            .WithMessage("New password must be different from old password");
        
        RuleFor(x => x.NewPasswordConfirmation)
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
    }
}
