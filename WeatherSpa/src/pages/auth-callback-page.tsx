import { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Button, Card, Container, Flex, Heading, Spinner, Text } from '../components/design-system';
import { useAuth } from '../contexts/auth-context';
import { processAuthCallback } from '../services/auth-service';
import './auth-callback-page.css';

export default function AuthCallbackPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const { login } = useAuth();
  const [error, setError] = useState<string | null>(null);
  const [processing, setProcessing] = useState(true);

  useEffect(() => {
    const handleCallback = async () => {
      try {
        const queryParams = new URLSearchParams(location.search);

        // Check for error parameters first
        const error = queryParams.get('error');
        const errorDescription = queryParams.get('error_description');

        if (error) {
          setError(errorDescription || error);
          setProcessing(false);
          return;
        }

        // Get remember me preference from session storage
        const rememberMe = sessionStorage.getItem('remember_me') === 'true';

        const authData = await processAuthCallback(queryParams, rememberMe);

        if (!authData) {
          setError('Authentication failed');
          setProcessing(false);
          return;
        }

        // Login with the new tokens
        login(authData.access_token, authData.refresh_token, rememberMe);

        // Clear remember me preference
        sessionStorage.removeItem('remember_me');

        // Redirect to home page after successful authentication
        navigate('/');

        setProcessing(false);
      } catch (err) {
        console.error('Error processing auth callback:', err);
        setError(err instanceof Error ? err.message : 'Authentication failed');
        setProcessing(false);
      }
    };

    handleCallback();
  }, [location, login, navigate]);

  if (processing) {
    return (
      <Container className="auth-callback-page">
        <Card className="callback-container">
          <Flex direction="col" align="center">
            <Spinner size="lg" text="Processing authentication..." />
            <Text color="secondary" className="callback-message">
              Please wait while we complete your login.
            </Text>
          </Flex>
        </Card>
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="auth-callback-page">
        <Card className="callback-container error">
          <Heading level={2} color="error">
            Authentication Error
          </Heading>
          <Text color="secondary">{error}</Text>
          <Button to="/login" variant="primary">
            Back to Login
          </Button>
        </Card>
      </Container>
    );
  }

  return null;
}
