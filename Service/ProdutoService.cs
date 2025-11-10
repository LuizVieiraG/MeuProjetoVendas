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
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepositorio _repositorio;
        private readonly IMapper _mapper;

        public ProdutoService(IProdutoRepositorio repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<ProdutoDto> addAsync(ProdutoDto produtoDto)
        {
            var produto = _mapper.Map<Produto>(produtoDto);
            var produtoAdicionado = await _repositorio.addAsync(produto);
            
            // Recarregar com categoria incluída
            var produtoCompleto = await _repositorio.getAsync(produtoAdicionado.Id);
            var produtoDtoRetorno = _mapper.Map<ProdutoDto>(produtoCompleto);
            
            // Preencher NomeCategoria
            if (produtoCompleto != null && produtoCompleto.Categoria != null)
            {
                produtoDtoRetorno.NomeCategoria = produtoCompleto.Categoria.Nome;
            }
            
            return produtoDtoRetorno;
        }

        public async Task<ProdutoDto?> getAsync(int id)
        {
            var produto = await _repositorio.getAsync(id);
            if (produto == null) return null;
            
            var produtoDto = _mapper.Map<ProdutoDto>(produto);
            
            // Preencher NomeCategoria
            if (produto.Categoria != null)
            {
                produtoDto.NomeCategoria = produto.Categoria.Nome;
            }
            
            return produtoDto;
        }

        public async Task<IEnumerable<ProdutoDto>> getAllAsync(Func<ProdutoDto, bool> filtro)
        {
            var produtos = await _repositorio.getAllAsync(p => true);
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            
            // Preencher NomeCategoria para cada produto
            foreach (var produtoDto in produtosDto)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == produtoDto.Id);
                if (produto != null && produto.Categoria != null)
                {
                    produtoDto.NomeCategoria = produto.Categoria.Nome;
                }
            }
            
            return produtosDto.Where(filtro);
        }

        public async Task<IEnumerable<ProdutoDto>> getByCategoriaAsync(int idCategoria)
        {
            var produtos = await _repositorio.getByCategoriaAsync(idCategoria);
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            
            // Preencher NomeCategoria
            foreach (var produtoDto in produtosDto)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == produtoDto.Id);
                if (produto != null && produto.Categoria != null)
                {
                    produtoDto.NomeCategoria = produto.Categoria.Nome;
                }
            }
            
            return produtosDto;
        }

        public async Task<IEnumerable<ProdutoDto>> getByMarcaAsync(string marca)
        {
            var produtos = await _repositorio.getByMarcaAsync(marca);
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            
            // Preencher NomeCategoria
            foreach (var produtoDto in produtosDto)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == produtoDto.Id);
                if (produto != null && produto.Categoria != null)
                {
                    produtoDto.NomeCategoria = produto.Categoria.Nome;
                }
            }
            
            return produtosDto;
        }

        public async Task<IEnumerable<ProdutoDto>> getProdutosEmEstoqueAsync()
        {
            var produtos = await _repositorio.getProdutosEmEstoqueAsync();
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            
            // Preencher NomeCategoria
            foreach (var produtoDto in produtosDto)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == produtoDto.Id);
                if (produto != null && produto.Categoria != null)
                {
                    produtoDto.NomeCategoria = produto.Categoria.Nome;
                }
            }
            
            return produtosDto;
        }

        public async Task<bool> atualizarEstoqueAsync(int idProduto, int quantidade)
        {
            return await _repositorio.atualizarEstoqueAsync(idProduto, quantidade);
        }

        public async Task<IEnumerable<ProdutoDto>> buscarProdutosAsync(string termo)
        {
            var produtos = await _repositorio.getAllAsync(p => 
                p.Nome.Contains(termo) || 
                p.Descricao.Contains(termo) || 
                p.Marca.Contains(termo) || 
                p.Modelo.Contains(termo));
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
            
            // Preencher NomeCategoria
            foreach (var produtoDto in produtosDto)
            {
                var produto = produtos.FirstOrDefault(p => p.Id == produtoDto.Id);
                if (produto != null && produto.Categoria != null)
                {
                    produtoDto.NomeCategoria = produto.Categoria.Nome;
                }
            }
            
            return produtosDto;
        }

        public async Task removeAsync(int id)
        {
            var produto = await _repositorio.getAsync(id);
            if (produto != null)
            {
                await _repositorio.removeAsync(produto);
            }
        }

        public async Task<ProdutoDto> updateAsync(ProdutoDto produtoDto)
        {
            var produto = _mapper.Map<Produto>(produtoDto);
            await _repositorio.updateAsync(produto);
            
            // Recarregar com categoria incluída
            var produtoAtualizado = await _repositorio.getAsync(produto.Id);
            var produtoDtoRetorno = _mapper.Map<ProdutoDto>(produtoAtualizado);
            
            // Preencher NomeCategoria
            if (produtoAtualizado != null && produtoAtualizado.Categoria != null)
            {
                produtoDtoRetorno.NomeCategoria = produtoAtualizado.Categoria.Nome;
            }
            
            return produtoDtoRetorno;
        }
    }
}

