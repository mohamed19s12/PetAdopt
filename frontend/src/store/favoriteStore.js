import { create } from 'zustand';
import { favoritesService } from '../services/favoritesService';

export const useFavoriteStore = create((set) => ({
  favorites: [],
  isLoading: false,
  error: null,

  // Fetch favorites
  fetchFavorites: async () => {
    set({ isLoading: true, error: null });
    try {
      const response = await favoritesService.getFavorites();
      set({ favorites: response.data, isLoading: false });
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to fetch favorites';
      set({ error: errorMessage, isLoading: false });
    }
  },

  // Add to favorites
  addToFavorites: async (petId) => {
    set({ isLoading: true, error: null });
    try {
      const response = await favoritesService.addToFavorites(petId);
      set((state) => ({
        favorites: [...state.favorites, response.data],
        isLoading: false,
      }));
      return response.data;
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to add to favorites';
      set({ error: errorMessage, isLoading: false });
      throw error;
    }
  },

  // Remove from favorites
  removeFromFavorites: async (petId) => {
    set({ isLoading: true, error: null });
    try {
      await favoritesService.removeFromFavorites(petId);
      set((state) => ({
        favorites: state.favorites.filter((fav) => fav.id !== petId),
        isLoading: false,
      }));
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to remove from favorites';
      set({ error: errorMessage, isLoading: false });
      throw error;
    }
  },

  // Check if pet is favorited
  isFavorited: (petId) => {
    const state = useFavoriteStore.getState();
    return state.favorites.some((fav) => fav.id === petId);
  },
}));
