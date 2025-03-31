# C# AuthServer Guidelines for Claude

## Build and Run Commands
- Build: `dotnet build`
- Run: `dotnet run --project AuthServer`
- Watch mode: `dotnet watch run`
- Test: `dotnet test`
- Single test: `dotnet test --filter "FullyQualifiedName=Namespace.TestClass.TestMethod"`
- Publish: `dotnet publish -c Release`

## Code Style Guidelines
- **Namespaces**: Use file-scoped namespaces (`namespace X;`)
- **Formatting**: 4-space indentation, UTF-8 encoding, LF line endings
- **Types**: Prefer `var` when type is apparent (`csharp_style_var_when_type_is_apparent = true:warning`)
- **Naming**: 
  - PascalCase for public members, types, constants
  - Private fields: camelCase with `_` prefix
  - Don't add `Async` suffix to method names unless exposing both sync/async variants
- **Error handling**: Use exceptions for exceptional conditions, null checks for expected failures
- **Async/Await**: Use async/await throughout; minimize blocking calls
- **Imports**: Place imports outside namespaces (`csharp_using_directive_placement = outside_namespace:suggestion`)
- **Attributes**: No spaces between attributes (`[Attribute1][Attribute2]`)
- **Authentication**: JWT tokens with refresh token rotation for security
- **Constructors**: Prefer primary constructors with parameters for dependency injection
- **Documentation**: XML docs required on public APIs and interfaces

## Security Guidelines
- Never log or expose JWT secrets or user credentials
- Always validate input data, especially in authentication flows
- For redirect flows, always validate the state parameter
- Keep access tokens short-lived and use refresh token rotation

Always run linter before committing changes. PR validation requires clean builds.