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
    public class VendaService : IVendaService
    {
        private readonly IVendaRepositorio _repositorio;
        private readonly IItemVendaRepositorio _itemVendaRepositorio;
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly IMapper _mapper;

        public VendaService(IVendaRepositorio repositorio, IItemVendaRepositorio itemVendaRepositorio, 
            IProdutoRepositorio produtoRepositorio, IClienteRepositorio clienteRepositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _itemVendaRepositorio = itemVendaRepositorio;
            _produtoRepositorio = produtoRepositorio;
            _clienteRepositorio = clienteRepositorio;
            _mapper = mapper;
        }

        public async Task<VendaDto> addAsync(VendaDto vendaDto)
        {
            var venda = _mapper.Map<Venda>(vendaDto);
            
            // Sempre usar a data/hora atual do servidor (timezone local) para novas vendas
            // Isso garante que a data seja sempre a data correta do servidor
            venda.DataVenda = DateTime.Now;
            
            // Calcular valor total
            venda.ValorTotal = vendaDto.ItensVenda.Sum(i => i.Subtotal);
            
            var vendaAdicionada = await _repositorio.addAsync(venda);
            
            // Adicionar itens da venda
            foreach (var itemDto in vendaDto.ItensVenda)
            {
                var item = _mapper.Map<ItemVenda>(itemDto);
                item.IdVenda = vendaAdicionada.Id;
                item.Subtotal = item.Quantidade * item.PrecoUnitario - item.Desconto;
                
                await _itemVendaRepositorio.addAsync(item);
                
                // Atualizar estoque
                await _produtoRepositorio.atualizarEstoqueAsync(item.IdProduto, -item.Quantidade);
            }
            
            // Recarregar venda com cliente incluído
            var vendaCompleta = await _repositorio.getAsync(vendaAdicionada.Id);
            var vendaDtoRetorno = _mapper.Map<VendaDto>(vendaCompleta);
            
            // Preencher NomeCliente
            if (vendaCompleta != null && vendaCompleta.Cliente != null)
            {
                vendaDtoRetorno.NomeCliente = vendaCompleta.Cliente.Nome;
            }
            else if (vendaDtoRetorno.IdCliente > 0)
            {
                // Se o cliente não foi carregado, buscar pelo ID
                var cliente = await _clienteRepositorio.getAsync(vendaDtoRetorno.IdCliente);
                if (cliente != null)
                {
                    vendaDtoRetorno.NomeCliente = cliente.Nome;
                }
            }
            
            return vendaDtoRetorno;
        }

        public async Task<VendaDto?> getAsync(int id)
        {
            var venda = await _repositorio.getAsync(id);
            if (venda == null) return null;
            
            var vendaDto = _mapper.Map<VendaDto>(venda);
            
            // Preencher NomeCliente
            if (venda.Cliente != null)
            {
                vendaDto.NomeCliente = venda.Cliente.Nome;
            }
            else if (vendaDto.IdCliente > 0)
            {
                // Se o cliente não foi carregado, buscar pelo ID
                var cliente = await _clienteRepositorio.getAsync(vendaDto.IdCliente);
                if (cliente != null)
                {
                    vendaDto.NomeCliente = cliente.Nome;
                }
            }
            
            return vendaDto;
        }

        public async Task<VendaDto?> getWithItensAsync(int id)
        {
            var venda = await _repositorio.getWithItensAsync(id);
            if (venda == null) return null;
            
            var vendaDto = _mapper.Map<VendaDto>(venda);
            
            // Preencher NomeCliente
            if (venda.Cliente != null)
            {
                vendaDto.NomeCliente = venda.Cliente.Nome;
            }
            
            return vendaDto;
        }

        public async Task<IEnumerable<VendaDto>> getAllAsync(Func<VendaDto, bool> filtro)
        {
            var vendas = await _repositorio.getAllAsync(v => true);
            var vendasDto = _mapper.Map<IEnumerable<VendaDto>>(vendas).ToList();
            
            // Coletar todos os IDs de clientes únicos
            var idsClientes = vendasDto
                .Where(v => v.IdCliente > 0)
                .Select(v => v.IdCliente)
                .Distinct()
                .ToList();
            
            // Buscar todos os clientes necessários de uma vez
            if (idsClientes.Any())
            {
                var todosClientes = await _clienteRepositorio.getAllAsync(c => idsClientes.Contains(c.Id));
                var clientesDict = todosClientes.ToDictionary(c => c.Id, c => c.Nome);
                
                // Preencher NomeCliente para todas as vendas
                foreach (var vendaDto in vendasDto)
                {
                    if (vendaDto.IdCliente > 0)
                    {
                        if (clientesDict.TryGetValue(vendaDto.IdCliente, out var nomeCliente))
                        {
                            vendaDto.NomeCliente = nomeCliente;
                        }
                    }
                }
            }
            
            return vendasDto.Where(filtro);
        }

        public async Task<IEnumerable<VendaDto>> getByClienteAsync(int idCliente)
        {
            var vendas = await _repositorio.getAllAsync(v => v.IdCliente == idCliente);
            var vendasDto = _mapper.Map<IEnumerable<VendaDto>>(vendas);
            
            // Preencher NomeCliente para cada venda
            foreach (var vendaDto in vendasDto)
            {
                var venda = vendas.FirstOrDefault(v => v.Id == vendaDto.Id);
                if (venda != null && venda.Cliente != null)
                {
                    vendaDto.NomeCliente = venda.Cliente.Nome;
                }
            }
            
            return vendasDto;
        }

        public async Task<IEnumerable<VendaDto>> getByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _repositorio.getAllAsync(v => 
                v.DataVenda >= dataInicio && v.DataVenda <= dataFim);
            var vendasDto = _mapper.Map<IEnumerable<VendaDto>>(vendas);
            
            // Preencher NomeCliente para cada venda
            foreach (var vendaDto in vendasDto)
            {
                var venda = vendas.FirstOrDefault(v => v.Id == vendaDto.Id);
                if (venda != null && venda.Cliente != null)
                {
                    vendaDto.NomeCliente = venda.Cliente.Nome;
                }
            }
            
            return vendasDto;
        }

        public async Task<VendaDto> finalizarVendaAsync(int idVenda)
        {
            var venda = await _repositorio.getAsync(idVenda);
            if (venda != null)
            {
                venda.Status = "Finalizada";
                var vendaAtualizada = await _repositorio.updateAsync(venda);
                
                // Recarregar venda com cliente incluído
                var vendaCompleta = await _repositorio.getAsync(vendaAtualizada.Id);
                var vendaDto = _mapper.Map<VendaDto>(vendaCompleta);
                
                // Preencher NomeCliente
                if (vendaCompleta != null && vendaCompleta.Cliente != null)
                {
                    vendaDto.NomeCliente = vendaCompleta.Cliente.Nome;
                }
                
                return vendaDto;
            }
            throw new ArgumentException("Venda não encontrada");
        }

        public async Task<VendaDto> cancelarVendaAsync(int idVenda)
        {
            var venda = await _repositorio.getAsync(idVenda);
            if (venda != null)
            {
                venda.Status = "Cancelada";
                
                // Restaurar estoque dos produtos
                var itens = await _itemVendaRepositorio.getByVendaIdAsync(idVenda);
                foreach (var item in itens)
                {
                    await _produtoRepositorio.atualizarEstoqueAsync(item.IdProduto, item.Quantidade);
                }
                
                var vendaAtualizada = await _repositorio.updateAsync(venda);
                
                // Recarregar venda com cliente incluído
                var vendaCompleta = await _repositorio.getAsync(vendaAtualizada.Id);
                var vendaDto = _mapper.Map<VendaDto>(vendaCompleta);
                
                // Preencher NomeCliente
                if (vendaCompleta != null && vendaCompleta.Cliente != null)
                {
                    vendaDto.NomeCliente = vendaCompleta.Cliente.Nome;
                }
                
                return vendaDto;
            }
            throw new ArgumentException("Venda não encontrada");
        }

        public async Task removeAsync(int id)
        {
            await _repositorio.removeAsync(id);
        }

        public async Task<VendaDto> updateAsync(VendaDto vendaDto)
        {
            var venda = _mapper.Map<Venda>(vendaDto);
            var vendaAtualizada = await _repositorio.updateAsync(venda);
            
            // Recarregar venda com cliente incluído
            var vendaCompleta = await _repositorio.getAsync(vendaAtualizada.Id);
            var vendaDtoRetorno = _mapper.Map<VendaDto>(vendaCompleta);
            
            // Preencher NomeCliente
            if (vendaCompleta != null && vendaCompleta.Cliente != null)
            {
                vendaDtoRetorno.NomeCliente = vendaCompleta.Cliente.Nome;
            }
            
            return vendaDtoRetorno;
        }
    }
}

