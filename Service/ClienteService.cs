using AutoMapper;
using Dominio.Dtos;
using Dominio.Entidades;
using Interface.Repositorio;
using Interface.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepositorio _repositorio;
        private readonly IVendaRepositorio _vendaRepositorio;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepositorio repositorio, IVendaRepositorio vendaRepositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _vendaRepositorio = vendaRepositorio;
            _mapper = mapper;
        }

        public async Task<ClienteDto> addAsync(ClienteDto clienteDto)
        {
            var cliente = _mapper.Map<Cliente>(clienteDto);
            // Garantir que DataCadastro seja definida se não foi enviada
            if (cliente.DataCadastro == default(DateTime))
            {
                cliente.DataCadastro = DateTime.Now;
            }
            var clienteAdicionado = await _repositorio.addAsync(cliente);
            return _mapper.Map<ClienteDto>(clienteAdicionado);
        }

        public async Task<ClienteDto?> getAsync(int id)
        {
            var cliente = await _repositorio.getAsync(id);
            return cliente != null ? _mapper.Map<ClienteDto>(cliente) : null;
        }

        public async Task<IEnumerable<ClienteDto>> getAllAsync(Func<ClienteDto, bool> filtro)
        {
            var clientes = await _repositorio.getAllAsync(c => true);
            var clientesDto = _mapper.Map<IEnumerable<ClienteDto>>(clientes);
            return clientesDto.Where(filtro);
        }

        public async Task<IEnumerable<ClienteDto>> getByEmailAsync(string email)
        {
            var clientes = await _repositorio.getAllAsync(c => c.Email.Contains(email));
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }

        public async Task<IEnumerable<ClienteDto>> getByNomeAsync(string nome)
        {
            var clientes = await _repositorio.getAllAsync(c => c.Nome.Contains(nome));
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }

        public async Task removeAsync(int id)
        {
            // Verificar se o cliente tem vendas associadas
            var vendas = await _vendaRepositorio.getAllAsync(v => v.IdCliente == id);
            if (vendas.Any())
            {
                throw new InvalidOperationException("Não foi possível excluir o cliente pois ele possui vendas associadas.");
            }
            
            await _repositorio.removeAsync(id);
        }

        public async Task<ClienteDto> updateAsync(ClienteDto clienteDto)
        {
            var cliente = _mapper.Map<Cliente>(clienteDto);
            var clienteAtualizado = await _repositorio.updateAsync(cliente);
            return _mapper.Map<ClienteDto>(clienteAtualizado);
        }
    }
}

