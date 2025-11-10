using Dominio.Entidades;
using InfraEstrutura.Data;
using Interface.Repositorio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfraEstrutura.Repositorio
{
    public class ProdutoRepositorio : IProdutoRepositorio
    {
        private readonly EmpresaContexto _contexto;

        public ProdutoRepositorio(EmpresaContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<Produto> addAsync(Produto produto)
        {
            await _contexto.Produtos.AddAsync(produto);
            await _contexto.SaveChangesAsync();
            return produto;
        }

        public async Task<Produto?> getAsync(int id)
        {
            return await _contexto.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Produto>> getAllAsync(Expression<Func<Produto, bool>> expression)
        {
            return await _contexto.Produtos
                .Include(p => p.Categoria)
                .Where(expression)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task updateAsync(Produto produto)
        {
            _contexto.Entry(produto).State = EntityState.Modified;
            await _contexto.SaveChangesAsync();
        }

        public async Task removeAsync(Produto produto)
        {
            _contexto.Produtos.Remove(produto);
            await _contexto.SaveChangesAsync();
        }

        public async Task<IEnumerable<Produto>> getByCategoriaAsync(int idCategoria)
        {
            return await _contexto.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.IdCategoria == idCategoria && p.Ativo)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> getByMarcaAsync(string marca)
        {
            return await _contexto.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Marca.Contains(marca) && p.Ativo)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> getProdutosEmEstoqueAsync()
        {
            return await _contexto.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.QuantidadeEstoque > 0 && p.Ativo)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<bool> atualizarEstoqueAsync(int idProduto, int quantidade)
        {
            var produto = await _contexto.Produtos.FindAsync(idProduto);
            if (produto != null)
            {
                produto.QuantidadeEstoque += quantidade;
                if (produto.QuantidadeEstoque < 0)
                    return false;
                
                await _contexto.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}

