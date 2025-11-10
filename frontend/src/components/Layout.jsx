import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Layout = ({ children }) => {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const isActive = (path) => location.pathname === path;

  return (
    <div className="min-h-screen bg-black">
      {/* Navbar */}
      <nav className="bg-gamer-dark-50 border-b border-gray-800">
        <div className="max-w-7xl mx-auto px-6 sm:px-8 lg:px-10">
          <div className="flex justify-between h-20">
            <div className="flex items-center">
              <div className="flex-shrink-0 flex items-center">
                <h1 className="text-3xl font-bold text-gamer-purple-400 font-gamer" style={{ 
                  textShadow: '0 0 10px rgba(168, 85, 247, 0.5), 0 0 20px rgba(168, 85, 247, 0.3)',
                  color: '#a855f7'
                }}>
                  VendasPro
                </h1>
              </div>
              <div className="hidden sm:ml-16 sm:flex sm:gap-8">
                <Link
                  to="/"
                  className={`inline-flex items-center px-6 py-3 border-b-2 text-lg font-medium font-gamer transition-all ${
                    isActive('/')
                      ? 'border-gamer-purple-500 text-white'
                      : 'border-transparent text-gray-400 hover:text-white hover:border-gray-600'
                  }`}
                >
                  Dashboard
                </Link>
                <Link
                  to="/produtos"
                  className={`inline-flex items-center px-6 py-3 border-b-2 text-lg font-medium font-gamer transition-all ${
                    isActive('/produtos')
                      ? 'border-gamer-purple-500 text-white'
                      : 'border-transparent text-gray-400 hover:text-white hover:border-gray-600'
                  }`}
                >
                  Produtos
                </Link>
                <Link
                  to="/categorias"
                  className={`inline-flex items-center px-6 py-3 border-b-2 text-lg font-medium font-gamer transition-all ${
                    isActive('/categorias')
                      ? 'border-gamer-purple-500 text-white'
                      : 'border-transparent text-gray-400 hover:text-white hover:border-gray-600'
                  }`}
                >
                  Categorias
                </Link>
                <Link
                  to="/clientes"
                  className={`inline-flex items-center px-6 py-3 border-b-2 text-lg font-medium font-gamer transition-all ${
                    isActive('/clientes')
                      ? 'border-gamer-purple-500 text-white'
                      : 'border-transparent text-gray-400 hover:text-white hover:border-gray-600'
                  }`}
                >
                  Clientes
                </Link>
                <Link
                  to="/vendas"
                  className={`inline-flex items-center px-6 py-3 border-b-2 text-lg font-medium font-gamer transition-all ${
                    isActive('/vendas')
                      ? 'border-gamer-purple-500 text-white'
                      : 'border-transparent text-gray-400 hover:text-white hover:border-gray-600'
                  }`}
                >
                  Vendas
                </Link>
                {user && isAdmin() && (
                  <Link
                    to="/admin"
                    className={`inline-flex items-center px-6 py-3 border-b-2 text-lg font-medium font-gamer transition-all ${
                      isActive('/admin')
                        ? 'border-gamer-purple-500 text-white'
                        : 'border-transparent text-gray-400 hover:text-white hover:border-gray-600'
                    }`}
                  >
                    Admin
                  </Link>
                )}
              </div>
            </div>
            <div className="flex items-center">
              <div className="mr-6 text-base text-white">
                {user ? (
                  <>
                    <span className="font-semibold font-gamer">{user.userName}</span>
                    {user.role === 'Admin' && (
                      <span className="ml-3 px-3 py-1.5 bg-gamer-purple-600 text-white text-sm rounded-full border border-gamer-purple-500">
                        Admin
                      </span>
                    )}
                  </>
                ) : (
                  <span className="text-gray-400">Modo Demonstração</span>
                )}
              </div>
              {user && (
                <button
                  onClick={handleLogout}
                  className="bg-red-600 hover:bg-red-700 text-white px-6 py-2.5 rounded-lg text-base font-medium font-gamer transition-all"
                >
                  Sair
                </button>
              )}
            </div>
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto py-8 px-8 lg:px-12">
        <div className="w-full">
          {children}
        </div>
      </main>
    </div>
  );
};

export default Layout;

