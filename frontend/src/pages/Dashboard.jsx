import { useEffect, useState } from 'react';
import { produtoService, clienteService, vendaService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const Dashboard = () => {
  const { user } = useAuth();
  const [stats, setStats] = useState({
    totalProdutos: 0,
    totalClientes: 0,
    totalVendas: 0,
    vendasMes: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadStats = async () => {
      try {
        const [produtos, clientes, vendas] = await Promise.all([
          produtoService.getAll(),
          clienteService.getAll(),
          vendaService.getAll(),
        ]);

        const agora = new Date();
        const anoAtual = agora.getFullYear();
        const mesAtual = agora.getMonth(); // 0-11
        
        // Criar datas de início e fim do mês usando apenas ano/mês/dia (sem hora)
        const inicioMes = new Date(anoAtual, mesAtual, 1);
        const fimMes = new Date(anoAtual, mesAtual + 1, 0); // Último dia do mês
        
        const vendasMes = vendas.filter(v => {
          if (!v.dataVenda) return false;
          
          // Converter a data da venda para objeto Date
          const dataVenda = new Date(v.dataVenda);
          
          // Extrair apenas ano, mês e dia (ignorar hora) para comparação
          const anoVenda = dataVenda.getFullYear();
          const mesVenda = dataVenda.getMonth();
          const diaVenda = dataVenda.getDate();
          
          // Criar data normalizada da venda (sem hora)
          const dataVendaNormalizada = new Date(anoVenda, mesVenda, diaVenda);
          
          // Comparar se a data da venda está dentro do mês atual
          return dataVendaNormalizada >= inicioMes && dataVendaNormalizada <= fimMes;
        });

        setStats({
          totalProdutos: produtos.length,
          totalClientes: clientes.length,
          totalVendas: vendas.length,
          vendasMes: vendasMes.length,
        });
      } catch (error) {
        console.error('Erro ao carregar estatísticas:', error);
      } finally {
        setLoading(false);
      }
    };

    loadStats();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-lg text-white font-gamer gamer-glow">Carregando...</div>
      </div>
    );
  }

  const cards = [
    {
      title: 'Total de Produtos',
      value: stats.totalProdutos,
      icon: '📦',
      color: 'bg-gamer-purple-600',
      borderColor: 'border-gamer-purple-500',
    },
    {
      title: 'Total de Clientes',
      value: stats.totalClientes,
      icon: '👥',
      color: 'bg-gamer-purple-600',
      borderColor: 'border-gamer-purple-500',
    },
    {
      title: 'Total de Vendas',
      value: stats.totalVendas,
      icon: '💰',
      color: 'bg-gamer-purple-600',
      borderColor: 'border-gamer-purple-500',
    },
    {
      title: 'Vendas do Mês',
      value: stats.vendasMes,
      icon: '📊',
      color: 'bg-gamer-purple-600',
      borderColor: 'border-gamer-purple-500',
    },
  ];

  return (
    <div>
      <div className="mb-16">
        <h1 className="text-4xl font-bold text-white font-gamer mb-4">
          Bem-vindo{user?.userName ? `, ${user.userName}` : ''}!
        </h1>
        <p className="text-gray-400 text-lg font-gamer">Visão geral do seu sistema de vendas</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-10 mb-20">
        {cards.map((card, index) => (
          <div
            key={index}
            className="gamer-card rounded-lg p-10 hover:scale-105 transition-transform"
          >
            <div className="flex items-center justify-between mb-8">
              <div className={`${card.color} p-5 rounded-lg border border-gamer-purple-500/30`}>
                <span className="text-4xl">{card.icon}</span>
              </div>
            </div>
            <h3 className="text-gray-400 text-base font-medium mb-4 font-gamer">{card.title}</h3>
            <p className="text-4xl font-bold text-white font-gamer">{card.value}</p>
          </div>
        ))}
      </div>

      <div className="mt-20 gamer-card rounded-lg p-10">
        <h2 className="text-3xl font-bold text-white mb-8 font-gamer">Informações Rápidas</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div className="border-l-2 border-gray-700 pl-8 py-4">
            <h3 className="font-semibold text-white text-lg mb-3 font-gamer">Produtos em Estoque</h3>
            <p className="text-gray-400 text-base font-gamer">
              Gerencie seus produtos e controle o estoque
            </p>
          </div>
          <div className="border-l-2 border-gray-700 pl-8 py-4">
            <h3 className="font-semibold text-white text-lg mb-3 font-gamer">Cadastro de Clientes</h3>
            <p className="text-gray-400 text-base font-gamer">
              Mantenha seus clientes organizados
            </p>
          </div>
          <div className="border-l-2 border-gray-700 pl-8 py-4">
            <h3 className="font-semibold text-white text-lg mb-3 font-gamer">Gestão de Vendas</h3>
            <p className="text-gray-400 text-base font-gamer">
              Registre e gerencie suas vendas
            </p>
          </div>
          <div className="border-l-2 border-gray-700 pl-8 py-4">
            <h3 className="font-semibold text-white text-lg mb-3 font-gamer">Relatórios</h3>
            <p className="text-gray-400 text-base font-gamer">
              Visualize relatórios e estatísticas
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;

