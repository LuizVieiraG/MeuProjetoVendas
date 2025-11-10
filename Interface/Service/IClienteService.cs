using Dominio.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Service
{
    public interface IClienteService
    {
        Task<ClienteDto> addAsync(ClienteDto clienteDto);
        Task<ClienteDto> updateAsync(ClienteDto clienteDto);
        Task removeAsync(int id);
        Task<ClienteDto?> getAsync(int id);
        Task<IEnumerable<ClienteDto>> getAllAsync(Func<ClienteDto, bool> filtro);
        Task<IEnumerable<ClienteDto>> getByNomeAsync(string nome);
        Task<IEnumerable<ClienteDto>> getByEmailAsync(string email);
    }
}

