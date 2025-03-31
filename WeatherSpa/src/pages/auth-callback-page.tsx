import { useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { Button, Card, Spinner, Container, Heading, Text, Flex } from '../components/design-system';
import { useAuth } from '../contexts/auth-context';
import { processAuthCallback } from '../services/auth-service';
import './auth-callback-page.css';

export default function AuthCallbackPage() {
  const location = useLocation();
  const { login } = useAuth();
  const [error, setError] = useState<string | null>(null);
  const [processing, setProcessing] = useState(true);

  useEffect(() => {
    try {
      const queryParams = new URLSearchParams(location.search);
      const authData = processAuthCallback(queryParams);

      if (!authData) {
        setError('Authentication failed. Invalid or missing parameters.');
        setProcessing(false);
        return;
      }

      // Process successful authentication
      login(authData.token, authData.refreshToken);

      setProcessing(false);
    } catch (error) {
      console.error('Error processing authentication callback:', error);
      setError('An error occurred while processing the authentication.');
      setProcessing(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (!processing && !error) {
    return <Navigate to="/forecasts" replace />;
  }

  return (
    <Container className="auth-callback-page">
      {processing ? (
        <Card className="callback-container">
          <Flex direction="column" align="center">
            <Spinner size="lg" text="Processing authentication..." />
            <Text color="secondary" className="callback-message">
              Please wait while we complete your login.
            </Text>
          </Flex>
        </Card>
      ) : (
        <Card className="callback-container error">
          <Heading level={2} color="error">
            Authentication Error
          </Heading>
          <Text color="secondary">{error}</Text>
          <Button to="/login" variant="primary">
            Back to Login
          </Button>
        </Card>
      )}
    </Container>
  );
}
