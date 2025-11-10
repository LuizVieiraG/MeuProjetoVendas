using Dominio.Dtos;
using FluentValidation;
using Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Projeto2025_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _service;
        private readonly IValidator<ProdutoDto> _validator;

        public ProdutoController(IProdutoService service, IValidator<ProdutoDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProdutoDto>> addAsync(ProdutoDto produtoDto)
        {
            var result = _validator.Validate(produtoDto);
            if (result.IsValid)
            {
                var dto = await _service.addAsync(produtoDto);
                return Ok(dto);
            }
            else
                return BadRequest(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> getAllAsync()
        {
            var lista = await _service.getAllAsync(p => true);
            return Ok(lista);
        }

        [HttpGet("categoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> getByCategoriaAsync(int idCategoria)
        {
            var lista = await _service.getByCategoriaAsync(idCategoria);
            return Ok(lista);
        }

        [HttpGet("marca/{marca}")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> getByMarcaAsync(string marca)
        {
            var lista = await _service.getByMarcaAsync(marca);
            return Ok(lista);
        }

        [HttpGet("estoque")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> getProdutosEmEstoqueAsync()
        {
            var lista = await _service.getProdutosEmEstoqueAsync();
            return Ok(lista);
        }

        [HttpGet("buscar/{termo}")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> buscarProdutosAsync(string termo)
        {
            var lista = await _service.buscarProdutosAsync(termo);
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDto>> getAsync(int id)
        {
            var produto = await _service.getAsync(id);
            if (produto == null)
                return NotFound();
            else
                return Ok(produto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> deleteAsync(int id)
        {
            try
            {
                await _service.removeAsync(id);
                return NoContent();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Erro de violação de chave estrangeira
                return BadRequest(new { message = "Não é possível excluir este produto pois ele está sendo utilizado em vendas." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao excluir produto: {ex.Message}" });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> updateAsync(ProdutoDto produto)
        {
            var result = _validator.Validate(produto);
            if (result.IsValid)
            {
                await _service.updateAsync(produto);
                return NoContent();
            }
            else 
                return BadRequest(result);
        }

        [HttpPut("estoque/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> atualizarEstoqueAsync(int id, [FromBody] int quantidade)
        {
            var sucesso = await _service.atualizarEstoqueAsync(id, quantidade);
            if (sucesso)
                return Ok();
            else
                return BadRequest("Erro ao atualizar estoque");
        }
    }
}

