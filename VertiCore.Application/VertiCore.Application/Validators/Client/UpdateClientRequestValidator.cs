using FluentValidation;
using VertiCore.Application.DTOs.Client;

namespace VertiCore.Application.Validators.Client
{
    public class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
    {
        public UpdateClientRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Client name is required")
                .MinimumLength(2).WithMessage("Client name must be at least 2 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required");
        }
    }
}