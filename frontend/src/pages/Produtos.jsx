import { useState, useEffect } from 'react';
import { produtoService, categoriaService } from '../services/api';

/**
 * Página de Gerenciamento de Produtos
 * 
 * Esta página implementa o CRUD completo de Produtos:
 * - Listar todos os produtos
 * - Criar novos produtos
 * - Editar produtos existentes
 * - Excluir produtos
 * - Buscar produtos por nome/categoria
 * 
 * Tecnologias utilizadas:
 * - React Hooks (useState, useEffect)
 * - Tailwind CSS para estilização
 * - Axios para requisições HTTP
 * - Modal para formulários (criar/editar)
 */

const Produtos = () => {
  // Estados principais
  const [produtos, setProdutos] = useState([]);
  const [categorias, setCategorias] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  // Estados para controle do modal
  const [showModal, setShowModal] = useState(false);
  const [editingProduto, setEditingProduto] = useState(null);

  // Estados do formulário
  const [formData, setFormData] = useState({
    nome: '',
    descricao: '',
    marca: '',
    modelo: '',
    preco: '',
    quantidadeEstoque: '',
    especificacoes: '',
    imagemUrl: '',
    ativo: true,
    idCategoria: '',
  });

  // Carregar dados iniciais
  useEffect(() => {
    loadProdutos();
    loadCategorias();
  }, []);

  /**
   * Carrega todos os produtos da API
   */
  const loadProdutos = async () => {
    try {
      setLoading(true);
      const data = await produtoService.getAll();
      setProdutos(data);
    } catch (err) {
      setError('Erro ao carregar produtos');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  /**
   * Carrega todas as categorias para o select
   */
  const loadCategorias = async () => {
    try {
      const data = await categoriaService.getAll();
      setCategorias(data.filter(c => c.ativo)); // Apenas categorias ativas
    } catch (err) {
      console.error('Erro ao carregar categorias:', err);
    }
  };

  /**
   * Reseta o formulário para valores padrão
   */
  const resetForm = () => {
    setFormData({
      nome: '',
      descricao: '',
      marca: '',
      modelo: '',
      preco: '',
      quantidadeEstoque: '',
      especificacoes: '',
      imagemUrl: '',
      ativo: true,
      idCategoria: '',
    });
    setEditingProduto(null);
  };

  /**
   * Abre modal para criar novo produto
   */
  const handleCreateClick = () => {
    resetForm();
    setShowModal(true);
  };

  /**
   * Abre modal para editar produto existente
   */
  const handleEditClick = (produto) => {
    setFormData({
      ...produto,
      idCategoria: produto.idCategoria || '',
    });
    setEditingProduto(produto);
    setShowModal(true);
  };

  /**
   * Fecha o modal
   */
  const handleCloseModal = () => {
    setShowModal(false);
    resetForm();
  };

  /**
   * Salva o produto (cria ou edita)
   */
  const handleSave = async (e) => {
    e.preventDefault();
    try {
      // Converter para PascalCase conforme esperado pelo backend
      const produtoData = {
        Nome: formData.nome,
        Descricao: formData.descricao || '',
        Marca: formData.marca || '',
        Modelo: formData.modelo || '',
        Preco: parseFloat(formData.preco),
        QuantidadeEstoque: parseInt(formData.quantidadeEstoque),
        Especificacoes: formData.especificacoes || '',
        ImagemUrl: formData.imagemUrl || '',
        Ativo: formData.ativo,
        IdCategoria: parseInt(formData.idCategoria),
      };

      if (editingProduto) {
        // Atualizar
        await produtoService.update({ ...produtoData, Id: editingProduto.id });
      } else {
        // Criar
        await produtoService.create(produtoData);
      }

      await loadProdutos();
      handleCloseModal();
    } catch (err) {
      const errorMessage = err.response?.data?.message || 
                           (err.response?.data?.errors ? JSON.stringify(err.response.data.errors) : null) ||
                           'Erro ao salvar produto';
      alert(errorMessage);
      console.error('Erro completo:', err.response?.data || err);
    }
  };

  /**
   * Exclui um produto
   */
  const handleDelete = async (id) => {
    if (!confirm('Tem certeza que deseja excluir este produto?')) {
      return;
    }

    try {
      await produtoService.delete(id);
      await loadProdutos();
    } catch (err) {
      alert('Erro ao excluir produto');
      console.error(err);
    }
  };

  /**
   * Filtra produtos por termo de busca
   */
  const filteredProdutos = produtos.filter(produto =>
    produto.nome.toLowerCase().includes(searchTerm.toLowerCase()) ||
    produto.descricao.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-lg text-white font-gamer gamer-glow">Carregando...</div>
      </div>
    );
  }

  return (
    <div>
      {/* Cabeçalho */}
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-white mb-3 font-gamer">Gerenciamento de Produtos</h1>
        <p className="text-gray-400 text-lg font-gamer">Cadastre e gerencie seus produtos</p>
      </div>

      {/* Barra de ferramentas */}
      <div className="flex justify-between items-center mb-8 gap-4">
        <div className="flex-1 max-w-md">
          <input
            type="text"
            placeholder="Buscar produtos..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
          />
        </div>
        <button
          onClick={handleCreateClick}
          className="gamer-button text-white px-6 py-2 rounded-lg font-semibold font-gamer"
        >
          + Novo Produto
        </button>
      </div>

      {/* Tabela de produtos */}
      <div className="gamer-card rounded-lg overflow-hidden">
        <table className="w-full">
          <thead className="bg-gamer-dark-50 border-b border-gray-700">
            <tr>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Nome
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Categoria
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Preço
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Estoque
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Status
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Ações
              </th>
            </tr>
          </thead>
          <tbody className="bg-gamer-dark-50 divide-y divide-gray-700/30">
            {filteredProdutos.map((produto) => (
              <tr key={produto.id} className="hover:bg-gamer-dark-100 transition-colors">
                <td className="px-8 py-5 whitespace-nowrap">
                  <div className="text-sm font-medium text-white font-gamer">{produto.nome}</div>
                  <div className="text-sm text-gray-400">{produto.marca} - {produto.modelo}</div>
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base text-gray-400">
                  {produto.nomeCategoria || 'Sem categoria'}
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base font-semibold text-white font-gamer">
                  R$ {produto.preco.toFixed(2)}
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base text-gray-400">
                  {produto.quantidadeEstoque} unid.
                </td>
                <td className="px-8 py-5 whitespace-nowrap">
                  <span
                    className={`px-3 py-1 inline-flex text-sm leading-5 font-semibold rounded-full font-gamer ${
                      produto.ativo
                        ? 'bg-green-900/50 text-green-300 border border-green-500'
                        : 'bg-red-900/50 text-red-300 border border-red-500'
                    }`}
                  >
                    {produto.ativo ? 'Ativo' : 'Inativo'}
                  </span>
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base font-medium">
                  <button
                    onClick={() => handleEditClick(produto)}
                    className="text-gamer-purple-500 hover:text-gamer-purple-400 mr-6 font-gamer transition-colors"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => handleDelete(produto.id)}
                    className="text-red-400 hover:text-red-300 font-gamer transition-colors"
                  >
                    Excluir
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {filteredProdutos.length === 0 && (
          <div className="text-center py-16">
            <p className="text-gray-400 text-lg font-gamer">Nenhum produto encontrado</p>
          </div>
        )}
      </div>

      {/* Modal de formulário */}
      {showModal && (
        <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50 backdrop-blur-sm p-4">
          <div className="bg-gamer-dark-50 gamer-border rounded-lg p-8 w-full max-w-2xl max-h-[90vh] overflow-y-auto">
            <h2 className="text-3xl font-bold mb-6 text-white font-gamer">
              {editingProduto ? 'Editar Produto' : 'Novo Produto'}
            </h2>

            <form onSubmit={handleSave} className="space-y-6">
              <div className="grid grid-cols-2 gap-6">
                <div>
                  <label className="block text-sm font-medium text-white mb-1 font-gamer">
                    Nome *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.nome}
                    onChange={(e) => setFormData({ ...formData, nome: e.target.value })}
                    className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-white mb-1 font-gamer">
                    Categoria *
                  </label>
                  <select
                    required
                    value={formData.idCategoria}
                    onChange={(e) => setFormData({ ...formData, idCategoria: e.target.value })}
                    className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 font-gamer"
                  >
                    <option value="" className="bg-black text-white">Selecione...</option>
                    {categorias.map((cat) => (
                      <option key={cat.id} value={cat.id} className="bg-black text-white">
                        {cat.nome}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-white mb-1 font-gamer">
                    Marca
                  </label>
                  <input
                    type="text"
                    value={formData.marca}
                    onChange={(e) => setFormData({ ...formData, marca: e.target.value })}
                    className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-white mb-1 font-gamer">
                    Modelo
                  </label>
                  <input
                    type="text"
                    value={formData.modelo}
                    onChange={(e) => setFormData({ ...formData, modelo: e.target.value })}
                    className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-white mb-1 font-gamer">
                    Preço *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    value={formData.preco}
                    onChange={(e) => setFormData({ ...formData, preco: e.target.value })}
                    className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-white mb-1 font-gamer">
                    Quantidade em Estoque *
                  </label>
                  <input
                    type="number"
                    required
                    value={formData.quantidadeEstoque}
                    onChange={(e) => setFormData({ ...formData, quantidadeEstoque: e.target.value })}
                    className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-white mb-1 font-gamer">
                  Descrição
                </label>
                <textarea
                  value={formData.descricao}
                  onChange={(e) => setFormData({ ...formData, descricao: e.target.value })}
                  rows="3"
                  className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-white mb-1 font-gamer">
                  Especificações
                </label>
                <textarea
                  value={formData.especificacoes}
                  onChange={(e) => setFormData({ ...formData, especificacoes: e.target.value })}
                  rows="3"
                  className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-white mb-1 font-gamer">
                  URL da Imagem
                </label>
                <input
                  type="url"
                  value={formData.imagemUrl}
                  onChange={(e) => setFormData({ ...formData, imagemUrl: e.target.value })}
                  className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
                />
              </div>

              <div className="flex items-center">
                <input
                  type="checkbox"
                  id="ativo"
                  checked={formData.ativo}
                  onChange={(e) => setFormData({ ...formData, ativo: e.target.checked })}
                  className="mr-2 w-4 h-4 text-gamer-purple-600 bg-black border-gamer-purple-500 rounded focus:ring-gamer-purple-500"
                />
                <label htmlFor="ativo" className="text-sm font-medium text-white font-gamer">
                  Produto ativo
                </label>
              </div>

              <div className="flex justify-end space-x-4 pt-6 border-t border-gray-700">
                <button
                  type="button"
                  onClick={handleCloseModal}
                  className="px-8 py-3 border-2 border-gray-600 rounded-lg text-white hover:bg-gray-700 font-gamer transition-colors"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="px-8 py-3 gamer-button text-white rounded-lg font-gamer"
                >
                  Salvar
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default Produtos;


