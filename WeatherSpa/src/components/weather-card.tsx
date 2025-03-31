import { WeatherForecast } from '../types';
import './weather-card.css';

interface WeatherCardProps {
  forecast: WeatherForecast;
  detailed?: boolean;
}

function WeatherCard({ forecast, detailed = false }: WeatherCardProps) {
  // Convert ISO date string to readable format
  function formatDate(dateString: string) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'short',
      month: 'short',
      day: 'numeric',
    });
  }

  // Get appropriate weather icon based on summary
  function getWeatherIcon(summary: string) {
    const lowercaseSummary = summary?.toLowerCase() || '';

    if (lowercaseSummary.includes('sun') || lowercaseSummary.includes('clear')) {
      return '☀️';
    } else if (lowercaseSummary.includes('cloud')) {
      return '☁️';
    } else if (lowercaseSummary.includes('rain')) {
      return '🌧️';
    } else if (lowercaseSummary.includes('snow')) {
      return '❄️';
    } else if (lowercaseSummary.includes('storm') || lowercaseSummary.includes('thunder')) {
      return '⛈️';
    } else if (lowercaseSummary.includes('fog') || lowercaseSummary.includes('mist')) {
      return '🌫️';
    } else if (lowercaseSummary.includes('wind')) {
      return '💨';
    } else if (lowercaseSummary.includes('hot') || lowercaseSummary.includes('scorch')) {
      return '🔥';
    } else if (lowercaseSummary.includes('freez') || lowercaseSummary.includes('cold')) {
      return '🥶';
    }

    return '🌡️';
  }

  // Get appropriate color based on temperature
  function getTemperatureColor(temp: number) {
    if (temp <= 0) return '#add8e6'; // light blue
    if (temp <= 10) return '#87ceeb'; // sky blue
    if (temp <= 20) return '#90ee90'; // light green
    if (temp <= 30) return '#ffff00'; // yellow
    if (temp <= 40) return '#ffa500'; // orange
    return '#ff0000'; // red
  }

  return (
    <div className={`weather-card ${detailed ? 'detailed' : ''}`}>
      <div className="weather-icon" style={{ fontSize: detailed ? '4rem' : '2.5rem' }}>
        {getWeatherIcon(forecast.summary || '')}
      </div>

      <div className="weather-content">
        <div className="weather-date">{formatDate(forecast.date)}</div>
        <div className="weather-summary">{forecast.summary}</div>

        <div className="weather-temp">
          <span className="temp-c" style={{ color: getTemperatureColor(forecast.temperatureC) }}>
            {forecast.temperatureC}°C
          </span>
          <span className="temp-divider">|</span>
          <span className="temp-f">{forecast.temperatureF}°F</span>
        </div>
      </div>
    </div>
  );
}

export default WeatherCard;
