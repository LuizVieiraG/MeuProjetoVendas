using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositorio
{
    public interface IItemVendaRepositorio
    {
        Task<ItemVenda> addAsync(ItemVenda itemVenda);
        Task<ItemVenda> updateAsync(ItemVenda itemVenda);
        Task removeAsync(int id);
        Task<ItemVenda?> getAsync(int id);
        Task<IEnumerable<ItemVenda>> getAllAsync(Func<ItemVenda, bool> filtro);
        Task<IEnumerable<ItemVenda>> getByVendaIdAsync(int idVenda);
    }
}

