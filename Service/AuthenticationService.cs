using BCrypt.Net;
using Dominio.Dtos;
using Dominio.Entidades;
using Interface.Repositorio;
using Interface.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IUsuarioRepositorio usuarioRepositorio, IConfiguration configuration)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var usuario = await _usuarioRepositorio.getByUserNameAsync(loginRequest.UserName);
            
            if (usuario == null || !await ValidatePasswordAsync(loginRequest.Senha, usuario.SenhaHash))
            {
                return null;
            }

            // Atualizar último login
            usuario.UltimoLogin = DateTime.Now;
            await _usuarioRepositorio.updateAsync(usuario);

            var accessToken = GenerateAccessToken(usuario);
            var refreshToken = GenerateRefreshToken();

            // Salvar refresh token no banco
            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); // 7 dias
            await _usuarioRepositorio.updateAsync(usuario);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 120 * 60, // 120 minutos em segundos
                ExpiresAt = DateTime.Now.AddMinutes(120),
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    UserName = usuario.UserName,
                    Role = usuario.Role,
                    Ativo = usuario.Ativo
                }
            };
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
        {
            var usuario = await _usuarioRepositorio.getByRefreshTokenAsync(refreshTokenRequest.RefreshToken);
            
            if (usuario == null)
            {
                return null;
            }

            var accessToken = GenerateAccessToken(usuario);
            var newRefreshToken = GenerateRefreshToken();

            // Atualizar refresh token
            usuario.RefreshToken = newRefreshToken;
            usuario.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _usuarioRepositorio.updateAsync(usuario);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                ExpiresIn = 120 * 60,
                ExpiresAt = DateTime.Now.AddMinutes(120),
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    UserName = usuario.UserName,
                    Role = usuario.Role,
                    Ativo = usuario.Ativo
                }
            };
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var usuario = await _usuarioRepositorio.getByRefreshTokenAsync(refreshToken);
            
            if (usuario == null)
            {
                return false;
            }

            usuario.RefreshToken = null;
            usuario.RefreshTokenExpiryTime = null;
            await _usuarioRepositorio.updateAsync(usuario);

            return true;
        }

        public async Task<bool> ValidatePasswordAsync(string password, string hash)
        {
            return await Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
        }

        public async Task<string> HashPasswordAsync(string password)
        {
            return await Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12)));
        }

        private string GenerateAccessToken(Usuario usuario)
        {
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not found");
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.UserName),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Role),
                new Claim("user_id", usuario.Id.ToString()),
                new Claim("user_name", usuario.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
