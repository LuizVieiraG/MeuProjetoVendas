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
    public class VendaRepositorio : IVendaRepositorio
    {
        private readonly EmpresaContexto _contexto;

        public VendaRepositorio(EmpresaContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<Venda> addAsync(Venda venda)
        {
            await _contexto.Vendas.AddAsync(venda);
            await _contexto.SaveChangesAsync();
            return venda;
        }

        public async Task<Venda?> getAsync(int id)
        {
            return await _contexto.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.ItensVenda)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Venda?> getWithItensAsync(int id)
        {
            return await _contexto.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.ItensVenda)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Venda>> getAllAsync(Func<Venda, bool> filtro)
        {
            var vendas = await _contexto.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.ItensVenda)
                .ToListAsync();
            return vendas.Where(filtro);
        }

        public async Task removeAsync(int id)
        {
            var venda = await _contexto.Vendas.FindAsync(id);
            if (venda != null)
            {
                _contexto.Vendas.Remove(venda);
                await _contexto.SaveChangesAsync();
            }
        }

        public async Task<Venda> updateAsync(Venda venda)
        {
            _contexto.Vendas.Update(venda);
            await _contexto.SaveChangesAsync();
            return venda;
        }
    }
}
