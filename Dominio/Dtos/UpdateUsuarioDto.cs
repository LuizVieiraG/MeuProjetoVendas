using System.ComponentModel.DataAnnotations;

namespace Dominio.Dtos
{
    public class UpdateUsuarioDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username é obrigatório")]
        [StringLength(50, ErrorMessage = "Username deve ter no máximo 50 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username deve conter apenas letras, números e underscore")]
        public string UserName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Role deve ter no máximo 50 caracteres")]
        public string? Role { get; set; }

        public bool? Ativo { get; set; }
    }
}
