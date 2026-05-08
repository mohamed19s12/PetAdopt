import apiClient from '../config/api';

export const petService = {
  // Get all pets
  getAllPets: async () => {
    return apiClient.get('/Pets');
  },

  // Get pet by ID
  getPetById: async (id) => {
    return apiClient.get(`/Pets/${id}`);
  },

  // Create new pet (for owners)
  createPet: async (petData) => {
    const formData = new FormData();
    formData.append('Name', petData.name);
    formData.append('Age', petData.age);
    formData.append('Breed', petData.breed);
    formData.append('Gender', petData.gender); // 'Male' or 'Female'
    formData.append('HealthStatus', petData.healthStatus);
    formData.append('Description', petData.description);
    formData.append('Location', petData.location);

    // Handle multiple images
    if (petData.images && petData.images.length > 0) {
      petData.images.forEach((image) => {
        formData.append('Images', image);
      });
    }

    return apiClient.post('/Pets', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  // Update pet
  updatePet: async (id, petData) => {
    const formData = new FormData();
    formData.append('Name', petData.name);
    formData.append('Age', petData.age);
    formData.append('Breed', petData.breed);
    formData.append('Gender', petData.gender);
    formData.append('HealthStatus', petData.healthStatus);
    formData.append('Description', petData.description);
    formData.append('Location', petData.location);

    // Handle new images
    if (petData.newImages && petData.newImages.length > 0) {
      petData.newImages.forEach((image) => {
        formData.append('NewImages', image);
      });
    }

    return apiClient.put(`/Pets/${id}`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  // Delete pet
  deletePet: async (id) => {
    return apiClient.delete(`/Pets/${id}`);
  },

  // Search pets with filters
  searchPets: async (searchParams) => {
    return apiClient.get('/Pets/search', { params: searchParams });
  },

  // Get owner's pets
  getMyPets: async () => {
    return apiClient.get('/Pets/owner/my-pets');
  },
};
