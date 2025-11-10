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
        private readonly IMapper _mapper;

        public VendaService(IVendaRepositorio repositorio, IItemVendaRepositorio itemVendaRepositorio, 
            IProdutoRepositorio produtoRepositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _itemVendaRepositorio = itemVendaRepositorio;
            _produtoRepositorio = produtoRepositorio;
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
            
            return _mapper.Map<VendaDto>(vendaAdicionada);
        }

        public async Task<VendaDto?> getAsync(int id)
        {
            var venda = await _repositorio.getAsync(id);
            return venda != null ? _mapper.Map<VendaDto>(venda) : null;
        }

        public async Task<VendaDto?> getWithItensAsync(int id)
        {
            var venda = await _repositorio.getWithItensAsync(id);
            return venda != null ? _mapper.Map<VendaDto>(venda) : null;
        }

        public async Task<IEnumerable<VendaDto>> getAllAsync(Func<VendaDto, bool> filtro)
        {
            var vendas = await _repositorio.getAllAsync(v => true);
            var vendasDto = _mapper.Map<IEnumerable<VendaDto>>(vendas);
            return vendasDto.Where(filtro);
        }

        public async Task<IEnumerable<VendaDto>> getByClienteAsync(int idCliente)
        {
            var vendas = await _repositorio.getAllAsync(v => v.IdCliente == idCliente);
            return _mapper.Map<IEnumerable<VendaDto>>(vendas);
        }

        public async Task<IEnumerable<VendaDto>> getByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _repositorio.getAllAsync(v => 
                v.DataVenda >= dataInicio && v.DataVenda <= dataFim);
            return _mapper.Map<IEnumerable<VendaDto>>(vendas);
        }

        public async Task<VendaDto> finalizarVendaAsync(int idVenda)
        {
            var venda = await _repositorio.getAsync(idVenda);
            if (venda != null)
            {
                venda.Status = "Finalizada";
                var vendaAtualizada = await _repositorio.updateAsync(venda);
                return _mapper.Map<VendaDto>(vendaAtualizada);
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
                return _mapper.Map<VendaDto>(vendaAtualizada);
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
            return _mapper.Map<VendaDto>(vendaAtualizada);
        }
    }
}

