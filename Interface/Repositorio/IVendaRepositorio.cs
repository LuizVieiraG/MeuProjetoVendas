using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositorio
{
    public interface IVendaRepositorio
    {
        Task<Venda> addAsync(Venda venda);
        Task<Venda?> getAsync(int id);
        Task<Venda?> getWithItensAsync(int id);
        Task<IEnumerable<Venda>> getAllAsync(Func<Venda, bool> filtro);
        Task removeAsync(int id);
        Task<Venda> updateAsync(Venda venda);
    }
}
