import apiClient from '../config/api';

export const favoritesService = {
  // Get all favorite pets
  getFavorites: async () => {
    return apiClient.get('/Favorites');
  },

  // Add pet to favorites
  addToFavorites: async (petId) => {
    return apiClient.post(`/Favorites/add-to-favorite/${petId}`);
  },

  // Remove pet from favorites
  removeFromFavorites: async (petId) => {
    return apiClient.delete(`/Favorites/${petId}`);
  },
};
