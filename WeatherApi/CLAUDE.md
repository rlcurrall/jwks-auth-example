# C# WeatherApi Guidelines for Claude

## Build and Run Commands
- Build: `dotnet build`
- Run: `dotnet run --project WeatherApi`
- Watch mode: `dotnet watch run`
- Test: `dotnet test`
- Single test: `dotnet test --filter "FullyQualifiedName=Namespace.TestClass.TestMethod"`
- Publish: `dotnet publish -c Release`

## Code Style Guidelines
- **Namespaces**: Use file-scoped namespaces (`namespace X;`)
- **Formatting**: 4-space indentation, UTF-8 encoding, LF line endings
- **Types**: Prefer `var` when type is apparent
- **Naming**: 
  - PascalCase for public members, types, constants
  - Private fields: camelCase with `_` prefix
  - Don't add `Async` suffix to method names unless exposing both sync/async variants
- **Error handling**: Use exceptions for exceptional conditions, null checks for expected failures
- **Async/Await**: Use async/await throughout; minimize blocking calls
- **Imports**: Place imports outside namespaces
- **Attributes**: No spaces between attributes (`[Attribute1][Attribute2]`)
- **Authentication**: JWT validation with JWKS endpoint for public key retrieval
- **Constructors**: Prefer primary constructors with parameters for dependency injection
- **Documentation**: XML docs required on public APIs and interfaces

## Security Guidelines
- Never log or expose JWT tokens or sensitive claims
- Always validate authorization scopes for protected endpoints
- Keep JWKS URL in configuration, not hardcoded
- Use appropriate policies for specific permission requirements

Always run linter before committing changes. PR validation requires clean builds.