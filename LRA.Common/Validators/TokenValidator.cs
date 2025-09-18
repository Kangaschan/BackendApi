using FluentValidation;
using LRA.Common.DTOs;

namespace LRA.Common.Validators;

public class TokenValidator : AbstractValidator<TokenDto>
{
    public TokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required")
            .MinimumLength(8).WithMessage("Token must be at least 8 characters");
    }
}
