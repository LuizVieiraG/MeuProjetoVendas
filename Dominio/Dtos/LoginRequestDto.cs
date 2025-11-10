using System.ComponentModel.DataAnnotations;

namespace Dominio.Dtos
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username é obrigatório")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; } = string.Empty;
    }
}
