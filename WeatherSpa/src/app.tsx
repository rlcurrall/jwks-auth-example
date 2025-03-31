import { useEffect } from 'react';
import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import './app.css';
import Layout from './components/layout';
import ProtectedRoute from './components/protected-route';
import { AuthProvider, useAuth } from './contexts/auth-context';
import AuthCallbackPage from './pages/auth-callback-page';
import DirectLoginPage from './pages/direct-login-page';
import ForecastsPage from './pages/forecasts-page';
import HomePage from './pages/home-page';
import LoginPage from './pages/login-page';
import RedirectLoginPage from './pages/redirect-login-page';
import UnauthorizedPage from './pages/unauthorized-page';
import { setupAuthInterceptor } from './services/api';

function App() {
  return (
    <Router>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </Router>
  );
}

function AppContent() {
  const { getToken } = useAuth();

  // Setup axios interceptor to add token to requests
  useEffect(() => {
    setupAuthInterceptor(getToken);
  }, [getToken]);

  return (
    <Layout>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/login/direct" element={<DirectLoginPage />} />
        <Route path="/login/redirect" element={<RedirectLoginPage />} />
        <Route path="/callback" element={<AuthCallbackPage />} />
        <Route path="/unauthorized" element={<UnauthorizedPage />} />

        {/* Protected routes */}
        <Route
          path="/forecasts"
          element={
            <ProtectedRoute>
              <ForecastsPage />
            </ProtectedRoute>
          }
        />
      </Routes>
    </Layout>
  );
}

export default App;
