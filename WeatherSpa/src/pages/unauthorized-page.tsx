import {
  Button,
  Message,
  UserInfoBanner,
  Container,
  Card,
  Heading,
  Text,
  Flex,
} from '../components/design-system';
import { useAuth } from '../contexts/auth-context';
import './unauthorized-page.css';

export default function UnauthorizedPage() {
  const { user } = useAuth();

  return (
    <Container className="unauthorized-page">
      <Card className="unauthorized-container">
        <Message variant="warning" title="Access Denied" className="unauthorized-message">
          You do not have the required permissions to access this page or resource.
        </Message>

        {user && (
          <div className="user-permissions-section">
            <Heading level={2} size="md" color="text">
              Your Current Permissions
            </Heading>
            <Text color="secondary">Your authentication token includes the following scopes:</Text>

            <div className="user-banner-container">
              <UserInfoBanner
                user={user}
                variant="primary"
                displayedScopes={user.scopes}
                className="unauthorized-user-banner"
              />
            </div>

            <Text size="sm" color="tertiary" className="help-text">
              You may need additional scopes to access this resource. Try logging in again with a
              different user or requesting additional permissions.
            </Text>
          </div>
        )}

        <Flex justify="center" gap="md" className="action-buttons">
          <Button to="/" variant="secondary">
            Back to Home
          </Button>
          <Button to="/login" variant="primary">
            Login Again
          </Button>
        </Flex>
      </Card>
    </Container>
  );
}
