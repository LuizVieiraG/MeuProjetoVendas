import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { adminService } from '../services/api';

const Login = () => {
  const [userName, setUserName] = useState('');
  const [senha, setSenha] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [showCreateAdmin, setShowCreateAdmin] = useState(false);
  const [adminData, setAdminData] = useState({
    nome: '',
    email: '',
    userName: '',
    senha: '',
    confirmarSenha: ''
  });
  const [adminError, setAdminError] = useState('');
  const [adminSuccess, setAdminSuccess] = useState('');
  const [creatingAdmin, setCreatingAdmin] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login(userName, senha);
      navigate('/');
    } catch (err) {
      setError('Credenciais inválidas. Por favor, tente novamente.');
      console.error('Erro no login:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateAdmin = async (e) => {
    e.preventDefault();
    setAdminError('');
    setAdminSuccess('');

    // Validações
    if (adminData.senha !== adminData.confirmarSenha) {
      setAdminError('As senhas não coincidem.');
      return;
    }

    if (adminData.senha.length < 6) {
      setAdminError('A senha deve ter no mínimo 6 caracteres.');
      return;
    }

    setCreatingAdmin(true);

    try {
      const response = await adminService.createAdmin({
        Nome: adminData.nome,
        Email: adminData.email,
        UserName: adminData.userName,
        Senha: adminData.senha
      });

      setAdminSuccess('Administrador criado com sucesso! Fazendo login...');
      
      // Fazer login automaticamente após criar o admin
      setTimeout(async () => {
        try {
          await login(adminData.userName, adminData.senha);
          navigate('/');
        } catch (err) {
          setAdminError('Admin criado, mas erro ao fazer login. Tente fazer login manualmente.');
          setShowCreateAdmin(false);
        }
      }, 1500);
    } catch (err) {
      const errorMessage = err.response?.data?.message || 'Erro ao criar administrador. Tente novamente.';
      setAdminError(errorMessage);
      console.error('Erro ao criar admin:', err);
    } finally {
      setCreatingAdmin(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-black relative overflow-hidden">
      {/* Efeito de fundo animado */}
      <div className="absolute inset-0 bg-gradient-to-br from-gray-900/10 via-black to-gray-900/10"></div>
      
      <div className="relative bg-gamer-dark-50 p-10 rounded-lg gamer-border w-full max-w-md">
        <h1 className="text-4xl font-bold text-center mb-8 text-white font-gamer">
          Sistema de Vendas
        </h1>
        
        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label htmlFor="userName" className="block text-sm font-medium text-white mb-2 font-gamer">
              Usuário
            </label>
            <input
              id="userName"
              type="text"
              value={userName}
              onChange={(e) => setUserName(e.target.value)}
              required
              className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 transition-all placeholder-gray-500 font-gamer"
              placeholder="Digite seu usuário"
            />
          </div>

          <div>
            <label htmlFor="senha" className="block text-sm font-medium text-white mb-2 font-gamer">
              Senha
            </label>
            <input
              id="senha"
              type="password"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              required
              className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 transition-all placeholder-gray-500 font-gamer"
              placeholder="Digite sua senha"
            />
          </div>

          {error && (
            <div className="bg-red-900/50 border-2 border-red-500 text-red-200 px-4 py-3 rounded font-gamer">
              {error}
            </div>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full gamer-button text-white py-2 rounded-lg font-semibold font-gamer focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <div className="mt-8 text-center">
          <button
            type="button"
            onClick={() => setShowCreateAdmin(true)}
            className="w-full bg-gamer-purple-600 hover:bg-gamer-purple-700 text-white py-3 rounded-lg font-semibold font-gamer focus:outline-none focus:ring-2 focus:ring-gamer-purple-500 transition-all border border-gamer-purple-500"
          >
            Criar Administrador
          </button>
        </div>
      </div>

      {/* Modal de Criar Administrador */}
      {showCreateAdmin && (
        <div className="fixed inset-0 bg-black/80 flex items-center justify-center z-50 backdrop-blur-sm p-4">
          <div className="bg-gamer-dark-50 p-10 rounded-lg gamer-border w-full max-w-md max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-8">
              <h2 className="text-3xl font-bold text-white font-gamer">Criar Administrador</h2>
              <button
                onClick={() => {
                  setShowCreateAdmin(false);
                  setAdminData({ nome: '', email: '', userName: '', senha: '', confirmarSenha: '' });
                  setAdminError('');
                  setAdminSuccess('');
                }}
                className="text-gray-400 hover:text-white text-2xl font-gamer transition-colors"
              >
                ×
              </button>
            </div>

            <form onSubmit={handleCreateAdmin} className="space-y-6">
              <div>
                <label htmlFor="nome" className="block text-sm font-medium text-white mb-2 font-gamer">
                  Nome Completo
                </label>
                <input
                  id="nome"
                  type="text"
                  value={adminData.nome}
                  onChange={(e) => setAdminData({ ...adminData, nome: e.target.value })}
                  required
                  className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 transition-all placeholder-gray-500 font-gamer"
                  placeholder="Digite o nome completo"
                />
              </div>

              <div>
                <label htmlFor="email" className="block text-sm font-medium text-white mb-2 font-gamer">
                  Email
                </label>
                <input
                  id="email"
                  type="email"
                  value={adminData.email}
                  onChange={(e) => setAdminData({ ...adminData, email: e.target.value })}
                  required
                  className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 transition-all placeholder-gray-500 font-gamer"
                  placeholder="Digite o email"
                />
              </div>

              <div>
                <label htmlFor="adminUserName" className="block text-sm font-medium text-white mb-2 font-gamer">
                  Nome de Usuário
                </label>
                <input
                  id="adminUserName"
                  type="text"
                  value={adminData.userName}
                  onChange={(e) => setAdminData({ ...adminData, userName: e.target.value })}
                  required
                  className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 transition-all placeholder-gray-500 font-gamer"
                  placeholder="Digite o nome de usuário"
                />
              </div>

              <div>
                <label htmlFor="adminSenha" className="block text-sm font-medium text-white mb-2 font-gamer">
                  Senha
                </label>
                <input
                  id="adminSenha"
                  type="password"
                  value={adminData.senha}
                  onChange={(e) => setAdminData({ ...adminData, senha: e.target.value })}
                  required
                  className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 transition-all placeholder-gray-500 font-gamer"
                  placeholder="Digite a senha (mín. 6 caracteres)"
                />
              </div>

              <div>
                <label htmlFor="confirmarSenha" className="block text-sm font-medium text-white mb-2 font-gamer">
                  Confirmar Senha
                </label>
                <input
                  id="confirmarSenha"
                  type="password"
                  value={adminData.confirmarSenha}
                  onChange={(e) => setAdminData({ ...adminData, confirmarSenha: e.target.value })}
                  required
                  className="w-full px-4 py-2 bg-black border-2 border-gamer-purple-500/50 rounded-lg text-white focus:ring-2 focus:ring-gamer-purple-500 focus:border-gamer-purple-500 transition-all placeholder-gray-500 font-gamer"
                  placeholder="Confirme a senha"
                />
              </div>

              {adminError && (
                <div className="bg-red-900/50 border-2 border-red-500 text-red-200 px-4 py-3 rounded font-gamer">
                  {adminError}
                </div>
              )}

              {adminSuccess && (
                <div className="bg-green-900/50 border-2 border-green-500 text-green-200 px-4 py-3 rounded font-gamer">
                  {adminSuccess}
                </div>
              )}

              <div className="flex gap-4 pt-4 border-t border-gray-700">
                <button
                  type="button"
                  onClick={() => {
                    setShowCreateAdmin(false);
                    setAdminData({ nome: '', email: '', userName: '', senha: '', confirmarSenha: '' });
                    setAdminError('');
                    setAdminSuccess('');
                  }}
                  className="flex-1 bg-gray-700 text-white py-3 rounded-lg font-semibold font-gamer hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-500 transition border-2 border-gray-600"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={creatingAdmin}
                  className="flex-1 gamer-button text-white py-3 rounded-lg font-semibold font-gamer focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {creatingAdmin ? 'Criando...' : 'Criar Admin'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default Login;

