using FluentValidation;
using LRA.Common.Enums;
using LRA.Gateways.Admin.DTOs;

namespace LRA.Gateways.Admin.Validators;

public class JobApplicationProcessDtoValidator : AbstractValidator<JobApplicationProcessDto>
{
    public JobApplicationProcessDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEqual(KycStatusEnum.Pending)
            .WithMessage("Status cannot be Pending");
            
        When(x => x.Status == KycStatusEnum.Rejected, () =>
        {
            RuleFor(x => x.RejectReason)
                .NotEmpty().WithMessage("RejectReason is required when status is Rejected")
                .MaximumLength(1024).WithMessage("RejectReason cannot be longer than 1024 characters");
        });
    }
}
