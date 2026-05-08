import { useEffect } from 'react';
import { usePetStore } from '../../store/petStore';
import PetCard from './PetCard';

export default function PetList() {
  const { pets, isLoading, error, fetchPets } = usePetStore();

  useEffect(() => {
    fetchPets();
  }, [fetchPets]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
        Error: {error}
      </div>
    );
  }

  return (
    <div className="py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        <h2 className="text-3xl font-bold text-gray-900 mb-8">Available Pets</h2>

        {pets.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-gray-500 text-lg">No pets available at the moment.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {pets.map((pet) => (
              <PetCard key={pet.id} pet={pet} />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
