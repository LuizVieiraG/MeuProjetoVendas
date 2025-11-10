using Dominio.Dtos;
using Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Projeto2025_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegurancaController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public SegurancaController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginRequest);

            if (result == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            return Ok(result);
        }

        [HttpOptions("login")]
        [AllowAnonymous]
        public IActionResult PreflightLogin()
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
            Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RefreshTokenAsync(refreshTokenRequest);

            if (result == null)
            {
                return Unauthorized(new { message = "Refresh token inválido ou expirado" });
            }

            return Ok(result);
        }

        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        {
            var success = await _authService.RevokeTokenAsync(refreshTokenRequest.RefreshToken);

            if (!success)
            {
                return BadRequest(new { message = "Refresh token inválido" });
            }

            return Ok(new { message = "Token revogado com sucesso" });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        [HttpPost]
        [Obsolete("Use /login em vez deste endpoint")]
        public async Task<IActionResult> LoginLegacy([FromBody] UsuarioDTO loginDetalhes)
        {
            var loginRequest = new LoginRequestDto
            {
                UserName = loginDetalhes.User,
                Senha = loginDetalhes.Senha
            };

            return await Login(loginRequest);
        }
    }
}
