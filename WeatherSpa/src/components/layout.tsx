import { ReactNode } from 'react';
import { useAuth } from '../contexts/auth-context';
import { Button, UserInfoBanner } from './design-system';
import './layout.css';

interface LayoutProps {
  children: ReactNode;
}

function Layout({ children }: LayoutProps) {
  const { isAuthenticated, user, logout } = useAuth();

  return (
    <div className="layout">
      <a href="#main-content" className="ds-skip-link">
        Skip to main content
      </a>

      <header className="header" role="banner">
        <div className="logo">
          <Button to="/" variant="text">
            Weather SPA
          </Button>
        </div>
        <nav className="nav" aria-label="Main navigation">
          <div className="nav-links">
            <Button to="/" variant="nav-link">
              Home
            </Button>

            {isAuthenticated ? (
              <>
                <Button to="/forecasts" variant="nav-link">
                  Forecasts
                </Button>
                <Button
                  onClick={logout}
                  className="logout-btn"
                  variant="secondary"
                  size="sm"
                  aria-label="Log out of application"
                >
                  Logout
                </Button>
              </>
            ) : (
              <Button to="/login" variant="nav-link">
                Login
              </Button>
            )}
          </div>
        </nav>
      </header>

      {isAuthenticated && user && (
        <UserInfoBanner
          user={user}
          displayedScopes={['read', 'write']}
          aria-label="User information and permissions"
        />
      )}

      <main id="main-content" className="main-content" tabIndex={-1}>
        {children}
      </main>
    </div>
  );
}

export default Layout;
