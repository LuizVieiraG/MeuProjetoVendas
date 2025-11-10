import axios from 'axios';

// Configuração base da API
const api = axios.create({
  baseURL: 'http://localhost:5030/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptador para adicionar o token de autenticação
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptador para lidar com respostas e erros
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expirado ou inválido
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;

// Serviços específicos
export const authService = {
  login: async (userName, senha) => {
    const data = { 
      UserName: userName,
      Senha: senha
    };
    const response = await api.post('/seguranca/login', data);
    return response.data;
  },
  
  refreshToken: async (refreshToken) => {
    const response = await api.post('/seguranca/refresh-token', { refreshToken });
    return response.data;
  },
  
  revokeToken: async (refreshToken) => {
    const response = await api.post('/seguranca/revoke-token', { refreshToken });
    return response.data;
  },
};

export const produtoService = {
  getAll: async () => {
    const response = await api.get('/produto');
    return response.data;
  },
  
  getById: async (id) => {
    const response = await api.get(`/produto/${id}`);
    return response.data;
  },
  
  create: async (produto) => {
    const response = await api.post('/produto', produto);
    return response.data;
  },
  
  update: async (produto) => {
    const response = await api.put('/produto', produto);
    return response.data;
  },
  
  delete: async (id) => {
    await api.delete(`/produto/${id}`);
  },
};

export const clienteService = {
  getAll: async () => {
    const response = await api.get('/cliente');
    return response.data;
  },
  
  getById: async (id) => {
    const response = await api.get(`/cliente/${id}`);
    return response.data;
  },
  
  create: async (cliente) => {
    const response = await api.post('/cliente', cliente);
    return response.data;
  },
  
  update: async (cliente) => {
    const response = await api.put('/cliente', cliente);
    return response.data;
  },
  
  delete: async (id) => {
    await api.delete(`/cliente/${id}`);
  },
};

export const vendaService = {
  getAll: async () => {
    const response = await api.get('/venda');
    return response.data;
  },
  
  getById: async (id) => {
    const response = await api.get(`/venda/${id}`);
    return response.data;
  },
  
  getByCliente: async (idCliente) => {
    const response = await api.get(`/venda/cliente/${idCliente}`);
    return response.data;
  },
  
  getByPeriodo: async (dataInicio, dataFim) => {
    const response = await api.get('/venda/periodo', {
      params: { dataInicio, dataFim }
    });
    return response.data;
  },
  
  create: async (venda) => {
    const response = await api.post('/venda', venda);
    return response.data;
  },
  
  update: async (venda) => {
    const response = await api.put('/venda', venda);
    return response.data;
  },
  
  delete: async (id) => {
    await api.delete(`/venda/${id}`);
  },
  
  finalizar: async (id) => {
    const response = await api.put(`/venda/${id}/finalizar`);
    return response.data;
  },
  
  cancelar: async (id) => {
    const response = await api.put(`/venda/${id}/cancelar`);
    return response.data;
  },
};

export const categoriaService = {
  getAll: async () => {
    const response = await api.get('/categoria');
    return response.data;
  },
  
  getById: async (id) => {
    const response = await api.get(`/categoria/${id}`);
    return response.data;
  },
  
  create: async (categoria) => {
    const response = await api.post('/categoria', categoria);
    return response.data;
  },
  
  update: async (categoria) => {
    const response = await api.put('/categoria', categoria);
    return response.data;
  },
  
  delete: async (id) => {
    await api.delete(`/categoria/${id}`);
  },
};

export const adminService = {
  createAdmin: async (adminData) => {
    const response = await api.post('/admin/create-admin', adminData);
    return response.data;
  },
};

