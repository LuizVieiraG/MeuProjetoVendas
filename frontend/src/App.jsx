import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import PrivateRoute from './utils/PrivateRoute';
import Layout from './components/Layout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Produtos from './pages/Produtos';
import Clientes from './pages/Clientes';
import Vendas from './pages/Vendas';
import Categorias from './pages/Categorias';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Rota pública - Login */}
          <Route path="/login" element={<Login />} />

          {/* Rotas privadas */}
          <Route
            path="/"
            element={
              <PrivateRoute>
                <Layout>
                  <Dashboard />
                </Layout>
              </PrivateRoute>
            }
          />

          <Route
            path="/produtos"
            element={
              <PrivateRoute>
                <Layout>
                  <Produtos />
                </Layout>
              </PrivateRoute>
            }
          />

          <Route
            path="/categorias"
            element={
              <PrivateRoute>
                <Layout>
                  <Categorias />
                </Layout>
              </PrivateRoute>
            }
          />

          <Route
            path="/clientes"
            element={
              <PrivateRoute>
                <Layout>
                  <Clientes />
                </Layout>
              </PrivateRoute>
            }
          />

          <Route
            path="/vendas"
            element={
              <PrivateRoute>
                <Layout>
                  <Vendas />
                </Layout>
              </PrivateRoute>
            }
          />

          {/* Rota padrão - redirecionar para login */}
          <Route
            path="*"
            element={<Navigate to="/login" replace />}
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
