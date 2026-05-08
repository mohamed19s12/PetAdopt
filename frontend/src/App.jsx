import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useEffect } from 'react';
import { Toaster } from 'react-hot-toast';
import { useAuthStore } from './store/authStore';

// Components
import Navbar from './components/Common/Navbar';

// Pages
import Home from './pages/Home';
import LoginForm from './components/Auth/LoginForm';
import RegisterForm from './components/Auth/RegisterForm';
import PetList from './components/Pets/PetList';
import PetDetails from './pages/PetDetails';

// Protected Route Component
function ProtectedRoute({ children, requiredRole }) {
  const { isAuthenticated, user } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requiredRole && user?.role !== requiredRole) {
    return <Navigate to="/" replace />;
  }

  return children;
}

function App() {
  const { fetchCurrentUser } = useAuthStore();

  useEffect(() => {
    // Check if user is logged in on app load
    const token = localStorage.getItem('accessToken');
    if (token) {
      fetchCurrentUser();
    }
  }, [fetchCurrentUser]);

  return (
    <Router>
      <Toaster position="top-right" />
      <Navbar />
      <Routes>
        {/* Public Routes */}
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/register" element={<RegisterForm />} />
        <Route path="/pets" element={<PetList />} />
        <Route path="/pets/:id" element={<PetDetails />} />

        {/* Protected Routes - Adopter Only */}
        {/* <Route
          path="/favorites"
          element={
            <ProtectedRoute requiredRole="Adopter">
              <FavoritesList />
            </ProtectedRoute>
          }
        /> */}

        {/* Protected Routes - Owner Only */}
        {/* <Route
          path="/dashboard"
          element={
            <ProtectedRoute requiredRole="Owner">
              <OwnerDashboard />
            </ProtectedRoute>
          }
        /> */}

        {/* 404 Catch All */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Router>
  );
}

export default App;
