import { useState } from 'react';
import { Button, Input, LoginLayout, CheckboxGroup, Spinner } from '../components/design-system';
import { initiateAuthRedirect } from '../services/auth-service';
import './login-page.css';

// Available scopes for selection
const availableScopes = [
  { id: 'user_info', label: 'User Info', description: 'Access to basic user information' },
  { id: 'profile', label: 'Profile', description: 'Access to detailed profile information' },
  { id: 'email', label: 'Email', description: 'Access to email address' },
  { id: 'read', label: 'Read Data', description: 'Read-only access to data' },
  { id: 'write', label: 'Write Data', description: 'Write access to data' },
];

export default function RedirectLoginPage() {
  const [tenant, setTenant] = useState('tenant1');
  const [loading, setLoading] = useState(false);
  const [selectedScopes, setSelectedScopes] = useState<{ [key: string]: boolean }>({
    user_info: true,
    read: true,
  });

  function handleScopeChange(scopeId: string, checked: boolean) {
    setSelectedScopes(prev => ({
      ...prev,
      [scopeId]: checked,
    }));
  }

  function handleRedirectLogin() {
    const scopes = Object.entries(selectedScopes)
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
      .filter(([_, selected]) => selected)
      .map(([scope]) => scope);

    setLoading(true);
    initiateAuthRedirect(tenant, scopes);
  }

  return (
    <LoginLayout
      title="Redirect Login"
      description="Login using the redirect flow. You'll be redirected to the Auth Server for authentication."
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

      <Button variant="primary" onClick={handleRedirectLogin} disabled={loading} fullWidth>
        {loading ? (
          <>
            <Spinner size="sm" /> Redirecting...
          </>
        ) : (
          'Login with Redirect'
        )}
      </Button>
    </LoginLayout>
  );
}
