import apiClient from '../config/api';

export const reviewsService = {
  // Create review for a pet
  createReview: async (reviewData) => {
    const formData = new FormData();
    formData.append('PetId', reviewData.petId);
    formData.append('Rating', reviewData.rating);
    formData.append('Comment', reviewData.comment);

    return apiClient.post('/Reviews', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  // Get reviews received by current user's pets
  getMyReviews: async () => {
    return apiClient.get('/Reviews/my-reviews');
  },

  // Get reviews created by current user
  getReviewsIMade: async () => {
    return apiClient.get('/Reviews/i-made');
  },

  // Get reviews for a specific pet
  getPetReviews: async (petId) => {
    return apiClient.get(`/Reviews/pet/${petId}`);
  },

  // Update review
  updateReview: async (id, reviewData) => {
    const formData = new FormData();
    formData.append('Rating', reviewData.rating);
    formData.append('Comment', reviewData.comment);

    return apiClient.put(`/Reviews/${id}`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  // Delete review
  deleteReview: async (id) => {
    return apiClient.delete(`/Reviews/${id}`);
  },
};
