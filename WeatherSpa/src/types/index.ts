export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

export interface AuthToken {
  token: string;
  expiration: string;
  refreshToken: string;
  refreshTokenExpiration: string;
}

export interface Claims {
  aud: string;
  exp: number;
  iss: string;
  jti: string;
  sub: string;
  name: string;
  tenant: string;
  roles: string[];
  scope: string[];
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
