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
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly EmpresaContexto _contexto;

        public ClienteRepositorio(EmpresaContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<Cliente> addAsync(Cliente cliente)
        {
            await _contexto.Clientes.AddAsync(cliente);
            await _contexto.SaveChangesAsync();
            return cliente;
        }

        public async Task<Cliente?> getAsync(int id)
        {
            return await _contexto.Clientes
                .Include(c => c.Vendas)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cliente>> getAllAsync(Func<Cliente, bool> filtro)
        {
            return await Task.FromResult(_contexto.Clientes
                .Include(c => c.Vendas)
                .Where(filtro)
                .ToList());
        }

        public async Task removeAsync(int id)
        {
            var cliente = await _contexto.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _contexto.Clientes.Remove(cliente);
                await _contexto.SaveChangesAsync();
            }
        }

        public async Task<Cliente> updateAsync(Cliente cliente)
        {
            _contexto.Clientes.Update(cliente);
            await _contexto.SaveChangesAsync();
            return cliente;
        }
    }
}
