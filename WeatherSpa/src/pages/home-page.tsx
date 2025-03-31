import { Button, Card, Flex, Grid, Heading, Hero, Text } from '../components/design-system';
import { useAuth } from '../contexts/auth-context';
import './home-page.css';

export default function HomePage() {
  const { isAuthenticated, user } = useAuth();

  return (
    <>
      <Hero
        title="Weather SPA Demo"
        subtitle="A demonstration of authentication and authorization with JWT tokens"
      >
        {isAuthenticated ? (
          <Card className="welcome-message">
            <Heading level={2} color="brand">
              Welcome, {user?.username}!
            </Heading>
            <Text>
              You are successfully authenticated. You can now access the weather forecast data from
              the protected API.
            </Text>
            <Flex justify="center" className="action-buttons">
              <Button to="/forecasts" variant="primary">
                View Weather Forecasts
              </Button>
            </Flex>
          </Card>
        ) : (
          <Card className="hero-cta">
            <Text>
              To access the weather forecast data, you need to authenticate using the Auth Server.
            </Text>
            <Flex justify="center" gap="md" className="auth-buttons">
              <Button to="/login/redirect" variant="primary">
                Login with Redirect
              </Button>
              <Button to="/login/direct" variant="secondary">
                Login with Credentials
              </Button>
            </Flex>
          </Card>
        )}
      </Hero>

      <section className="features">
        <Heading level={2} align="center" className="features-title">
          Key Features
        </Heading>
        <Grid cols={2} responsive={{ md: 3 }} gap="lg" className="features-grid">
          <Card title="JWT Authentication" variant="outline">
            <Text margin={false}>
              Secure authentication using JSON Web Tokens with asymmetric keys.
            </Text>
          </Card>

          <Card title="Role-Based Access Control" variant="outline">
            <Text margin={false}>
              Different API endpoints require different scopes for authorization.
            </Text>
          </Card>

          <Card title="OAuth-Style Redirect Flow" variant="outline">
            <Text margin={false}>Supports modern redirect-based authentication flow for SPAs.</Text>
          </Card>
        </Grid>
      </section>
    </>
  );
}
