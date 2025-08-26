using Dominio.Dtos;
using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Service
{
    public interface ICategoriaService
    {
        Task<CategoriaDto> addAsync(CategoriaDto categoria);
        Task removeAsync(int id);
        Task<CategoriaDto?> getAsync(int id);
        Task<IEnumerable<CategoriaDto>>
            getAllAsync(Expression<Func<Categoria, bool>>
                        expression);
        Task updateAsync(CategoriaDto categoria);
    }
}
