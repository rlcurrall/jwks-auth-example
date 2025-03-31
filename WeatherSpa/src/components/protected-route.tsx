import { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { Spinner } from './design-system';
import { useAuth } from '../contexts/auth-context';

interface ProtectedRouteProps {
  children: ReactNode;
  requiredScopes?: string[];
}

function ProtectedRoute({ children, requiredScopes = [] }: ProtectedRouteProps) {
  const { isAuthenticated, user, loading } = useAuth();

  if (loading) {
    return <Spinner text="Loading authentication..." fullScreen />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requiredScopes.length > 0 && user) {
    const hasRequiredScopes = requiredScopes.every(scope => user.scopes.includes(scope));

    if (!hasRequiredScopes) {
      return <Navigate to="/unauthorized" replace />;
    }
  }

  return <>{children}</>;
}

export default ProtectedRoute;
