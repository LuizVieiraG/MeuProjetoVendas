import { useState, useEffect } from 'react';
import { clienteService } from '../services/api';

const Clientes = () => {
  const [clientes, setClientes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingCliente, setEditingCliente] = useState(null);

  const [formData, setFormData] = useState({
    nome: '',
    email: '',
    telefone: '',
    cpf: '',
    dataNascimento: '',
    endereco: '',
    cidade: '',
    estado: '',
    cep: '',
    ativo: true,
  });

  useEffect(() => {
    loadClientes();
  }, []);

  const loadClientes = async () => {
    try {
      setLoading(true);
      const data = await clienteService.getAll();
      setClientes(data);
    } catch (err) {
      console.error('Erro ao carregar clientes:', err);
    } finally {
      setLoading(false);
    }
  };

  const resetForm = () => {
    setFormData({
      nome: '',
      email: '',
      telefone: '',
      cpf: '',
      dataNascimento: '',
      endereco: '',
      cidade: '',
      estado: '',
      cep: '',
      ativo: true,
    });
    setEditingCliente(null);
  };

  const handleCreateClick = () => {
    resetForm();
    setShowModal(true);
  };

  const handleEditClick = (cliente) => {
    setFormData({
      ...cliente,
      dataNascimento: cliente.dataNascimento ? cliente.dataNascimento.split('T')[0] : '',
    });
    setEditingCliente(cliente);
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    resetForm();
  };

  const handleSave = async (e) => {
    e.preventDefault();
    try {
      // Converter para PascalCase conforme esperado pelo backend
      const clienteData = {
        Nome: formData.nome,
        Email: formData.email,
        Telefone: formData.telefone || '',
        Cpf: formData.cpf,
        DataNascimento: formData.dataNascimento ? new Date(formData.dataNascimento) : new Date(2000, 0, 1),
        Endereco: formData.endereco || '',
        Cidade: formData.cidade || '',
        Estado: formData.estado || '',
        Cep: formData.cep || '',
        Ativo: formData.ativo
      };

      if (editingCliente) {
        await clienteService.update({ ...clienteData, Id: editingCliente.id });
      } else {
        await clienteService.create(clienteData);
      }

      await loadClientes();
      handleCloseModal();
    } catch (err) {
      const errorMessage = err.response?.data?.message || 
                           (err.response?.data?.errors ? JSON.stringify(err.response.data.errors) : null) ||
                           'Erro ao salvar cliente';
      alert(errorMessage);
      console.error('Erro completo:', err.response?.data || err);
    }
  };

  const handleDelete = async (id) => {
    if (!confirm('Tem certeza que deseja excluir este cliente?')) {
      return;
    }

    try {
      await clienteService.delete(id);
      await loadClientes();
    } catch (err) {
      alert('Erro ao excluir cliente');
      console.error(err);
    }
  };

  const filteredClientes = clientes.filter(cliente =>
    cliente.nome.toLowerCase().includes(searchTerm.toLowerCase()) ||
    cliente.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
    cliente.cpf.includes(searchTerm)
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
        <h1 className="text-4xl font-bold text-white mb-3 font-gamer">Gerenciamento de Clientes</h1>
        <p className="text-gray-400 text-lg font-gamer">Cadastre e gerencie seus clientes</p>
      </div>

      <div className="flex justify-between items-center mb-8 gap-4">
        <div className="flex-1 max-w-md">
          <input
            type="text"
            placeholder="Buscar clientes..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
          />
        </div>
        <button
          onClick={handleCreateClick}
          className="gamer-button text-white px-6 py-2 rounded-lg font-semibold font-gamer"
        >
          + Novo Cliente
        </button>
      </div>

      <div className="gamer-card rounded-lg overflow-hidden">
        <table className="w-full">
          <thead className="bg-gamer-dark-50 border-b border-gray-700">
            <tr>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Nome
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Email
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Telefone
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                CPF
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
            {filteredClientes.map((cliente) => (
              <tr key={cliente.id} className="hover:bg-gamer-dark-100 transition-colors">
                <td className="px-8 py-5 whitespace-nowrap">
                  <div className="text-base font-medium text-white font-gamer">{cliente.nome}</div>
                  {cliente.cidade && (
                    <div className="text-sm text-gray-400 mt-1">{cliente.cidade} - {cliente.estado}</div>
                  )}
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base text-gray-400">
                  {cliente.email}
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base text-gray-400">
                  {cliente.telefone}
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base text-gray-400">
                  {cliente.cpf}
                </td>
                <td className="px-8 py-5 whitespace-nowrap">
                  <span
                    className={`px-3 py-1 inline-flex text-sm leading-5 font-semibold rounded-full font-gamer ${
                      cliente.ativo
                        ? 'bg-green-900/50 text-green-300 border border-green-500'
                        : 'bg-red-900/50 text-red-300 border border-red-500'
                    }`}
                  >
                    {cliente.ativo ? 'Ativo' : 'Inativo'}
                  </span>
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base font-medium">
                  <button
                    onClick={() => handleEditClick(cliente)}
                    className="text-gamer-purple-500 hover:text-gamer-purple-400 mr-6 font-gamer transition-colors"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => handleDelete(cliente.id)}
                    className="text-red-400 hover:text-red-300 font-gamer transition-colors"
                  >
                    Excluir
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {filteredClientes.length === 0 && (
          <div className="text-center py-16">
            <p className="text-gray-400 text-lg font-gamer">Nenhum cliente encontrado</p>
          </div>
        )}
      </div>

      {showModal && (
        <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50 backdrop-blur-sm p-4">
          <div className="bg-gamer-dark-50 gamer-border rounded-lg p-8 w-full max-w-3xl max-h-[90vh] overflow-y-auto">
            <h2 className="text-3xl font-bold mb-6 text-white font-gamer">
              {editingCliente ? 'Editar Cliente' : 'Novo Cliente'}
            </h2>

            <form onSubmit={handleSave} className="space-y-6">
              <div className="grid grid-cols-2 gap-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Nome *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.nome}
                    onChange={(e) => setFormData({ ...formData, nome: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Email *
                  </label>
                  <input
                    type="email"
                    required
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Telefone
                  </label>
                  <input
                    type="text"
                    value={formData.telefone}
                    onChange={(e) => setFormData({ ...formData, telefone: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    CPF *
                  </label>
                  <input
                    type="text"
                    required
                    value={formData.cpf}
                    onChange={(e) => setFormData({ ...formData, cpf: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Data de Nascimento
                  </label>
                  <input
                    type="date"
                    value={formData.dataNascimento}
                    onChange={(e) => setFormData({ ...formData, dataNascimento: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    CEP
                  </label>
                  <input
                    type="text"
                    value={formData.cep}
                    onChange={(e) => setFormData({ ...formData, cep: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Endereço
                  </label>
                  <input
                    type="text"
                    value={formData.endereco}
                    onChange={(e) => setFormData({ ...formData, endereco: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Cidade
                  </label>
                  <input
                    type="text"
                    value={formData.cidade}
                    onChange={(e) => setFormData({ ...formData, cidade: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Estado
                  </label>
                  <input
                    type="text"
                    value={formData.estado}
                    onChange={(e) => setFormData({ ...formData, estado: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>
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
                  Cliente ativo
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

export default Clientes;


