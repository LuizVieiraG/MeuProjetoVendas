using Dominio.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Service
{
    public interface IProdutoService
    {
        Task<ProdutoDto> addAsync(ProdutoDto produtoDto);
        Task<ProdutoDto> updateAsync(ProdutoDto produtoDto);
        Task removeAsync(int id);
        Task<ProdutoDto?> getAsync(int id);
        Task<IEnumerable<ProdutoDto>> getAllAsync(Func<ProdutoDto, bool> filtro);
        Task<IEnumerable<ProdutoDto>> getByCategoriaAsync(int idCategoria);
        Task<IEnumerable<ProdutoDto>> getByMarcaAsync(string marca);
        Task<IEnumerable<ProdutoDto>> getProdutosEmEstoqueAsync();
        Task<bool> atualizarEstoqueAsync(int idProduto, int quantidade);
        Task<IEnumerable<ProdutoDto>> buscarProdutosAsync(string termo);
    }
}