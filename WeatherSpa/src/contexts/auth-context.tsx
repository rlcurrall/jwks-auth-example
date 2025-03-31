import { jwtDecode } from 'jwt-decode';
import { createContext, ReactNode, useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthState, Claims, User } from '../types';

interface AuthContextType extends AuthState {
  login: (token: string, refreshToken: string) => void;
  logout: () => void;
  getToken: () => string | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const navigate = useNavigate();
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    user: null,
    token: null,
    loading: true,
    error: null,
  });

  useEffect(() => {
    // Check for token in sessionStorage on app load
    const token = sessionStorage.getItem('auth_token');
    const refreshToken = sessionStorage.getItem('refresh_token');

    if (token && refreshToken) {
      try {
        // Decode user info from token
        const decoded = jwtDecode<Claims>(token);

        const expiryDate = new Date(decoded.exp * 1000);

        // If token is expired, clear auth state
        if (expiryDate < new Date()) {
          console.log('Token expired, clearing auth state');
          clearAuthData();
          setAuthState({
            isAuthenticated: false,
            user: null,
            token: null,
            loading: false,
            error: 'Token expired. Please login again.',
          });
          return;
        }

        const user: User = {
          userId: decoded.sub,
          username: decoded.name || decoded.sub,
          tenant: decoded.tenant || '',
          roles: decoded.roles || [],
          scopes: decoded.scope || [],
        };

        setAuthState({
          isAuthenticated: true,
          user,
          token,
          loading: false,
          error: null,
        });
      } catch (error) {
        console.error('Error decoding token:', error);
        clearAuthData();
        setAuthState({
          isAuthenticated: false,
          user: null,
          token: null,
          loading: false,
          error: 'Invalid authentication token',
        });
      }
    } else {
      setAuthState(prev => ({ ...prev, loading: false }));
    }
  }, []);

  function clearAuthData() {
    console.log('Clearing auth data');
    sessionStorage.removeItem('auth_token');
    sessionStorage.removeItem('refresh_token');
    sessionStorage.removeItem('auth_state');
  }

  function login(token: string, refreshToken: string) {
    console.log('Auth context login called with token and refresh token');
    try {
      // Store tokens in sessionStorage
      sessionStorage.setItem('auth_token', token);
      sessionStorage.setItem('refresh_token', refreshToken);

      // Decode user info from token
      const decoded = jwtDecode<Claims>(token);
      const user: User = {
        userId: decoded.sub,
        username: decoded.name || decoded.sub,
        tenant: decoded.tenant || '',
        roles: decoded.roles || [],
        scopes: decoded.scope || [],
      };

      setAuthState({
        isAuthenticated: true,
        user,
        token,
        loading: false,
        error: null,
      });

      navigate('/forecasts');
    } catch (error) {
      console.error('Login error:', error);
      setAuthState({
        isAuthenticated: false,
        user: null,
        token: null,
        loading: false,
        error: 'Failed to authenticate',
      });
    }
  }

  function logout() {
    clearAuthData();
    setAuthState({
      isAuthenticated: false,
      user: null,
      token: null,
      loading: false,
      error: null,
    });
    navigate('/');
  }

  function getToken() {
    return authState.token;
  }

  return (
    <AuthContext.Provider
      value={{
        ...authState,
        login,
        logout,
        getToken,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
