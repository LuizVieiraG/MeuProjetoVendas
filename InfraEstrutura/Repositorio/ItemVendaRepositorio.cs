using Dominio.Entidades;
using InfraEstrutura.Data;
using Interface.Repositorio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraEstrutura.Repositorio
{
    public class ItemVendaRepositorio : IItemVendaRepositorio
    {
        private readonly EmpresaContexto _contexto;

        public ItemVendaRepositorio(EmpresaContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<ItemVenda> addAsync(ItemVenda itemVenda)
        {
            await _contexto.ItensVenda.AddAsync(itemVenda);
            await _contexto.SaveChangesAsync();
            return itemVenda;
        }

        public async Task<ItemVenda?> getAsync(int id)
        {
            return await _contexto.ItensVenda
                .Include(i => i.Venda)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<ItemVenda>> getAllAsync(Func<ItemVenda, bool> filtro)
        {
            return await Task.FromResult(_contexto.ItensVenda
                .Include(i => i.Venda)
                .Include(i => i.Produto)
                .Where(filtro)
                .ToList());
        }

        public async Task<IEnumerable<ItemVenda>> getByVendaIdAsync(int idVenda)
        {
            return await _contexto.ItensVenda
                .Include(i => i.Produto)
                .Where(i => i.IdVenda == idVenda)
                .ToListAsync();
        }

        public async Task removeAsync(int id)
        {
            var itemVenda = await _contexto.ItensVenda.FindAsync(id);
            if (itemVenda != null)
            {
                _contexto.ItensVenda.Remove(itemVenda);
                await _contexto.SaveChangesAsync();
            }
        }

        public async Task<ItemVenda> updateAsync(ItemVenda itemVenda)
        {
            _contexto.ItensVenda.Update(itemVenda);
            await _contexto.SaveChangesAsync();
            return itemVenda;
        }
    }
}
