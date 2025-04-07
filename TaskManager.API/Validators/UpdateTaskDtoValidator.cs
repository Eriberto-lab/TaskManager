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
               .WithMessage("O título é obrigatório.")
               .MaximumLength(100)
               .WithMessage("O título não pode ultrapassar 100 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("A descrição não pode ultrapassar 500 caracteres.");

            RuleFor(x => x.Status)
                .Must(BeAValidStatus)
                .WithMessage("Status inválido.");

        }
        private bool BeAValidStatus(string status)
        {
            return Enum.TryParse<TaskItemStatus>(status, true, out _);
        }


    }
}