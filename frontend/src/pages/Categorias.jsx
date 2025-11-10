import { useState, useEffect } from 'react';
import { categoriaService } from '../services/api';

const Categorias = () => {
  const [categorias, setCategorias] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingCategoria, setEditingCategoria] = useState(null);

  const [formData, setFormData] = useState({
    nome: '',
    descricao: '',
    ativo: true,
  });

  useEffect(() => {
    loadCategorias();
  }, []);

  const loadCategorias = async () => {
    try {
      setLoading(true);
      const data = await categoriaService.getAll();
      setCategorias(data);
    } catch (err) {
      console.error('Erro ao carregar categorias:', err);
    } finally {
      setLoading(false);
    }
  };

  const resetForm = () => {
    setFormData({
      nome: '',
      descricao: '',
      ativo: true,
    });
    setEditingCategoria(null);
  };

  const handleCreateClick = () => {
    resetForm();
    setShowModal(true);
  };

  const handleEditClick = (categoria) => {
    setFormData(categoria);
    setEditingCategoria(categoria);
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    resetForm();
  };

  const handleSave = async (e) => {
    e.preventDefault();
    try {
      if (editingCategoria) {
        await categoriaService.update({ ...formData, id: editingCategoria.id });
      } else {
        await categoriaService.create(formData);
      }

      await loadCategorias();
      handleCloseModal();
    } catch (err) {
      alert('Erro ao salvar categoria');
      console.error(err);
    }
  };

  const handleDelete = async (id) => {
    if (!confirm('Tem certeza que deseja excluir esta categoria?')) {
      return;
    }

    try {
      await categoriaService.delete(id);
      await loadCategorias();
    } catch (err) {
      const errorMessage = err.response?.data?.message || 'Erro ao excluir categoria';
      alert(errorMessage);
      console.error(err);
    }
  };

  const filteredCategorias = categorias.filter(categoria =>
    categoria.nome.toLowerCase().includes(searchTerm.toLowerCase()) ||
    categoria.descricao.toLowerCase().includes(searchTerm.toLowerCase())
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
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-white mb-3 font-gamer">Gerenciamento de Categorias</h1>
        <p className="text-gray-400 text-lg font-gamer">Organize seus produtos por categorias</p>
      </div>

      <div className="flex justify-between items-center mb-8 gap-4">
        <div className="flex-1 max-w-md">
          <input
            type="text"
            placeholder="Buscar categorias..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
          />
        </div>
        <button
          onClick={handleCreateClick}
          className="gamer-button text-white px-6 py-2 rounded-lg font-semibold font-gamer"
        >
          + Nova Categoria
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredCategorias.map((categoria) => (
          <div
            key={categoria.id}
            className="gamer-card rounded-lg p-8"
          >
            <div className="flex justify-between items-start mb-6">
              <h3 className="text-2xl font-bold text-white font-gamer">{categoria.nome}</h3>
              <span
                className={`px-3 py-1 text-sm font-semibold rounded-full font-gamer ${
                  categoria.ativo
                    ? 'bg-green-900/50 text-green-300 border border-green-500'
                    : 'bg-red-900/50 text-red-300 border border-red-500'
                }`}
              >
                {categoria.ativo ? 'Ativa' : 'Inativa'}
              </span>
            </div>
            <p className="text-gray-400 mb-6 text-base font-gamer">{categoria.descricao || 'Sem descrição'}</p>
            <div className="flex justify-end space-x-3">
              <button
                onClick={() => handleEditClick(categoria)}
                className="px-4 py-2 bg-gamer-purple-600 text-white rounded-lg hover:bg-gamer-purple-700 text-sm font-medium font-gamer border border-gamer-purple-500 transition-colors"
              >
                Editar
              </button>
              <button
                onClick={() => handleDelete(categoria.id)}
                className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 text-sm font-medium font-gamer border border-red-500 transition-colors"
              >
                Excluir
              </button>
            </div>
          </div>
        ))}
      </div>

      {filteredCategorias.length === 0 && (
        <div className="text-center py-16">
          <p className="text-gray-400 text-lg font-gamer">Nenhuma categoria encontrada</p>
        </div>
      )}

      {showModal && (
        <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50 backdrop-blur-sm p-4">
          <div className="bg-gamer-dark-50 gamer-border rounded-lg p-8 w-full max-w-md">
            <h2 className="text-3xl font-bold mb-6 text-white font-gamer">
              {editingCategoria ? 'Editar Categoria' : 'Nova Categoria'}
            </h2>

            <form onSubmit={handleSave} className="space-y-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Nome *
                </label>
                <input
                  type="text"
                  required
                  value={formData.nome}
                  onChange={(e) => setFormData({ ...formData, nome: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Descrição
                </label>
                <textarea
                  value={formData.descricao}
                  onChange={(e) => setFormData({ ...formData, descricao: e.target.value })}
                  rows="4"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div className="flex items-center">
                <input
                  type="checkbox"
                  id="ativo"
                  checked={formData.ativo}
                  onChange={(e) => setFormData({ ...formData, ativo: e.target.checked })}
                  className="mr-2"
                />
                <label htmlFor="ativo" className="text-sm font-medium text-gray-700">
                  Categoria ativa
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

export default Categorias;


