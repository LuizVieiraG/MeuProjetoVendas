using Dominio.Dtos;
using FluentValidation;

namespace Projeto2025_API.Validation
{
    public class CategoriaValidation
        :AbstractValidator<CategoriaDto>
    {
        public CategoriaValidation() {
            RuleFor(p=>p.Descricao)
                .MaximumLength(150)
                .WithMessage("A descrição precisa ter no" +
                " máximo 150 caracteres!");

            RuleFor(p => p.Descricao)
                .NotEmpty()
                .WithMessage("A descrição não pode ser vazia!");

            RuleFor(p => p.Descricao)
                .NotNull()
                .WithMessage("A descrição não pode ser vazia!");

        }
    }
}
