using AutoMapper;
using Dominio.Dtos;
using Dominio.Entidades;
using Interface.Repositorio;
using Interface.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CategoriaService : ICategoriaService
    {

        private ICategoriaRepositorio repositorio;

        private IMapper mapper;

        public CategoriaService(ICategoriaRepositorio repositorio,
            IMapper mapper)
        {
            this.repositorio = repositorio;
            this.mapper = mapper;
        }

        public async Task<CategoriaDto> addAsync(CategoriaDto categoria)
        {
            var entidade = mapper.Map<Categoria>(categoria);
            entidade = await this.repositorio.addAsync(entidade);
            return mapper.Map<CategoriaDto>(entidade);

        }

        public async Task<IEnumerable<CategoriaDto>> getAllAsync(Expression<Func<Categoria, bool>> expression)
        {
            var listaCat =
                await this.repositorio.getAllAsync(expression);
            return mapper.Map<IEnumerable<CategoriaDto>>(listaCat);
        }

        public async Task<CategoriaDto?> getAsync(int id)
        {
            var cat = await this.repositorio.getAsync(id);
            return mapper.Map<CategoriaDto>(cat);
        }

        public async Task removeAsync(int id)
        {
            var cat = await this.repositorio.getAsync(id);
            if(cat!=null)
             await this.repositorio.removeAsync(cat);
        }

        public async Task updateAsync(CategoriaDto categoria)
        {
            var cat = mapper.Map<Categoria>(categoria);
            await this.repositorio.updateAsync(cat);
        }
    }
}
