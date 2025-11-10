using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositorio
{
    public interface IClienteRepositorio
    {
        Task<Cliente> addAsync(Cliente cliente);
        Task<Cliente> updateAsync(Cliente cliente);
        Task removeAsync(int id);
        Task<Cliente?> getAsync(int id);
        Task<IEnumerable<Cliente>> getAllAsync(Func<Cliente, bool> filtro);
    }
}

