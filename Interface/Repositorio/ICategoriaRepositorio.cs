using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositorio
{
    public interface ICategoriaRepositorio
    {

        Task<Categoria> addAsync(Categoria categoria);
        Task removeAsync(Categoria categoria);
        Task<Categoria?> getAsync(int id);
        Task<IEnumerable<Categoria>> 
            getAllAsync(Expression<Func<Categoria, bool>> 
                        expression);
        Task updateAsync(Categoria categoria);


    }
}
