using Azure.Core;
using Dominio.Dtos;
using FluentValidation;
using Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Projeto2025_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriaController : ControllerBase
    {

        private ICategoriaService service;
        private IValidator<CategoriaDto> validator;

        public CategoriaController(ICategoriaService 
            service, IValidator<CategoriaDto> validator)
        {
            this.service = service;
            this.validator = validator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoriaDto>>
            addAsync (CategoriaDto categoriaDto)
        {

            var result = validator.Validate(categoriaDto);
            if (result.IsValid)
            {
                var dto = await this.service.addAsync(categoriaDto);
                return Ok(dto);
            }
            else
                return BadRequest(result);
           

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>>
            getAllAsync()
        {
           var lista= await this.service.getAllAsync(p=>true);
           return Ok (lista);  

        }


        [HttpGet("/filtrar/{descricao}")]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>>
           getDescricaoAsync(string descricao)
        {
            var lista = await this.service.getAllAsync(
                p => p.Descricao.Contains(descricao));
            return Ok(lista);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>>
            getAsync(int id)
        {
            var cat = await this.service.getAsync(id);
            if (cat == null)
                return NotFound(); //não encontrou
            else
                return Ok(cat);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteAsync(int id) { 
        
            await this.service.removeAsync (id);
            return NoContent(); //sem retorno
        }

        [HttpPut]
        public async Task<ActionResult>
            updateAsync(CategoriaDto cat) {


            var result = validator.Validate(cat);
            if (result.IsValid)
            {
                await this.service.updateAsync(cat);
                return NoContent();
            }
            else return BadRequest(result);
        
        }

    }
}
