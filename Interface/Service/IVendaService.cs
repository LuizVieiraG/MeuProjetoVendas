using Dominio.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Service
{
    public interface IVendaService
    {
        Task<VendaDto> addAsync(VendaDto vendaDto);
        Task<VendaDto> updateAsync(VendaDto vendaDto);
        Task removeAsync(int id);
        Task<VendaDto?> getAsync(int id);
        Task<IEnumerable<VendaDto>> getAllAsync(Func<VendaDto, bool> filtro);
        Task<VendaDto?> getWithItensAsync(int id);
        Task<IEnumerable<VendaDto>> getByClienteAsync(int idCliente);
        Task<IEnumerable<VendaDto>> getByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<VendaDto> finalizarVendaAsync(int idVenda);
        Task<VendaDto> cancelarVendaAsync(int idVenda);
    }
}

