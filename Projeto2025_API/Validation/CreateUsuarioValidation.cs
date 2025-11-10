using Dominio.Dtos;
using FluentValidation;

namespace Projeto2025_API.Validation
{
    public class CreateUsuarioValidation : AbstractValidator<CreateUsuarioDto>
    {
        public CreateUsuarioValidation()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres")
                .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username é obrigatório")
                .MaximumLength(50).WithMessage("Username deve ter no máximo 50 caracteres")
                .MinimumLength(3).WithMessage("Username deve ter pelo menos 3 caracteres")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username deve conter apenas letras, números e underscore");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres")
                .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Senha deve conter pelo menos uma letra minúscula, uma maiúscula e um número");

            RuleFor(x => x.Role)
                .MaximumLength(50).WithMessage("Role deve ter no máximo 50 caracteres")
                .Must(role => string.IsNullOrEmpty(role) || IsValidRole(role)).WithMessage("Role inválida");
        }

        private bool IsValidRole(string role)
        {
            var validRoles = new[] { "Admin", "User", "Manager", "Vendedor" };
            return validRoles.Contains(role);
        }
    }
}
