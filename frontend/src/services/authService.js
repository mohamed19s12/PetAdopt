import apiClient from '../config/api';

export const authService = {
  // Register new user
  register: async (userData) => {
    const formData = new FormData();
    formData.append('FullName', userData.fullName);
    formData.append('Email', userData.email);
    formData.append('Password', userData.password);
    formData.append('Role', userData.role); // 'Owner' or 'Adopter'

    return apiClient.post('/Auth/register', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  // Login user
  login: async (email, password) => {
    const formData = new FormData();
    formData.append('Email', email);
    formData.append('Password', password);

    return apiClient.post('/Auth/login', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  // Get current user info
  getCurrentUser: async () => {
    return apiClient.get('/Auth/me');
  },

  // Logout
  logout: async () => {
    return apiClient.post('/Auth/logout');
  },

  // Refresh token
  refreshToken: async () => {
    return apiClient.post('/Auth/refresh-token');
  },
};
