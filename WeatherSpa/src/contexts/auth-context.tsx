import { jwtDecode } from 'jwt-decode';
import { createContext, ReactNode, useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { refreshAccessToken } from '../services/auth-service';
import { Claims, User } from '../types';

interface AuthContextType {
  isAuthenticated: boolean;
  user: User | null;
  login: (token: string, refreshToken?: string, rememberMe?: boolean) => void;
  logout: () => void;
  getToken: () => string | null;
}

const AuthContext = createContext<AuthContextType | null>(null);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const navigate = useNavigate();
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isInitialLogin, setIsInitialLogin] = useState(false);
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    // Only check for refresh token if we haven't just logged in
    if (!isInitialLogin) {
      // Check both storage locations for refresh token
      const refreshToken = localStorage.getItem('refresh_token') || sessionStorage.getItem('refresh_token');
      if (refreshToken) {
        refreshAccessToken().then(authData => {
          if (authData) {
            login(authData.access_token, authData.refresh_token);
          }
        });
      }
    }
  }, [isInitialLogin]);

  const login = (token: string, refreshToken?: string, rememberMe: boolean = false) => {
    try {
      // Decode user info from token
      const decoded = jwtDecode<Claims>(token);
      const user: User = {
        userId: decoded.sub,
        username: decoded.name || decoded.sub,
        tenant: decoded.tenant || '',
        roles: decoded.roles || [],
        scopes: (decoded.scope || []) as string[],
      };

      sessionStorage.setItem('access_token', token);
      if (refreshToken) {
        if (rememberMe) {
          localStorage.setItem('refresh_token', refreshToken);
        } else {
          sessionStorage.setItem('refresh_token', refreshToken);
        }
      }
      setIsAuthenticated(true);
      setIsInitialLogin(true);
      setUser(user);
      navigate('/forecasts');
    } catch (error) {
      console.error('Error decoding token:', error);
      logout();
    }
  };

  const logout = () => {
    sessionStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    sessionStorage.removeItem('refresh_token');
    setIsAuthenticated(false);
    setIsInitialLogin(false);
    setUser(null);
    navigate('/');
  };

  const getToken = () => {
    return sessionStorage.getItem('access_token');
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, logout, getToken }}>
      {children}
    </AuthContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
