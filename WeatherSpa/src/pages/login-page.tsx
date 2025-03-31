import { Hero, Container, CardLink, Heading, Grid } from '../components/design-system';
import './login-page.css';

export default function LoginPage() {
  return (
    <>
      <Hero
        title="Login"
        subtitle="Access your Weather SPA account to view personalized forecasts"
        size="sm"
      />

      <Container className="login-page">
        <article className="login-container">
          <Heading level={2} align="center" className="login-options-title">
            Choose a login method
          </Heading>

          <div className="login-options">
            <Grid cols={2} className="login-cards" gap="md">
              <CardLink
                title="Redirect Login"
                description="Login using the redirect flow. You'll be redirected to the Auth Server for authentication."
                actionText="Use Redirect Login"
                to="/login/redirect"
                buttonVariant="primary"
              />

              <CardLink
                title="Direct Login"
                description="Login directly by entering your credentials within this application."
                actionText="Use Direct Login"
                to="/login/direct"
                buttonVariant="secondary"
              />
            </Grid>
          </div>
        </article>
      </Container>
    </>
  );
}
