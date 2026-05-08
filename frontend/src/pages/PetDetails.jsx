import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { usePetStore } from '../store/petStore';
import { adoptionService } from '../services/adoptionService';
import { reviewsService } from '../services/reviewsService';
import { useAuthStore } from '../store/authStore';
import toast from 'react-hot-toast';
import { FiArrowLeft, FiHeart } from 'react-icons/fi';

export default function PetDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { currentPet, isLoading, fetchPetById } = usePetStore();
  const { user, isAuthenticated } = useAuthStore();
  const [reviews, setReviews] = useState([]);
  const [showAdoptionForm, setShowAdoptionForm] = useState(false);
  const [newReview, setNewReview] = useState({ rating: 5, comment: '' });

  useEffect(() => {
    if (id) {
      fetchPetById(parseInt(id));
      loadReviews();
    }
  }, [id, fetchPetById]);

  const loadReviews = async () => {
    try {
      const response = await reviewsService.getPetReviews(id);
      setReviews(response.data);
    } catch (error) {
      console.error('Failed to load reviews:', error);
    }
  };

  const handleAdoptionRequest = async () => {
    if (!isAuthenticated) {
      toast.error('Please login to submit adoption request');
      navigate('/login');
      return;
    }

    try {
      await adoptionService.submitAdoptionRequest(parseInt(id));
      toast.success('Adoption request submitted!');
      setShowAdoptionForm(false);
    } catch (error) {
      toast.error('Failed to submit adoption request');
    }
  };

  const handleSubmitReview = async () => {
    if (!isAuthenticated) {
      toast.error('Please login to submit a review');
      return;
    }

    try {
      await reviewsService.createReview({
        petId: parseInt(id),
        rating: newReview.rating,
        comment: newReview.comment,
      });
      toast.success('Review submitted!');
      setNewReview({ rating: 5, comment: '' });
      loadReviews();
    } catch (error) {
      toast.error('Failed to submit review');
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  if (!currentPet) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500 text-lg">Pet not found</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4">
      <div className="max-w-4xl mx-auto">
        {/* Back Button */}
        <button
          onClick={() => navigate('/pets')}
          className="flex items-center gap-2 text-indigo-600 hover:text-indigo-700 mb-6 font-medium"
        >
          <FiArrowLeft size={20} />
          Back to Pets
        </button>

        <div className="bg-white rounded-lg shadow-lg overflow-hidden">
          {/* Images */}
          <div className="grid md:grid-cols-2 gap-6 p-8">
            <div>
              {currentPet.images && currentPet.images.length > 0 ? (
                <img
                  src={currentPet.images[0]}
                  alt={currentPet.name}
                  className="w-full h-96 object-cover rounded-lg"
                />
              ) : (
                <div className="w-full h-96 bg-gray-200 rounded-lg flex items-center justify-center">
                  No Image
                </div>
              )}
            </div>

            {/* Details */}
            <div>
              <h1 className="text-4xl font-bold text-gray-900 mb-4">{currentPet.name}</h1>

              <div className="space-y-4 mb-6">
                <div>
                  <p className="text-gray-600">Breed</p>
                  <p className="text-xl font-semibold text-gray-900">{currentPet.breed}</p>
                </div>
                <div>
                  <p className="text-gray-600">Age</p>
                  <p className="text-xl font-semibold text-gray-900">{currentPet.age} years</p>
                </div>
                <div>
                  <p className="text-gray-600">Gender</p>
                  <p className="text-xl font-semibold text-gray-900">{currentPet.gender}</p>
                </div>
                <div>
                  <p className="text-gray-600">Health Status</p>
                  <p className="text-xl font-semibold text-gray-900">{currentPet.healthStatus}</p>
                </div>
                <div>
                  <p className="text-gray-600">Location</p>
                  <p className="text-xl font-semibold text-gray-900">{currentPet.location}</p>
                </div>
              </div>

              {/* Action Button */}
              {user?.role === 'Adopter' && (
                <button
                  onClick={() => setShowAdoptionForm(true)}
                  className="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-bold py-3 px-6 rounded-lg transition"
                >
                  Request Adoption
                </button>
              )}
            </div>
          </div>

          {/* Description */}
          <div className="px-8 pb-8">
            <h2 className="text-2xl font-bold text-gray-900 mb-4">About</h2>
            <p className="text-gray-700 text-lg leading-relaxed">{currentPet.description}</p>
          </div>

          {/* Reviews Section */}
          <div className="px-8 pb-8 border-t pt-8">
            <h2 className="text-2xl font-bold text-gray-900 mb-6">Reviews</h2>

            {isAuthenticated && user?.role === 'Adopter' && (
              <div className="mb-8 p-6 bg-gray-50 rounded-lg">
                <h3 className="text-lg font-semibold mb-4">Leave a Review</h3>
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium mb-2">Rating</label>
                    <select
                      value={newReview.rating}
                      onChange={(e) =>
                        setNewReview({ ...newReview, rating: parseInt(e.target.value) })
                      }
                      className="w-full border rounded-lg px-4 py-2"
                    >
                      {[1, 2, 3, 4, 5].map((num) => (
                        <option key={num} value={num}>
                          {num} Stars
                        </option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-2">Comment</label>
                    <textarea
                      value={newReview.comment}
                      onChange={(e) =>
                        setNewReview({ ...newReview, comment: e.target.value })
                      }
                      className="w-full border rounded-lg px-4 py-2 h-24"
                      placeholder="Share your experience..."
                    />
                  </div>
                  <button
                    onClick={handleSubmitReview}
                    className="bg-indigo-600 hover:bg-indigo-700 text-white font-semibold py-2 px-4 rounded-lg"
                  >
                    Submit Review
                  </button>
                </div>
              </div>
            )}

            {reviews.length > 0 ? (
              <div className="space-y-4">
                {reviews.map((review) => (
                  <div key={review.id} className="p-4 bg-gray-50 rounded-lg">
                    <div className="flex items-center justify-between mb-2">
                      <p className="font-semibold text-gray-900">{review.reviewerName}</p>
                      <span className="text-yellow-500">{'⭐'.repeat(review.rating)}</span>
                    </div>
                    <p className="text-gray-700">{review.comment}</p>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-500">No reviews yet</p>
            )}
          </div>
        </div>
      </div>

      {/* Adoption Modal */}
      {showAdoptionForm && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full p-6">
            <h2 className="text-2xl font-bold mb-4">Confirm Adoption Request</h2>
            <p className="text-gray-600 mb-6">
              Are you sure you want to request to adopt {currentPet.name}? The owner will review
              your request.
            </p>
            <div className="flex gap-4">
              <button
                onClick={() => setShowAdoptionForm(false)}
                className="flex-1 border border-gray-300 text-gray-700 font-semibold py-2 px-4 rounded-lg hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                onClick={handleAdoptionRequest}
                className="flex-1 bg-indigo-600 hover:bg-indigo-700 text-white font-semibold py-2 px-4 rounded-lg"
              >
                Confirm
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
