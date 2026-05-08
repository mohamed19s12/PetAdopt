import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import { FiMenu, FiX, FiLogOut } from 'react-icons/fi';
import toast from 'react-hot-toast';

export default function Navbar() {
  const [isOpen, setIsOpen] = useState(false);
  const { user, isAuthenticated, logout } = useAuthStore();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    toast.success('Logged out successfully');
    navigate('/');
    setIsOpen(false);
  };

  return (
    <nav className="bg-white shadow-md sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          {/* Logo */}
          <Link to="/" className="flex items-center">
            <span className="text-2xl font-bold text-indigo-600">🐾 PetAdopt</span>
          </Link>

          {/* Desktop Navigation */}
          <div className="hidden md:flex items-center space-x-8">
            <Link to="/" className="text-gray-700 hover:text-indigo-600 transition">
              Home
            </Link>
            <Link to="/pets" className="text-gray-700 hover:text-indigo-600 transition">
              Browse Pets
            </Link>

            {isAuthenticated ? (
              <>
                {user?.role === 'Owner' && (
                  <Link to="/dashboard" className="text-gray-700 hover:text-indigo-600 transition">
                    My Pets
                  </Link>
                )}
                {user?.role === 'Adopter' && (
                  <Link to="/favorites" className="text-gray-700 hover:text-indigo-600 transition">
                    Favorites
                  </Link>
                )}
                <div className="flex items-center space-x-4">
                  <span className="text-gray-700">{user?.fullName}</span>
                  <button
                    onClick={handleLogout}
                    className="flex items-center space-x-2 bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-lg transition"
                  >
                    <FiLogOut size={18} />
                    <span>Logout</span>
                  </button>
                </div>
              </>
            ) : (
              <>
                <Link
                  to="/login"
                  className="text-gray-700 hover:text-indigo-600 transition font-medium"
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-lg transition font-medium"
                >
                  Sign Up
                </Link>
              </>
            )}
          </div>

          {/* Mobile Menu Button */}
          <div className="md:hidden">
            <button
              onClick={() => setIsOpen(!isOpen)}
              className="text-gray-700 hover:text-indigo-600"
            >
              {isOpen ? <FiX size={24} /> : <FiMenu size={24} />}
            </button>
          </div>
        </div>

        {/* Mobile Navigation */}
        {isOpen && (
          <div className="md:hidden pb-4 space-y-3">
            <Link to="/" className="block text-gray-700 hover:text-indigo-600">
              Home
            </Link>
            <Link to="/pets" className="block text-gray-700 hover:text-indigo-600">
              Browse Pets
            </Link>

            {isAuthenticated ? (
              <>
                {user?.role === 'Owner' && (
                  <Link to="/dashboard" className="block text-gray-700 hover:text-indigo-600">
                    My Pets
                  </Link>
                )}
                {user?.role === 'Adopter' && (
                  <Link to="/favorites" className="block text-gray-700 hover:text-indigo-600">
                    Favorites
                  </Link>
                )}
                <button
                  onClick={handleLogout}
                  className="w-full text-left flex items-center space-x-2 bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-lg"
                >
                  <FiLogOut size={18} />
                  <span>Logout</span>
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="block text-gray-700 hover:text-indigo-600">
                  Login
                </Link>
                <Link to="/register" className="block text-gray-700 hover:text-indigo-600">
                  Sign Up
                </Link>
              </>
            )}
          </div>
        )}
      </div>
    </nav>
  );
}
