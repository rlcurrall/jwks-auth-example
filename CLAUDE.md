# AuthTest Project Guidelines

## Build, Lint, and Test Commands
- **Auth Server**: `dotnet build/run/test/watch AuthServer`
- **Weather API**: `dotnet build/run/test/watch WeatherApi`
- **Weather SPA**: 
  - Development: `npm run dev` (WeatherSpa)
  - Tests/Linting: `npm run lint:fix format typecheck validate` (WeatherSpa)
- **Single Test (.NET)**: `dotnet test --filter "FullyQualifiedName=Namespace.TestClass.TestMethod"`
- **Formatting (.NET)**: `dotnet csharpier AuthServer` or `dotnet csharpier WeatherApi` (to format a specific project)

## Code Style Guidelines
- **C# (.NET)**: 
  - File-scoped namespaces, 4-space indent, PascalCase types/public members
  - Private fields: camelCase with `_` prefix
  - Imports outside namespaces, use async/await consistently
  - Use dependency injection with primary constructors
  - XML documentation on public APIs and interfaces
  - Always use ProblemDetails for API error responses with appropriate status codes
  - OAuth endpoints should include standard OAuth error codes in ProblemDetails extensions
  - Use CSharpier for code formatting (`dotnet csharpier <project>` before committing)

- **TypeScript/React**: 
  - Function components with explicit prop interfaces
  - PascalCase components, camelCase functions, kebab-case files/CSS
  - Group imports by: React, components, hooks, services, types, CSS
  - Component-scoped CSS with semantic class names
  - Use existing design system components before creating new ones

## Security
- Never log/expose JWT tokens, secrets, or credentials
- JWT with refresh token rotation in auth flows
- Validate all input data, especially in authentication flows
- Handle errors appropriately with user feedback
- Redirect authentication should use authorization codes, not direct tokens

Always run lint/validate/format before committing changes:
- .NET: `dotnet csharpier <project>` (formats C# code)
- SPA: `npm run validate` (runs typecheck, lint, format:check)