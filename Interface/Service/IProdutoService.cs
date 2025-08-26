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
    public interface IProdutoService
    {
        Task<ProdutoDto> addAsync(ProdutoDto produto);
        Task removeAsync(int id);
        Task<ProdutoDto> getAsync(int id);
        Task<IEnumerable<ProdutoDto>>
            getAllAsync(Expression<Func<Produto, bool>>
                        expression);
        Task updateAsync(ProdutoDto produto);
    }
}
