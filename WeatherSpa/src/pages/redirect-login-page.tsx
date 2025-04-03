import { useState } from 'react';
import {
  Button,
  Checkbox,
  CheckboxGroup,
  Input,
  LoginLayout,
  Spinner,
} from '../components/design-system';
import { initiateAuthRedirect } from '../services/auth-service';
import './login-page.css';

// Available scopes for selection
const availableScopes = [
  { id: 'openid', label: 'OpenID Connect', description: 'Access to OpenID Connect functionality' },
  { id: 'profile', label: 'Profile', description: 'Access to detailed profile information' },
  { id: 'email', label: 'Email', description: 'Access to email address' },
  { id: 'weather.read', label: 'Weather Data', description: 'Read access to weather data' },
];

export default function RedirectLoginPage() {
  const [tenant, setTenant] = useState('tenant1');
  const [loading, setLoading] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const [selectedScopes, setSelectedScopes] = useState<{ [key: string]: boolean }>({
    openid: true,
    profile: true,
    email: true,
    'weather.read': true,
  });

  function handleScopeChange(scopeId: string, checked: boolean) {
    setSelectedScopes(prev => ({
      ...prev,
      [scopeId]: checked,
    }));
  }

  async function handleRedirectLogin() {
    const scopes = Object.entries(selectedScopes)
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      .filter(([_, selected]) => selected)
      .map(([scope]) => scope);

    // Store remember me preference in session storage
    sessionStorage.setItem('remember_me', rememberMe.toString());

    setLoading(true);
    await initiateAuthRedirect(tenant, scopes);
  }

  return (
    <LoginLayout
      title="Redirect Login"
      description="Login using the OAuth 2.0 Authorization Code flow with PKCE. You'll be redirected to the Auth Server for authentication."
      showBackButton
    >
      <Input
        id="tenant-redirect"
        label="Tenant:"
        value={tenant}
        onChange={e => setTenant(e.target.value)}
        placeholder="Enter tenant (e.g., tenant1)"
        iconLeft={
          <span role="img" aria-hidden="true">
            🏢
          </span>
        }
        fullWidth
      />

      <CheckboxGroup
        legend="Select Scopes:"
        helper="Choose which permissions to grant the application"
        items={availableScopes}
        selected={selectedScopes}
        onChange={handleScopeChange}
        columns={2}
        idPrefix="scope-redirect"
      />

      <Checkbox
        id="remember-me"
        label="Remember me"
        checked={rememberMe}
        onChange={checked => setRememberMe(checked)}
        helper="Stay logged in across browser sessions"
      />

      <Button variant="primary" onClick={handleRedirectLogin} disabled={loading} fullWidth>
        {loading ? (
          <>
            <Spinner size="sm" /> Redirecting...
          </>
        ) : (
          'Login with OAuth'
        )}
      </Button>
    </LoginLayout>
  );
}
