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
    public class VendaController : ControllerBase
    {
        private readonly IVendaService _service;
        private readonly IValidator<VendaDto> _validator;

        public VendaController(IVendaService service, IValidator<VendaDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpPost]
        public async Task<ActionResult<VendaDto>> addAsync(VendaDto vendaDto)
        {
            try
            {
                var result = _validator.Validate(vendaDto);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new { message = "Erro de validação", errors = errors });
                }

                var dto = await _service.addAsync(vendaDto);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar venda", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaDto>>> getAllAsync()
        {
            var lista = await _service.getAllAsync(p => true);
            return Ok(lista);
        }

        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> getByClienteAsync(int idCliente)
        {
            var lista = await _service.getByClienteAsync(idCliente);
            return Ok(lista);
        }

        [HttpGet("periodo")]
        public async Task<ActionResult<IEnumerable<VendaDto>>> getByPeriodoAsync([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            var lista = await _service.getByPeriodoAsync(dataInicio, dataFim);
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendaDto>> getAsync(int id)
        {
            var venda = await _service.getAsync(id);
            if (venda == null)
                return NotFound();
            else
                return Ok(venda);
        }

        [HttpGet("{id}/itens")]
        public async Task<ActionResult<VendaDto>> getWithItensAsync(int id)
        {
            var venda = await _service.getWithItensAsync(id);
            if (venda == null)
                return NotFound();
            else
                return Ok(venda);
        }

        [HttpPut("{id}/finalizar")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VendaDto>> finalizarVendaAsync(int id)
        {
            try
            {
                var venda = await _service.finalizarVendaAsync(id);
                return Ok(venda);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/cancelar")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VendaDto>> cancelarVendaAsync(int id)
        {
            try
            {
                var venda = await _service.cancelarVendaAsync(id);
                return Ok(venda);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
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
        public async Task<ActionResult> updateAsync(VendaDto venda)
        {
            var result = _validator.Validate(venda);
            if (result.IsValid)
            {
                await _service.updateAsync(venda);
                return NoContent();
            }
            else 
                return BadRequest(result);
        }
    }
}

