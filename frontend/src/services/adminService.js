import apiClient from '../config/api';

export const adminService = {
  // Get dashboard data
  getDashboard: async () => {
    return apiClient.get('/Admin/dashboard');
  },

  // Get pending users
  getPendingUsers: async () => {
    return apiClient.get('/Admin/pending-users');
  },

  // Get all adopters
  getAllAdopters: async () => {
    return apiClient.get('/Admin/all-adopters');
  },

  // Get all owners
  getAllOwners: async () => {
    return apiClient.get('/Admin/all-owners');
  },

  // Approve user
  approveUser: async (userId) => {
    return apiClient.put(`/Admin/users/${userId}/approve`);
  },

  // Reject user
  rejectUser: async (userId) => {
    return apiClient.put(`/Admin/users/${userId}/reject`);
  },

  // Delete user
  deleteUser: async (userId) => {
    return apiClient.delete(`/Admin/delete-user/${userId}`);
  },

  // Get all pets with filters
  getAllPets: async (poststatus, adoptstat) => {
    return apiClient.get('/Admin/pets', {
      params: {
        poststatus,
        adoptstat,
      },
    });
  },

  // Approve pet
  approvePet: async (petId) => {
    return apiClient.put(`/Admin/pets/${petId}/approve`);
  },

  // Reject pet
  rejectPet: async (petId) => {
    return apiClient.put(`/Admin/pets/${petId}/reject`);
  },
};
