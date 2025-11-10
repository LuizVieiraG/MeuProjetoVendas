using Dominio.Dtos;
using Dominio.Entidades;
using Interface.Repositorio;
using Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Projeto2025_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IAuthenticationService _authService;

        public AdminController(IUsuarioRepositorio usuarioRepositorio, IAuthenticationService authService)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _authService = authService;
        }

        [HttpPost("create-admin")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioDto>> CreateAdminAsync([FromBody] CreateAdminDto createAdminDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar se já existe um admin
                var existingAdmin = await _usuarioRepositorio.getByUserNameAsync(createAdminDto.UserName);
                if (existingAdmin != null)
                {
                    return Conflict(new { message = "Usuário administrador já existe" });
                }

                // Hash da senha
                var senhaHash = await _authService.HashPasswordAsync(createAdminDto.Senha);

                var admin = new Usuario
                {
                    Nome = createAdminDto.Nome,
                    Email = createAdminDto.Email,
                    UserName = createAdminDto.UserName,
                    SenhaHash = senhaHash,
                    Role = "Admin",
                    Ativo = true,
                    DataCriacao = DateTime.Now
                };

                var adminCriado = await _usuarioRepositorio.addAsync(admin);

                var adminDto = new UsuarioDto
                {
                    Id = adminCriado.Id,
                    Nome = adminCriado.Nome,
                    Email = adminCriado.Email,
                    UserName = adminCriado.UserName,
                    Role = adminCriado.Role,
                    Ativo = adminCriado.Ativo
                };

                return Ok(new { 
                    message = "Usuário administrador criado com sucesso!",
                    usuario = adminDto,
                    credenciais = new {
                        username = createAdminDto.UserName,
                        senha = createAdminDto.Senha
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStatsAsync()
        {
            try
            {
                var usuarios = await _usuarioRepositorio.getAllAsync();
                var usuariosAtivos = usuarios.Count(u => u.Ativo);
                var admins = usuarios.Count(u => u.Role == "Admin" && u.Ativo);
                var usuariosComuns = usuarios.Count(u => u.Role == "User" && u.Ativo);

                return Ok(new
                {
                    totalUsuarios = usuarios.Count(),
                    usuariosAtivos,
                    admins,
                    usuariosComuns,
                    ultimaAtualizacao = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}/reset-password")]
        public async Task<ActionResult> ResetPasswordAsync(int id, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuario = await _usuarioRepositorio.getAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                // Hash da nova senha
                var novaSenhaHash = await _authService.HashPasswordAsync(resetPasswordDto.NovaSenha);
                usuario.SenhaHash = novaSenhaHash;

                await _usuarioRepositorio.updateAsync(usuario);

                return Ok(new { message = "Senha resetada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }

    public class CreateAdminDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string NovaSenha { get; set; } = string.Empty;
    }
}
