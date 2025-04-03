export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

export interface AuthToken {
  access_token: string;
  expires_in: string;
  refresh_token: string;
  refresh_token_expires_in: string;
  scope?: string;
}

export interface Claims {
  sub: string;
  name?: string;
  tenant?: string;
  roles?: string[];
  scope?: string[];
  exp: number;
  iat: number;
  iss: string;
  aud: string;
}

export interface User {
  userId: string;
  username: string;
  tenant: string;
  roles: string[];
  scopes: string[];
}

export interface AuthState {
  isAuthenticated: boolean;
  user: User | null;
  token: string | null;
  loading: boolean;
  error: string | null;
}
