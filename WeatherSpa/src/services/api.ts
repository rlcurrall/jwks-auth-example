import axios from 'axios';

const AUTH_API_URL = 'http://localhost:5143';
const WEATHER_API_URL = 'http://localhost:5006';

export const authApi = axios.create({
  baseURL: AUTH_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const weatherApi = axios.create({
  baseURL: WEATHER_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to weather API requests
export const setupAuthInterceptor = (getToken: () => string | null) => {
  weatherApi.interceptors.request.use(
    config => {
      const token = getToken();
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    error => {
      return Promise.reject(error);
    }
  );
};
