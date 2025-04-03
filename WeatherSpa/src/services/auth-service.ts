/* eslint-disable @typescript-eslint/no-explicit-any */
import { AuthToken } from '../types';
import { authApi } from './api';

// Constants
const AUTH_SERVER_URL = import.meta.env.VITE_AUTH_SERVER_URL || 'http://localhost:5143';

// Types
interface AuthData {
  access_token: string;
  refresh_token: string;
  expires_in: number;
  token_type: string;
}

// Generate a cryptographically strong random string for state
export const generateRandomState = (): string => {
  return (([1e7] as any) + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, (c: any) =>
    (c ^ (crypto.getRandomValues(new Uint8Array(1))[0] & (15 >> (c / 4)))).toString(16)
  );
};

// Generate PKCE code verifier and challenge
export const generatePkcePair = async (): Promise<{
  codeVerifier: string;
  codeChallenge: string;
}> => {
  // Generate a random code verifier
  const codeVerifier = generateRandomState();

  // Generate the code challenge using SHA-256
  const encoder = new TextEncoder();
  const data = encoder.encode(codeVerifier);
  const hash = await crypto.subtle.digest('SHA-256', data);
  const codeChallenge = btoa(String.fromCharCode(...new Uint8Array(hash)))
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=+$/, '');

  return { codeVerifier, codeChallenge };
};

// Initiate the redirect-based authentication flow
export const initiateAuthRedirect = async (
  tenant: string = '',
  scopes: string[] = ['openid', 'profile', 'email']
) => {
  const state = generateRandomState();
  const redirectUri = `${window.location.origin}/callback`;
  const { codeVerifier, codeChallenge } = await generatePkcePair();
  const clientId = 'weather-spa'; // Our client ID

  // Store state and code verifier in session storage
  sessionStorage.setItem('oauth_state', state);
  sessionStorage.setItem('code_verifier', codeVerifier);

  // Build the redirect URL
  let authUrl = `${authApi.defaults.baseURL}/oauth/authorize?client_id=${encodeURIComponent(clientId)}&redirect_uri=${encodeURIComponent(redirectUri)}&state=${state}&response_type=code&code_challenge=${codeChallenge}&code_challenge_method=S256`;

  // Add tenant if provided
  if (tenant) {
    authUrl += `&tenant=${encodeURIComponent(tenant)}`;
  }

  // Add scopes if provided (space-separated as per OAuth2 standard)
  if (scopes && scopes.length > 0) {
    authUrl += `&scope=${encodeURIComponent(scopes.join(' '))}`;
  }

  // Redirect to the authorization URL
  window.location.href = authUrl;
};

// Process the callback after authentication redirect
export async function processAuthCallback(
  queryParams: URLSearchParams,
  rememberMe: boolean = false
): Promise<AuthData | null> {
  const code = queryParams.get('code');
  const state = queryParams.get('state');
  const error = queryParams.get('error');
  const errorDescription = queryParams.get('error_description');

  if (error) {
    console.error('OAuth error:', error, errorDescription);
    throw new Error(errorDescription || error);
  }

  if (!code) {
    console.error('No authorization code received');
    return null;
  }

  // Verify state if it was sent
  const storedState = sessionStorage.getItem('oauth_state');
  if (state && storedState !== state) {
    console.error('State mismatch', { state, storedState });
    return null;
  }

  const tokenEndpoint = `${AUTH_SERVER_URL}/oauth/token`;
  const redirectUri = window.location.origin + '/callback';
  const codeVerifier = sessionStorage.getItem('code_verifier');

  if (!codeVerifier) {
    console.error('No code verifier found');
    return null;
  }

  const formData = new URLSearchParams();
  formData.append('grant_type', 'authorization_code');
  formData.append('code', code);
  formData.append('redirect_uri', redirectUri);
  formData.append('code_verifier', codeVerifier);
  formData.append('client_id', 'weather-spa');
  formData.append('remember_me', rememberMe.toString());

  try {
    const response = await fetch(tokenEndpoint, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: formData,
    });

    if (!response.ok) {
      const errorData = await response.json();
      console.error('Token request failed:', errorData);
      throw new Error(errorData.error_description || 'Failed to exchange code for token');
    }

    const data = await response.json();

    // Store refresh token based on remember me preference
    if (data.refresh_token) {
      if (rememberMe) {
        localStorage.setItem('refresh_token', data.refresh_token);
      } else {
        sessionStorage.setItem('refresh_token', data.refresh_token);
      }
    }

    // Clear the code verifier and state from session storage
    sessionStorage.removeItem('code_verifier');
    sessionStorage.removeItem('oauth_state');

    return {
      access_token: data.access_token,
      refresh_token: data.refresh_token,
      expires_in: data.expires_in,
      token_type: data.token_type,
    };
  } catch (error) {
    console.error('Error exchanging code for token:', error);
    throw error;
  }
}

// Add a function to handle token refresh
export async function refreshAccessToken(): Promise<AuthData | null> {
  const refreshToken = localStorage.getItem('refresh_token');
  if (!refreshToken) {
    return null;
  }

  const tokenEndpoint = `${AUTH_SERVER_URL}/oauth/token`;
  const formData = new URLSearchParams();
  formData.append('grant_type', 'refresh_token');
  formData.append('refresh_token', refreshToken);
  formData.append('client_id', 'weather-spa');

  try {
    const response = await fetch(tokenEndpoint, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: formData,
    });

    if (!response.ok) {
      // If refresh fails, clear the stored refresh token
      localStorage.removeItem('refresh_token');
      return null;
    }

    const data = await response.json();

    // Store the new refresh token
    if (data.refresh_token) {
      localStorage.setItem('refresh_token', data.refresh_token);
    }

    return {
      access_token: data.access_token,
      refresh_token: data.refresh_token,
      expires_in: data.expires_in,
      token_type: data.token_type,
    };
  } catch (error) {
    console.error('Error refreshing token:', error);
    return null;
  }
}

// Direct login via API
export const loginWithCredentials = async (
  username: string,
  password: string,
  tenant: string,
  scopes: string[] = ['openid', 'profile', 'email']
): Promise<AuthToken> => {
  const response = await authApi.post<AuthToken>('/api/login', {
    username,
    password,
    tenant,
    scopes,
  });

  return response.data;
};

// Refresh the access token
export const refreshToken = async (refreshToken: string): Promise<AuthToken> => {
  // Create form data for the token request
  const formData = new URLSearchParams();
  formData.append('grant_type', 'refresh_token');
  formData.append('refresh_token', refreshToken);

  const response = await authApi.post<AuthToken>('/oauth/token', formData, {
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded',
    },
  });

  return response.data;
};

// Get user info
export const getUserInfo = async (): Promise<any> => {
  const response = await authApi.get('/oauth/userinfo');
  return response.data;
};
