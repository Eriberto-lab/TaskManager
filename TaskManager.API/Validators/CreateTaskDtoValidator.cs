using FluentValidation;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.API.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("O título é obrigatório.")
                .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

            RuleFor(x => x.DueDate)
                .Must(FutureDateOnly)
                .When(x => x.DueDate.HasValue)
                .WithMessage("A data de vencimento deve ser futura (apenas dia/mês/ano).");



        }

        private bool FutureDateOnly(DateTime? dueDate)
        {
            if (!dueDate.HasValue)
                return true;

            var hoje = DateTime.Today;
            var dataInformada = dueDate.Value.Date;

            return dataInformada > hoje;
        }



    }
}
