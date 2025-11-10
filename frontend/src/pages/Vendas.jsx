import { useState, useEffect } from 'react';
import { vendaService, clienteService, produtoService } from '../services/api';

const Vendas = () => {
  const [vendas, setVendas] = useState([]);
  const [clientes, setClientes] = useState([]);
  const [produtos, setProdutos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingVenda, setEditingVenda] = useState(null);

  const [formData, setFormData] = useState({
    idCliente: '',
    formaPagamento: 'Dinheiro',
    observacoes: '',
  });

  const [itensVenda, setItensVenda] = useState([]);
  const [itemSelecionado, setItemSelecionado] = useState({
    idProduto: '',
    quantidade: 1,
  });

  useEffect(() => {
    loadVendas();
    loadClientes();
    loadProdutos();
  }, []);

  const loadVendas = async () => {
    try {
      setLoading(true);
      const data = await vendaService.getAll();
      setVendas(data);
    } catch (err) {
      console.error('Erro ao carregar vendas:', err);
    } finally {
      setLoading(false);
    }
  };

  const loadClientes = async () => {
    try {
      const data = await clienteService.getAll();
      setClientes(data.filter(c => c.ativo));
    } catch (err) {
      console.error('Erro ao carregar clientes:', err);
    }
  };

  const loadProdutos = async () => {
    try {
      const data = await produtoService.getAll();
      setProdutos(data.filter(p => p.ativo));
    } catch (err) {
      console.error('Erro ao carregar produtos:', err);
    }
  };

  const resetForm = () => {
    setFormData({
      idCliente: '',
      formaPagamento: 'Dinheiro',
      observacoes: '',
    });
    setItensVenda([]);
    setEditingVenda(null);
  };

  const handleCreateClick = () => {
    resetForm();
    setShowModal(true);
  };

  const handleEditClick = (venda) => {
    setFormData({
      idCliente: venda.idCliente,
      formaPagamento: venda.formaPagamento,
      observacoes: venda.observacoes,
    });
    setItensVenda(venda.itensVenda || []);
    setEditingVenda(venda);
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    resetForm();
  };

  const handleAddItem = () => {
    const produto = produtos.find(p => p.id === parseInt(itemSelecionado.idProduto));
    if (!produto) return;

    const subtotal = produto.preco * itemSelecionado.quantidade;

    setItensVenda([
      ...itensVenda,
      {
        idProduto: produto.id,
        nomeProduto: produto.nome,
        quantidade: itemSelecionado.quantidade,
        precoUnitario: produto.preco,
        desconto: 0,
        subtotal: subtotal,
      },
    ]);

    setItemSelecionado({ idProduto: '', quantidade: 1 });
  };

  const handleRemoveItem = (index) => {
    setItensVenda(itensVenda.filter((_, i) => i !== index));
  };

  const calcularTotal = () => {
    return itensVenda.reduce((total, item) => total + item.subtotal, 0);
  };

  const handleSave = async (e) => {
    e.preventDefault();
    
    if (itensVenda.length === 0) {
      alert('Adicione pelo menos um item à venda');
      return;
    }

    try {
      // Preparar itens da venda no formato correto (PascalCase)
      const itensFormatados = itensVenda.map(item => ({
        IdProduto: item.idProduto,
        Quantidade: item.quantidade,
        PrecoUnitario: item.precoUnitario,
        Desconto: item.desconto || 0,
        Subtotal: item.subtotal
      }));

      const vendaData = {
        IdCliente: parseInt(formData.idCliente),
        // Não enviar DataVenda - deixar o backend definir com DateTime.Now (data/hora local do servidor)
        ValorTotal: calcularTotal(),
        Desconto: 0,
        Status: 'Pendente',
        FormaPagamento: formData.formaPagamento,
        Observacoes: formData.observacoes || '',
        ItensVenda: itensFormatados
      };

      if (editingVenda) {
        await vendaService.update({ ...vendaData, Id: editingVenda.id });
      } else {
        await vendaService.create(vendaData);
      }

      await loadVendas();
      handleCloseModal();
    } catch (err) {
      const errorMessage = err.response?.data?.message || 
                           (err.response?.data?.errors ? JSON.stringify(err.response.data.errors) : null) ||
                           'Erro ao salvar venda';
      alert(errorMessage);
      console.error('Erro completo:', err.response?.data || err);
    }
  };

  const handleDelete = async (id) => {
    if (!confirm('Tem certeza que deseja excluir esta venda?')) {
      return;
    }

    try {
      await vendaService.delete(id);
      await loadVendas();
    } catch (err) {
      alert('Erro ao excluir venda');
      console.error(err);
    }
  };

  const formatarData = (data) => {
    if (!data) return '';
    // Criar data local para evitar problemas de timezone
    const dataObj = new Date(data);
    // Usar métodos locais para garantir a data correta
    const dia = String(dataObj.getDate()).padStart(2, '0');
    const mes = String(dataObj.getMonth() + 1).padStart(2, '0');
    const ano = dataObj.getFullYear();
    return `${dia}/${mes}/${ano}`;
  };

  const formatarMoeda = (valor) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor);
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'Finalizada':
        return 'bg-green-100 text-green-800';
      case 'Confirmada':
        return 'bg-blue-100 text-blue-800';
      case 'Pendente':
        return 'bg-yellow-100 text-yellow-800';
      case 'Cancelada':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const filteredVendas = vendas.filter(venda =>
    venda.nomeCliente.toLowerCase().includes(searchTerm.toLowerCase()) ||
    venda.id.toString().includes(searchTerm)
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
        <h1 className="text-4xl font-bold text-white mb-3 font-gamer">Gerenciamento de Vendas</h1>
        <p className="text-gray-400 text-lg font-gamer">Registre e gerencie suas vendas</p>
      </div>

      <div className="flex justify-between items-center mb-8 gap-4">
        <div className="flex-1 max-w-md">
          <input
            type="text"
            placeholder="Buscar vendas..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 placeholder-gray-500 font-gamer"
          />
        </div>
        <button
          onClick={handleCreateClick}
          className="gamer-button text-white px-6 py-2 rounded-lg font-semibold font-gamer"
        >
          + Nova Venda
        </button>
      </div>

      <div className="gamer-card rounded-lg overflow-hidden">
        <table className="w-full">
          <thead className="bg-gamer-dark-50 border-b border-gray-700">
            <tr>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                ID / Data
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Cliente
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Valor Total
              </th>
              <th className="px-8 py-4 text-left text-sm font-medium text-gray-300 uppercase tracking-wider font-gamer">
                Forma Pagamento
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
            {filteredVendas.map((venda) => (
              <tr key={venda.id} className="hover:bg-gamer-dark-100 transition-colors">
                <td className="px-8 py-5 whitespace-nowrap">
                  <div className="text-base font-medium text-white font-gamer">#{venda.id}</div>
                  <div className="text-sm text-gray-400 mt-1">{formatarData(venda.dataVenda)}</div>
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base text-gray-400">
                  {venda.nomeCliente}
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base font-semibold text-white font-gamer">
                  {formatarMoeda(venda.valorTotal)}
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base text-gray-400">
                  {venda.formaPagamento}
                </td>
                <td className="px-8 py-5 whitespace-nowrap">
                  <span
                    className={`px-3 py-1 inline-flex text-sm leading-5 font-semibold rounded-full font-gamer ${
                      venda.status === 'Finalizada' ? 'bg-green-900/50 text-green-300 border border-green-500' :
                      venda.status === 'Confirmada' ? 'bg-blue-900/50 text-blue-300 border border-blue-500' :
                      venda.status === 'Pendente' ? 'bg-yellow-900/50 text-yellow-300 border border-yellow-500' :
                      venda.status === 'Cancelada' ? 'bg-red-900/50 text-red-300 border border-red-500' :
                      'bg-gray-900/50 text-gray-300 border border-gray-500'
                    }`}
                  >
                    {venda.status}
                  </span>
                </td>
                <td className="px-8 py-5 whitespace-nowrap text-base font-medium">
                  <button
                    onClick={() => handleEditClick(venda)}
                    className="text-gamer-purple-500 hover:text-gamer-purple-400 mr-6 font-gamer transition-colors"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => handleDelete(venda.id)}
                    className="text-red-400 hover:text-red-300 font-gamer transition-colors"
                  >
                    Excluir
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {filteredVendas.length === 0 && (
          <div className="text-center py-16">
            <p className="text-gray-400 text-lg font-gamer">Nenhuma venda encontrada</p>
          </div>
        )}
      </div>

      {showModal && (
        <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50 backdrop-blur-sm p-4">
          <div className="bg-gamer-dark-50 gamer-border rounded-lg p-8 w-full max-w-4xl max-h-[90vh] overflow-y-auto">
            <h2 className="text-3xl font-bold mb-6 text-white font-gamer">
              {editingVenda ? 'Editar Venda' : 'Nova Venda'}
            </h2>

            <form onSubmit={handleSave} className="space-y-6">
              <div className="grid grid-cols-2 gap-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Cliente *
                  </label>
                  <select
                    required
                    value={formData.idCliente}
                    onChange={(e) => setFormData({ ...formData, idCliente: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
                  >
                    <option value="">Selecione...</option>
                    {clientes.map((cliente) => (
                      <option key={cliente.id} value={cliente.id}>
                        {cliente.nome}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Forma de Pagamento *
                  </label>
                  <select
                    required
                    value={formData.formaPagamento}
                    onChange={(e) => setFormData({ ...formData, formaPagamento: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
                  >
                    <option value="Dinheiro">Dinheiro</option>
                    <option value="Cartão">Cartão</option>
                    <option value="PIX">PIX</option>
                    <option value="Boleto">Boleto</option>
                  </select>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Observações
                </label>
                <textarea
                  value={formData.observacoes}
                  onChange={(e) => setFormData({ ...formData, observacoes: e.target.value })}
                  rows="2"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
                />
              </div>

              <div className="border-t pt-4">
                <h3 className="text-lg font-semibold mb-3">Itens da Venda</h3>
                <div className="grid grid-cols-3 gap-4 mb-3">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Produto
                    </label>
                    <select
                      value={itemSelecionado.idProduto}
                      onChange={(e) => setItemSelecionado({ ...itemSelecionado, idProduto: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
                    >
                      <option value="">Selecione...</option>
                      {produtos.map((produto) => (
                        <option key={produto.id} value={produto.id}>
                          {produto.nome} - R$ {produto.preco.toFixed(2)}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Quantidade
                    </label>
                    <input
                      type="number"
                      min="1"
                      value={itemSelecionado.quantidade}
                      onChange={(e) => setItemSelecionado({ ...itemSelecionado, quantidade: parseInt(e.target.value) || 1 })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500"
                    />
                  </div>
                  <div className="flex items-end">
                    <button
                      type="button"
                      onClick={handleAddItem}
                      className="w-full px-4 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600"
                    >
                      Adicionar
                    </button>
                  </div>
                </div>

                <div className="mt-4 space-y-2 max-h-48 overflow-y-auto">
                  {itensVenda.map((item, index) => (
                    <div key={index} className="flex justify-between items-center p-3 bg-gray-50 rounded">
                      <div>
                        <span className="font-medium">{item.nomeProduto}</span>
                        <span className="text-sm text-gray-500 ml-2">
                          {item.quantidade}x {formatarMoeda(item.precoUnitario)}
                        </span>
                      </div>
                      <div className="flex items-center space-x-4">
                        <span className="font-semibold">{formatarMoeda(item.subtotal)}</span>
                        <button
                          type="button"
                          onClick={() => handleRemoveItem(index)}
                          className="text-red-600 hover:text-red-800"
                        >
                          Remover
                        </button>
                      </div>
                    </div>
                  ))}
                </div>

                <div className="mt-4 p-4 bg-gray-100 rounded">
                  <div className="flex justify-between text-lg font-bold">
                    <span>Total:</span>
                    <span>{formatarMoeda(calcularTotal())}</span>
                  </div>
                </div>
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

export default Vendas;


