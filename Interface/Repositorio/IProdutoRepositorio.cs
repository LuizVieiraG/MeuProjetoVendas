using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositorio
{
    public interface IProdutoRepositorio
    {
        Task<Produto> addAsync(Produto produto);
        Task removeAsync(Produto produto);
        Task<Produto> getAsync(int id);
        Task<IEnumerable<Produto>>
            getAllAsync(Expression<Func<Produto, bool>>
                        expression);
        Task updateAsync(Produto produto);
    }
}
