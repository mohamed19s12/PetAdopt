import { create } from 'zustand';
import { petService } from '../services/petService';

export const usePetStore = create((set) => ({
  pets: [],
  myPets: [],
  currentPet: null,
  isLoading: false,
  error: null,

  // Fetch all pets
  fetchPets: async () => {
    set({ isLoading: true, error: null });
    try {
      const response = await petService.getAllPets();
      set({ pets: response.data, isLoading: false });
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to fetch pets';
      set({ error: errorMessage, isLoading: false });
    }
  },

  // Fetch pet by ID
  fetchPetById: async (id) => {
    set({ isLoading: true, error: null });
    try {
      const response = await petService.getPetById(id);
      set({ currentPet: response.data, isLoading: false });
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to fetch pet';
      set({ error: errorMessage, isLoading: false });
    }
  },

  // Fetch user's pets
  fetchMyPets: async () => {
    set({ isLoading: true, error: null });
    try {
      const response = await petService.getMyPets();
      set({ myPets: response.data, isLoading: false });
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to fetch your pets';
      set({ error: errorMessage, isLoading: false });
    }
  },

  // Create pet
  createPet: async (petData) => {
    set({ isLoading: true, error: null });
    try {
      const response = await petService.createPet(petData);
      set((state) => ({
        myPets: [...state.myPets, response.data],
        isLoading: false,
      }));
      return response.data;
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to create pet';
      set({ error: errorMessage, isLoading: false });
      throw error;
    }
  },

  // Update pet
  updatePet: async (id, petData) => {
    set({ isLoading: true, error: null });
    try {
      const response = await petService.updatePet(id, petData);
      set((state) => ({
        myPets: state.myPets.map((pet) => (pet.id === id ? response.data : pet)),
        currentPet: response.data,
        isLoading: false,
      }));
      return response.data;
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to update pet';
      set({ error: errorMessage, isLoading: false });
      throw error;
    }
  },

  // Delete pet
  deletePet: async (id) => {
    set({ isLoading: true, error: null });
    try {
      await petService.deletePet(id);
      set((state) => ({
        myPets: state.myPets.filter((pet) => pet.id !== id),
        isLoading: false,
      }));
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Failed to delete pet';
      set({ error: errorMessage, isLoading: false });
      throw error;
    }
  },

  // Search pets
  searchPets: async (searchParams) => {
    set({ isLoading: true, error: null });
    try {
      const response = await petService.searchPets(searchParams);
      set({ pets: response.data, isLoading: false });
    } catch (error) {
      const errorMessage = error.response?.data?.message || 'Search failed';
      set({ error: errorMessage, isLoading: false });
    }
  },
}));
