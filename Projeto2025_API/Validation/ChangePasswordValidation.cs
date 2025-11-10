using Dominio.Dtos;
using FluentValidation;

namespace Projeto2025_API.Validation
{
    public class ChangePasswordValidation : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidation()
        {
            RuleFor(x => x.SenhaAtual)
                .NotEmpty().WithMessage("Senha atual é obrigatória");

            RuleFor(x => x.NovaSenha)
                .NotEmpty().WithMessage("Nova senha é obrigatória")
                .MinimumLength(6).WithMessage("Nova senha deve ter pelo menos 6 caracteres")
                .MaximumLength(100).WithMessage("Nova senha deve ter no máximo 100 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Nova senha deve conter pelo menos uma letra minúscula, uma maiúscula e um número")
                .NotEqual(x => x.SenhaAtual).WithMessage("Nova senha deve ser diferente da senha atual");

            RuleFor(x => x.ConfirmarNovaSenha)
                .NotEmpty().WithMessage("Confirmação de senha é obrigatória")
                .Equal(x => x.NovaSenha).WithMessage("Confirmação de senha não confere");
        }
    }
}
