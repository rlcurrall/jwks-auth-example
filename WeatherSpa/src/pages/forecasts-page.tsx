import { useEffect, useState } from 'react';
import {
  Button,
  Card,
  Container,
  Heading,
  Message,
  Spinner,
  Text,
} from '../components/design-system';
import { useAuth } from '../contexts/auth-context';
import { getWeatherForecasts } from '../services/weather-service';
import { WeatherForecast } from '../types';
import './forecasts-page.css';

export default function ForecastsPage() {
  const { user } = useAuth();
  const [forecasts, setForecasts] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Check scopes
  const hasReadScope = user?.scopes.includes('weather.read');
  const hasWriteScope = user?.scopes.includes('weather.write');

  useEffect(() => {
    async function fetchForecasts() {
      try {
        setLoading(true);
        const data = await getWeatherForecasts();
        setForecasts(data);
        setError(null);
      } catch (error) {
        console.error('Error fetching forecasts:', error);
        setError('Failed to fetch weather forecasts. Please try again later.');
      } finally {
        setLoading(false);
      }
    }

    fetchForecasts();
  }, []);

  return (
    <Container size="md" className="forecasts-page">
      <header className="forecasts-page-header">
        <Heading level={3} color="brand">
          Weather Forecasts
        </Heading>
      </header>

      <Card className="forecasts-container">
        {loading ? (
          <Spinner text="Loading weather data..." fullHeight />
        ) : error ? (
          <Message variant="error" title="Error loading forecasts">
            <Text>{error}</Text>
            <Button
              onClick={() => window.location.reload()}
              variant="error"
              size="sm"
              className="ds-mt-sm"
            >
              Retry
            </Button>
          </Message>
        ) : (
          <table className="forecasts-table" aria-label="Weather forecast data">
            <caption className="ds-visually-hidden">5-Day Weather Forecast</caption>
            <thead>
              <tr>
                <th scope="col">Date</th>
                <th scope="col" aria-label="Weather icon"></th>
                <th scope="col">Description</th>
                <th scope="col">Temperature</th>
              </tr>
            </thead>
            <tbody>
              {forecasts.map((forecast, index) => (
                <tr key={index} className="forecast-row">
                  <td className="date-cell">{formatDate(forecast.date)}</td>
                  <td className="icon-cell">
                    <span className="weather-icon" role="img" aria-label={forecast.summary}>
                      {getWeatherIcon(forecast.summary)}
                    </span>
                  </td>
                  <td>{forecast.summary}</td>
                  <td className="temp-cell">
                    <span
                      className="temp-c"
                      style={{ color: getTemperatureColor(forecast.temperatureC) }}
                    >
                      {forecast.temperatureC}°C
                    </span>
                    <span className="temp-divider"> | </span>
                    <span className="temp-f">{forecast.temperatureF}°F</span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </Card>

      <Card className="permissions-info">
        <Heading level={3} color="brand">
          About API Permissions
        </Heading>
        <Text>
          This demo shows how JWT tokens with different scopes control access to API features. Your
          current token has the following permissions:
        </Text>

        <ul className="permissions-list" role="list">
          <li className={hasReadScope ? 'available' : 'unavailable'}>
            <strong className="permission-name">Read Access:</strong>
            <span className="permission-desc">
              {hasReadScope
                ? 'You can view detailed weather forecasts.'
                : 'You cannot access detailed weather data.'}
            </span>
          </li>

          <li className={hasWriteScope ? 'available' : 'unavailable'}>
            <strong className="permission-name">Write Access:</strong>
            <span className="permission-desc">
              {hasWriteScope
                ? 'You can update weather forecasts (not implemented in this simplified demo).'
                : 'You cannot modify weather data.'}
            </span>
          </li>
        </ul>
      </Card>
    </Container>
  );
}

function getTemperatureColor(temp: number) {
  // Colors aligned with temperature ranges
  if (temp <= -15) return '#93c5fd'; // Blue 300 (Freezing)
  if (temp <= -5) return '#60a5fa'; // Blue 400 (Bracing)
  if (temp <= 5) return '#38bdf8'; // Sky 400 (Chilly)
  if (temp <= 15) return '#34d399'; // Green 400 (Cool)
  if (temp <= 20) return '#4ade80'; // Green 300 (Mild)
  if (temp <= 25) return '#fcd34d'; // Amber 300 (Warm)
  if (temp <= 30) return '#fbbf24'; // Amber 400 (Balmy)
  if (temp <= 35) return '#f97316'; // Orange 500 (Hot)
  if (temp <= 45) return '#fb7185'; // Rose 400 (Sweltering)
  return '#ef4444'; // Red 500 (Scorching)
}

/**
 * Get the appropriate weather icon based on temperature and summary
 * This handles potential inconsistencies between temperature values and descriptions
 */
function getWeatherIcon(summary: string) {
  // Handle null or empty summaries
  if (!summary) return '🌡️';

  // Match the exact summaries from the API
  switch (summary.toLowerCase()) {
    case 'freezing':
      return '🥶'; // Very cold face
    case 'bracing':
      return '❄️'; // Snowflake
    case 'chilly':
      return '🧊'; // Ice
    case 'cool':
      return '😎'; // Cool face with sunglasses
    case 'mild':
      return '🌤️'; // Sun behind cloud
    case 'warm':
      return '☀️'; // Sun
    case 'balmy':
      return '🌞'; // Sun with face
    case 'hot':
      return '🌡️'; // Thermometer
    case 'sweltering':
      return '🥵'; // Hot face
    case 'scorching':
      return '🔥'; // Fire
    default:
      return '🌡️'; // Default thermometer
  }
}

function formatDate(dateString: string) {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    weekday: 'short',
    month: 'short',
    day: 'numeric',
  });
}
