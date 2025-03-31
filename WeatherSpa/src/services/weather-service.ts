/* eslint-disable @typescript-eslint/no-explicit-any */
import { WeatherForecast } from '../types';
import { weatherApi } from './api';

// Get all weather forecasts
export const getWeatherForecasts = async (): Promise<WeatherForecast[]> => {
  try {
    const response = await weatherApi.get<WeatherForecast[]>('/api/weatherforecast');
    return response.data;
  } catch (error) {
    console.error('Error fetching weather forecasts:', error);
    throw error;
  }
};

// Get a specific weather forecast by ID (requires "read" scope)
export const getWeatherForecastById = async (id: number): Promise<WeatherForecast> => {
  try {
    const response = await weatherApi.get<WeatherForecast>(`/api/weatherforecast/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching weather forecast with ID ${id}:`, error);
    throw error;
  }
};

// Update a weather forecast (requires "write" scope)
export const updateWeatherForecast = async (
  id: number,
  forecast: Pick<WeatherForecast, 'temperatureC' | 'summary'>
): Promise<WeatherForecast> => {
  try {
    const response = await weatherApi.put<WeatherForecast>(`/api/weatherforecast/${id}`, forecast);
    return response.data;
  } catch (error) {
    console.error(`Error updating weather forecast with ID ${id}:`, error);
    throw error;
  }
};

// Get user information from the JWT token
export const getUserInfo = async (): Promise<any> => {
  try {
    const response = await weatherApi.get('/api/weatherforecast/me');
    return response.data;
  } catch (error) {
    console.error('Error fetching user info:', error);
    throw error;
  }
};
