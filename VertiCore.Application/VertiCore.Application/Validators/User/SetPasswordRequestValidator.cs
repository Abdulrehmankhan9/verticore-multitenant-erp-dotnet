using FluentValidation;
using VertiCore.Application.DTOs.User;

namespace VertiCore.Application.Validators.User
{
    public class SetPasswordRequestValidator : AbstractValidator<SetPasswordRequest>
    {
        public SetPasswordRequestValidator()
        {
            RuleFor(request => request.Token)
                .NotEmpty().WithMessage("Invitation token is required");

            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters");
        }
    }
}