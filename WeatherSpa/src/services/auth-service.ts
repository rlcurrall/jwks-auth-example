/* eslint-disable @typescript-eslint/no-explicit-any */
import { AuthToken } from '../types';
import { authApi } from './api';

// Generate a cryptographically strong random string for state
export const generateRandomState = (): string => {
  return (([1e7] as any) + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, (c: any) =>
    (c ^ (crypto.getRandomValues(new Uint8Array(1))[0] & (15 >> (c / 4)))).toString(16)
  );
};

// Initiate the redirect-based authentication flow
export const initiateAuthRedirect = (tenant: string = '', scopes: string[] = ['user_info']) => {
  const state = generateRandomState();
  const redirectUri = `${window.location.origin}/callback`;

  sessionStorage.setItem('auth_state', state);

  // Build the redirect URL
  let authUrl = `${authApi.defaults.baseURL}/api/authorize?redirectUri=${encodeURIComponent(redirectUri)}&state=${state}`;

  // Add tenant if provided
  if (tenant) {
    authUrl += `&tenant=${encodeURIComponent(tenant)}`;
  }

  // Add scopes if provided (space-separated as per OAuth2 standard)
  if (scopes && scopes.length > 0) {
    authUrl += `&scopes=${encodeURIComponent(scopes.join(' '))}`;
  }

  // Redirect to the authorization URL
  window.location.href = authUrl;
};

// Process the callback after authentication redirect
export const processAuthCallback = (
  queryParams: URLSearchParams
): {
  token: string;
  refreshToken: string;
  state: string;
} | null => {
  const token = queryParams.get('token');
  const refreshToken = queryParams.get('refreshToken');
  const state = queryParams.get('state');
  const storedState = sessionStorage.getItem('auth_state');

  if (!token) {
    console.error('Missing token parameter in callback');
    return null;
  }

  if (!refreshToken) {
    console.error('Missing refreshToken parameter in callback');
    return null;
  }

  if (!state) {
    console.error('Missing state parameter in callback');
    return null;
  }

  if (state !== storedState) {
    console.error('State parameter mismatch in callback');
    return null;
  }

  return {
    token,
    refreshToken,
    state,
  };
};

// Direct login via API
export const loginWithCredentials = async (
  username: string,
  password: string,
  tenant: string,
  scopes: string[] = ['user_info', 'read', 'write']
): Promise<AuthToken> => {
  const response = await authApi.post<AuthToken>('/api/login', {
    username,
    password,
    tenant,
    scopes,
  });

  return response.data;
};
