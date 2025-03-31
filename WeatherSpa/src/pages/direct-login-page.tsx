import { useState } from 'react';
import {
  Button,
  Input,
  Message,
  Spinner,
  LoginLayout,
  CheckboxGroup,
} from '../components/design-system';
import { useAuth } from '../contexts/auth-context';
import { loginWithCredentials } from '../services/auth-service';
import './login-page.css';

// Available scopes for selection
const availableScopes = [
  { id: 'user_info', label: 'User Info', description: 'Access to basic user information' },
  { id: 'profile', label: 'Profile', description: 'Access to detailed profile information' },
  { id: 'email', label: 'Email', description: 'Access to email address' },
  { id: 'read', label: 'Read Data', description: 'Read-only access to data' },
  { id: 'write', label: 'Write Data', description: 'Write access to data' },
];

export default function DirectLoginPage() {
  const { login } = useAuth();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [tenant, setTenant] = useState('tenant1');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedScopes, setSelectedScopes] = useState<{ [key: string]: boolean }>({
    user_info: true, // Enable user_info by default
    read: true, // Enable read by default
  });

  async function handleLoginWithCredentials(e: React.FormEvent) {
    e.preventDefault();

    if (!username || !password || !tenant) {
      setError('Please fill in all fields');
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const scopes = Object.entries(selectedScopes)
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        .filter(([_, selected]) => selected)
        .map(([scope]) => scope);

      const authToken = await loginWithCredentials(username, password, tenant, scopes);

      login(authToken.token, authToken.refreshToken);
    } catch (error: unknown) {
      console.error('Login error:', error);
      const errorObj = error as { response?: { data?: { message?: string } } };
      setError(errorObj.response?.data?.message || 'Login failed. Please check your credentials.');
    } finally {
      setLoading(false);
    }
  }

  function handleScopeChange(scopeId: string, checked: boolean) {
    setSelectedScopes(prev => ({
      ...prev,
      [scopeId]: checked,
    }));
  }

  return (
    <LoginLayout
      title="Direct Login"
      description="Enter your credentials to access the application directly."
      showBackButton
    >
      {error && (
        <Message variant="error" id="login-error">
          {error}
        </Message>
      )}

      <form
        onSubmit={handleLoginWithCredentials}
        aria-describedby={error ? 'login-error' : undefined}
      >
        <Input
          id="username"
          label="Username:"
          type="text"
          value={username}
          onChange={e => setUsername(e.target.value)}
          placeholder="Enter username (e.g., user1)"
          required
          iconLeft={
            <span role="img" aria-hidden="true">
              👤
            </span>
          }
          error={username === '' && error ? 'Username is required' : ''}
        />

        <Input
          id="password"
          label="Password:"
          type="password"
          value={password}
          onChange={e => setPassword(e.target.value)}
          placeholder="Enter password (e.g., password1)"
          required
          iconLeft={
            <span role="img" aria-hidden="true">
              🔒
            </span>
          }
          error={password === '' && error ? 'Password is required' : ''}
        />

        <Input
          id="tenant"
          label="Tenant:"
          type="text"
          value={tenant}
          onChange={e => setTenant(e.target.value)}
          placeholder="Enter tenant (e.g., tenant1)"
          required
          iconLeft={
            <span role="img" aria-hidden="true">
              🏢
            </span>
          }
          error={tenant === '' && error ? 'Tenant is required' : ''}
        />

        <CheckboxGroup
          legend="Select Scopes:"
          helper="Choose which permissions to grant the application"
          items={availableScopes}
          selected={selectedScopes}
          onChange={handleScopeChange}
          columns={2}
          idPrefix="scope-direct"
        />

        <Button type="submit" variant="secondary" fullWidth disabled={loading}>
          {loading ? (
            <>
              <Spinner size="sm" /> Logging in...
            </>
          ) : (
            'Login with Credentials'
          )}
        </Button>
      </form>
    </LoginLayout>
  );
}
