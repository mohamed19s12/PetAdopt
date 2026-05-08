import { Link } from 'react-router-dom';
import { FiHeart, FiMapPin, FiCalendar } from 'react-icons/fi';
import { useFavoriteStore } from '../../store/favoriteStore';
import toast from 'react-hot-toast';

export default function PetCard({ pet }) {
  const { addToFavorites, removeFromFavorites, favorites } = useFavoriteStore();
  const isFavorited = favorites.some((fav) => fav.id === pet.id);

  const handleFavoriteToggle = async (e) => {
    e.preventDefault();
    try {
      if (isFavorited) {
        await removeFromFavorites(pet.id);
        toast.success('Removed from favorites');
      } else {
        await addToFavorites(pet.id);
        toast.success('Added to favorites');
      }
    } catch (error) {
      toast.error('Failed to update favorites');
    }
  };

  return (
    <Link to={`/pets/${pet.id}`}>
      <div className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow overflow-hidden h-full flex flex-col">
        {/* Image */}
        <div className="relative h-48 bg-gray-200 overflow-hidden">
          {pet.images && pet.images.length > 0 ? (
            <img
              src={pet.images[0]}
              alt={pet.name}
              className="w-full h-full object-cover hover:scale-105 transition-transform"
            />
          ) : (
            <div className="w-full h-full flex items-center justify-center text-gray-400">
              No Image
            </div>
          )}
          <button
            onClick={handleFavoriteToggle}
            className="absolute top-2 right-2 p-2 bg-white rounded-full shadow-md hover:bg-gray-100 transition-colors"
          >
            <FiHeart
              size={20}
              className={isFavorited ? 'fill-red-500 text-red-500' : 'text-gray-400'}
            />
          </button>
        </div>

        {/* Content */}
        <div className="p-4 flex-grow flex flex-col">
          <h3 className="text-xl font-bold text-gray-900 mb-2">{pet.name}</h3>

          <p className="text-sm text-gray-600 mb-3">{pet.breed}</p>

          {/* Details */}
          <div className="space-y-2 text-sm text-gray-600 mb-3 flex-grow">
            <div className="flex items-center">
              <FiCalendar size={16} className="mr-2" />
              <span>{pet.age} years old</span>
            </div>
            <div className="flex items-center">
              <FiMapPin size={16} className="mr-2" />
              <span>{pet.location}</span>
            </div>
          </div>

          {/* Status Badge */}
          <div className="flex items-center justify-between">
            <span className="inline-block px-3 py-1 bg-blue-100 text-blue-800 text-xs font-semibold rounded-full">
              {pet.gender}
            </span>
            <span className="inline-block px-3 py-1 bg-green-100 text-green-800 text-xs font-semibold rounded-full">
              {pet.adoptionStatus || 'Available'}
            </span>
          </div>
        </div>
      </div>
    </Link>
  );
}
