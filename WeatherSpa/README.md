# Weather SPA Demo

A React SPA that demonstrates authentication with the AuthServer and consuming the WeatherApi.

## Features

- Redirect-based authentication flow
- Direct authentication with credentials
- Protected routes requiring authentication
- Role-based access control using JWT token scopes
- Integration with protected API endpoints
- JWT token management with refresh token support

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- Running instances of the AuthServer and WeatherApi

### Installation

1. Install dependencies:
   ```bash
   npm install
   ```

2. Start the development server:
   ```bash
   npm run dev
   ```

The app will be available at http://localhost:5173 by default.

### Running the Complete Solution

To run the full authentication flow:

1. Start the AuthServer (in one terminal):
   ```bash
   cd ../AuthServer
   dotnet run
   ```

2. Start the WeatherApi (in another terminal):
   ```bash
   cd ../WeatherApi
   dotnet run
   ```

3. Start this Weather SPA (in a third terminal):
   ```bash
   npm run dev
   ```

Make sure all three services are running simultaneously to test the complete authentication flow.

## Authentication Flow

This SPA demonstrates two authentication approaches:

### 1. Redirect-Based Authentication

1. The user clicks "Login with Redirect"
2. The browser is redirected to the AuthServer login page
3. After successful authentication, the user is redirected back to the SPA
4. The SPA extracts the JWT token and user info from the URL
5. The token is stored in sessionStorage and used for API requests

### 2. Direct API Authentication

1. The user enters credentials in the login form
2. The SPA sends a request to the AuthServer's login API
3. Upon successful authentication, the SPA receives a JWT token
4. The token is stored in sessionStorage and used for API requests

## Project Structure

- `/src/components` - Reusable UI components and design system
  - Design system: Button, Card, Input, Typography, etc.
  - Layout components: Container, Flex, Grid
  - Form elements: Input, FormGroup, CheckboxGroup
  - Display components: Message, Spinner, UserInfoBanner
- `/src/contexts` - React context for authentication state
- `/src/pages` - Main page components:
  - `HomePage.tsx` - Landing page for authenticated users
  - `LoginPage.tsx` - Login options selection
  - `DirectLoginPage.tsx` - Direct login with credentials
  - `RedirectLoginPage.tsx` - Redirect-based login
  - `ForecastsPage.tsx` - Weather forecast display
  - `AuthCallbackPage.tsx` - Authentication callback handler
- `/src/services` - API integration services
- `/src/types` - TypeScript type definitions

## Security Notes

- JWT tokens are stored in sessionStorage (cleared when the browser tab is closed)
- Authentication state includes token validation and expiration checks
- The AuthServer's state parameter is verified to prevent CSRF attacks
- The API service automatically adds the token to protected requests

## Design System

This application includes a comprehensive design system with the following features:

- Consistent component styling with CSS custom properties for theming
- Responsive layout components: Container, Flex, Grid
- Form components with validation: Input, FormGroup, CheckboxGroup
- Interactive elements: Button, Card, CardLink
- Feedback components: Message, Spinner
- Typography system with Heading and Text components
- Utility classes for spacing, alignment, and accessibility
- Dark mode support through CSS variables

## Demo Users

For testing purposes, the following users are available:

- Regular User
  - Username: `user1`
  - Password: `password1` 
  - Tenant: `tenant1`
  - Scopes: `read`

- Admin User
  - Username: `admin`
  - Password: `adminpass`
  - Tenant: `tenant1`
  - Scopes: `read`, `write`