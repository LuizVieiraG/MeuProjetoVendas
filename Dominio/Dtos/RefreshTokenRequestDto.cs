using System.ComponentModel.DataAnnotations;

namespace Dominio.Dtos
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh token é obrigatório")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
