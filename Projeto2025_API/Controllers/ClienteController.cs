using Dominio.Dtos;
using FluentValidation;
using Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Projeto2025_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;
        private readonly IValidator<ClienteDto> _validator;

        public ClienteController(IClienteService service, IValidator<ClienteDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClienteDto>> addAsync(ClienteDto clienteDto)
        {
            try
            {
                var result = _validator.Validate(clienteDto);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new { message = "Erro de validação", errors = errors });
                }

                var dto = await _service.addAsync(clienteDto);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar cliente", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> getAllAsync()
        {
            var lista = await _service.getAllAsync(p => true);
            return Ok(lista);
        }

        [HttpGet("buscar/{nome}")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> getByNomeAsync(string nome)
        {
            var lista = await _service.getByNomeAsync(nome);
            return Ok(lista);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> getByEmailAsync(string email)
        {
            var lista = await _service.getByEmailAsync(email);
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> getAsync(int id)
        {
            var cliente = await _service.getAsync(id);
            if (cliente == null)
                return NotFound();
            else
                return Ok(cliente);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> deleteAsync(int id)
        {
            await _service.removeAsync(id);
            return NoContent();
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> updateAsync(ClienteDto cliente)
        {
            try
            {
                var result = _validator.Validate(cliente);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new { message = "Erro de validação", errors = errors });
                }

                await _service.updateAsync(cliente);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar cliente", error = ex.Message });
            }
        }
    }
}

