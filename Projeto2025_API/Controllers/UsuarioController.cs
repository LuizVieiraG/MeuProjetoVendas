using Dominio.Dtos;
using Dominio.Entidades;
using Interface.Repositorio;
using Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Projeto2025_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IAuthenticationService _authService;

        public UsuarioController(IUsuarioRepositorio usuarioRepositorio, IAuthenticationService authService)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _authService = authService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAllAsync()
        {
            try
            {
                var usuarios = await _usuarioRepositorio.getAllAsync();
                var usuariosDto = usuarios.Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Email = u.Email,
                    UserName = u.UserName,
                    Role = u.Role,
                    Ativo = u.Ativo
                });

                return Ok(usuariosDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetByIdAsync(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                // Apenas Admin ou o próprio usuário pode ver os dados
                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return Forbid("Você só pode visualizar seus próprios dados");
                }

                var usuario = await _usuarioRepositorio.getAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                var usuarioDto = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    UserName = usuario.UserName,
                    Role = usuario.Role,
                    Ativo = usuario.Ativo
                };

                return Ok(usuarioDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<UsuarioDto>> GetByUsernameAsync(string username)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var usuario = await _usuarioRepositorio.getByUserNameAsync(username);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                // Apenas Admin ou o próprio usuário pode ver os dados
                if (currentUserRole != "Admin" && currentUserId != usuario.Id)
                {
                    return Forbid("Você só pode visualizar seus próprios dados");
                }

                var usuarioDto = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    UserName = usuario.UserName,
                    Role = usuario.Role,
                    Ativo = usuario.Ativo
                };

                return Ok(usuarioDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioDto>> CreateAsync([FromBody] CreateUsuarioDto createUsuarioDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar se username já existe
                var existingUser = await _usuarioRepositorio.getByUserNameAsync(createUsuarioDto.UserName);
                if (existingUser != null)
                {
                    return Conflict(new { message = "Username já está em uso" });
                }

                // Verificar se email já existe
                var existingEmail = await _usuarioRepositorio.getByEmailAsync(createUsuarioDto.Email);
                if (existingEmail != null)
                {
                    return Conflict(new { message = "Email já está em uso" });
                }

                // Hash da senha
                var senhaHash = await _authService.HashPasswordAsync(createUsuarioDto.Senha);

                var usuario = new Usuario
                {
                    Nome = createUsuarioDto.Nome,
                    Email = createUsuarioDto.Email,
                    UserName = createUsuarioDto.UserName,
                    SenhaHash = senhaHash,
                    Role = createUsuarioDto.Role ?? "User",
                    Ativo = true,
                    DataCriacao = DateTime.Now
                };

                var usuarioCriado = await _usuarioRepositorio.addAsync(usuario);

                var usuarioDto = new UsuarioDto
                {
                    Id = usuarioCriado.Id,
                    Nome = usuarioCriado.Nome,
                    Email = usuarioCriado.Email,
                    UserName = usuarioCriado.UserName,
                    Role = usuarioCriado.Role,
                    Ativo = usuarioCriado.Ativo
                };

                return CreatedAtAction(nameof(GetByIdAsync), new { id = usuarioCriado.Id }, usuarioDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioDto>> UpdateAsync(int id, [FromBody] UpdateUsuarioDto updateUsuarioDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                var usuario = await _usuarioRepositorio.getAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                // Apenas Admin ou o próprio usuário pode atualizar
                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return Forbid("Você só pode atualizar seus próprios dados");
                }

                // Verificar se username já existe (exceto para o próprio usuário)
                if (updateUsuarioDto.UserName != usuario.UserName)
                {
                    var existingUser = await _usuarioRepositorio.getByUserNameAsync(updateUsuarioDto.UserName);
                    if (existingUser != null)
                    {
                        return Conflict(new { message = "Username já está em uso" });
                    }
                }

                // Verificar se email já existe (exceto para o próprio usuário)
                if (updateUsuarioDto.Email != usuario.Email)
                {
                    var existingEmail = await _usuarioRepositorio.getByEmailAsync(updateUsuarioDto.Email);
                    if (existingEmail != null)
                    {
                        return Conflict(new { message = "Email já está em uso" });
                    }
                }

                // Atualizar dados
                usuario.Nome = updateUsuarioDto.Nome;
                usuario.Email = updateUsuarioDto.Email;
                usuario.UserName = updateUsuarioDto.UserName;

                // Apenas Admin pode alterar role
                if (currentUserRole == "Admin" && !string.IsNullOrEmpty(updateUsuarioDto.Role))
                {
                    usuario.Role = updateUsuarioDto.Role;
                }

                // Apenas Admin pode alterar status ativo
                if (currentUserRole == "Admin" && updateUsuarioDto.Ativo.HasValue)
                {
                    usuario.Ativo = updateUsuarioDto.Ativo.Value;
                }

                var usuarioAtualizado = await _usuarioRepositorio.updateAsync(usuario);

                var usuarioDto = new UsuarioDto
                {
                    Id = usuarioAtualizado.Id,
                    Nome = usuarioAtualizado.Nome,
                    Email = usuarioAtualizado.Email,
                    UserName = usuarioAtualizado.UserName,
                    Role = usuarioAtualizado.Role,
                    Ativo = usuarioAtualizado.Ativo
                };

                return Ok(usuarioDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}/alterar-senha")]
        public async Task<ActionResult> ChangePasswordAsync(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = GetCurrentUserId();

                // Apenas o próprio usuário pode alterar a senha
                if (currentUserId != id)
                {
                    return Forbid("Você só pode alterar sua própria senha");
                }

                var usuario = await _usuarioRepositorio.getAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                // Verificar senha atual
                var senhaAtualValida = await _authService.ValidatePasswordAsync(changePasswordDto.SenhaAtual, usuario.SenhaHash);
                if (!senhaAtualValida)
                {
                    return BadRequest(new { message = "Senha atual incorreta" });
                }

                // Hash da nova senha
                var novaSenhaHash = await _authService.HashPasswordAsync(changePasswordDto.NovaSenha);
                usuario.SenhaHash = novaSenhaHash;

                await _usuarioRepositorio.updateAsync(usuario);

                return Ok(new { message = "Senha alterada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}/desativar")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateAsync(int id)
        {
            try
            {
                var usuario = await _usuarioRepositorio.getAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                usuario.Ativo = false;
                await _usuarioRepositorio.updateAsync(usuario);

                return Ok(new { message = "Usuário desativado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}/ativar")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActivateAsync(int id)
        {
            try
            {
                var usuario = await _usuarioRepositorio.getAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                usuario.Ativo = true;
                await _usuarioRepositorio.updateAsync(usuario);

                return Ok(new { message = "Usuário ativado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                var usuario = await _usuarioRepositorio.getAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                await _usuarioRepositorio.removeAsync(id);

                return Ok(new { message = "Usuário removido com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<ActionResult<UsuarioDto>> GetMyProfileAsync()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var usuario = await _usuarioRepositorio.getAsync(currentUserId);
                
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                var usuarioDto = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    UserName = usuario.UserName,
                    Role = usuario.Role,
                    Ativo = usuario.Ativo
                };

                return Ok(usuarioDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        #region Helper Methods

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("user_id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        }

        #endregion
    }
}
