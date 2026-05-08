import apiClient from '../config/api';

export const adoptionService = {
  // Submit adoption request for a pet
  submitAdoptionRequest: async (petId) => {
    return apiClient.post(`/Adoption/${petId}/requests`);
  },

  // Get adoption requests for current adopter
  getAdopterRequests: async (status) => {
    return apiClient.get('/Adoption/adopter-requests', {
      params: { status },
    });
  },

  // Get adoption requests for current owner's pets
  getOwnerRequests: async () => {
    return apiClient.get('/Adoption/owner-requests');
  },

  // Get all adoption requests (admin)
  getAllRequests: async () => {
    return apiClient.get('/Adoption/all-requests');
  },

  // Approve adoption request
  approveRequest: async (requestId) => {
    return apiClient.put(`/Adoption/requests/${requestId}/approve`);
  },

  // Reject adoption request
  rejectRequest: async (requestId) => {
    return apiClient.put(`/Adoption/requests/${requestId}/reject`);
  },
};
