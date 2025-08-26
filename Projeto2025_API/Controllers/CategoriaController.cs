using Azure.Core;
using Dominio.Dtos;
using Interface.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Projeto2025_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {

        private ICategoriaService service;

        public CategoriaController(ICategoriaService 
            service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDto>>
            addAsync (CategoriaDto categoriaDto)
        {

           var dto=  await  this.service.addAsync (categoriaDto);
           return Ok (dto);

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
            await this.service.updateAsync(cat);
            return NoContent();
        
        }

    }
}
