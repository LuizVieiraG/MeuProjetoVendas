using System.ComponentModel.DataAnnotations;

namespace Dominio.Dtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Senha atual é obrigatória")]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Nova senha deve ter entre 6 e 100 caracteres")]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare(nameof(NovaSenha), ErrorMessage = "Confirmação de senha não confere")]
        public string ConfirmarNovaSenha { get; set; } = string.Empty;
    }
}
