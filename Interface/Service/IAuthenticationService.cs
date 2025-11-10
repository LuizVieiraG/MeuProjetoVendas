using Dominio.Dtos;

namespace Interface.Service
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<bool> ValidatePasswordAsync(string password, string hash);
        Task<string> HashPasswordAsync(string password);
    }
}
