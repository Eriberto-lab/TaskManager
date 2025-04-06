using FluentValidation;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.API.Validators
{
    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(100)
                .WithMessage("Title must not exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");
            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now)
                .WithMessage("Due date must be in the future.");
        }
        private bool BeAValidStatus(string status)
        {
            return Enum.TryParse<TaskItemStatus>(status, true, out _);
        }


    }
}