import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    open: true,
    cors: true,
    proxy: {
      // Proxy API requests to the API server
      '/auth-api': {
        target: 'http://localhost:5143',
        changeOrigin: true,
        rewrite: path => path.replace(/^\/auth-api/, ''),
      },
      '/weather-api': {
        target: 'http://localhost:5006',
        changeOrigin: true,
        rewrite: path => path.replace(/^\/weather-api/, ''),
        secure: false,
      },
    },
  },
});
