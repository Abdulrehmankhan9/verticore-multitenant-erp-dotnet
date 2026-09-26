using FluentValidation;
using VertiCore.Application.DTOs.Task;

namespace VertiCore.Application.Validators.Task
{
    public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
    {
        public CreateTaskRequestValidator()
        {
            RuleFor(request => request.Title).NotEmpty().MaximumLength(160);
            RuleFor(request => request.Description).MaximumLength(2000);
            RuleFor(request => request.AssignedUserId).NotEmpty();
            RuleFor(request => request.DueDate)
                .Must(dueDate => dueDate == null || dueDate > DateTime.UtcNow)
                .WithMessage("Due date must be in the future");
        }
    }
}