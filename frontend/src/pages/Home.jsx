import { Link } from 'react-router-dom';
import { FiArrowRight } from 'react-icons/fi';

export default function Home() {
  return (
    <div>
      {/* Hero Section */}
      <section className="bg-gradient-to-r from-indigo-600 to-purple-600 text-white py-20 px-4">
        <div className="max-w-7xl mx-auto text-center">
          <h1 className="text-5xl md:text-6xl font-bold mb-6">
            🐾 Find Your Perfect Pet Companion
          </h1>
          <p className="text-xl md:text-2xl mb-8 text-indigo-100">
            Connect with loving pet owners and find the pet of your dreams
          </p>
          <div className="flex flex-col md:flex-row gap-4 justify-center">
            <Link
              to="/pets"
              className="bg-white text-indigo-600 hover:bg-indigo-50 px-8 py-3 rounded-lg font-bold inline-flex items-center justify-center gap-2 transition"
            >
              Browse Pets
              <FiArrowRight size={20} />
            </Link>
            <Link
              to="/register"
              className="bg-indigo-700 hover:bg-indigo-800 px-8 py-3 rounded-lg font-bold transition"
            >
              Get Started
            </Link>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-20 px-4 bg-gray-50">
        <div className="max-w-7xl mx-auto">
          <h2 className="text-4xl font-bold text-center mb-12">Why PetAdopt?</h2>
          <div className="grid md:grid-cols-3 gap-8">
            {[
              {
                icon: '🔍',
                title: 'Easy Search',
                description: 'Find pets by breed, age, location, and more',
              },
              {
                icon: '❤️',
                title: 'Save Favorites',
                description: 'Keep track of pets you love',
              },
              {
                icon: '✅',
                title: 'Verified Profiles',
                description: 'All owners and adopters are verified',
              },
            ].map((feature, index) => (
              <div key={index} className="bg-white p-8 rounded-lg shadow-md text-center">
                <div className="text-5xl mb-4">{feature.icon}</div>
                <h3 className="text-xl font-bold mb-2">{feature.title}</h3>
                <p className="text-gray-600">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="bg-indigo-600 text-white py-16 px-4">
        <div className="max-w-4xl mx-auto text-center">
          <h2 className="text-4xl font-bold mb-4">Ready to find a furry friend?</h2>
          <p className="text-lg mb-8 text-indigo-100">
            Join thousands of happy pet owners and adopters on PetAdopt
          </p>
          <Link
            to="/register"
            className="bg-white text-indigo-600 hover:bg-indigo-50 px-8 py-3 rounded-lg font-bold inline-block transition"
          >
            Start Your Journey
          </Link>
        </div>
      </section>
    </div>
  );
}
