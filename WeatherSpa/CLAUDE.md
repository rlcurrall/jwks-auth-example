# WeatherSpa Development Guidelines

## Commands
- **Dev server**: `npm run dev`
- **Build**: `npm run build`
- **Preview build**: `npm run preview`
- **Lint**: `npm run lint` (check), `npm run lint:fix` (auto-fix)
- **Format**: `npm run format` (fix), `npm run format:check` (check)
- **Typecheck**: `npm run typecheck`, `npm run typecheck:watch` (watch mode)
- **Validate**: `npm run validate` (runs typecheck, lint, format:check)

## Code Style
- **Components**: Function components with explicit prop interfaces
- **Naming**: PascalCase for components, camelCase for functions, kebab-case for files/CSS
- **Typing**: Strong TypeScript typing, explicit interfaces for props/state
- **Imports**: Group by: React, components, hooks/contexts, services, types, CSS (last)
- **CSS**: Component-scoped CSS files with kebab-case classnames
- **Error handling**: Try/catch for API calls, explicit error states with UI feedback
- **State management**: Context API for auth, local state with proper typing
- **Architecture**: Separate concerns (pages, components, services, contexts)
- **Design system**: Use existing components before creating new ones