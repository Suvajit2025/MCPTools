# MCPTools Coding Standards

## 1. Purpose

This document defines the official coding standards for **MCPTools**, an enterprise-grade open-source .NET 10 framework for building Model Context Protocol (MCP) tools and developer automation utilities.

Coding standards are important because they:

- Keep the codebase readable and predictable.
- Make contributions easier to review and maintain.
- Reduce defects caused by inconsistent implementation patterns.
- Support long-term extensibility and testability.
- Align the project with Microsoft .NET Design Guidelines.
- Help MCPTools remain professional, stable, and welcoming to contributors.

All contributors are expected to follow these standards when creating or modifying code.

## 2. General Principles

| Principle | Standard |
| --- | --- |
| Readability | Code should be easy to understand without unnecessary comments or cleverness. |
| Maintainability | Components should be small, cohesive, and easy to change safely. |
| Simplicity | Prefer straightforward implementation over excessive abstraction. |
| Consistency | Follow project conventions even when multiple valid approaches exist. |
| Performance | Avoid wasteful patterns in common execution paths. |
| Testability | Design code so behavior can be verified with automated tests. |

Code should express intent clearly. Names, structure, and flow should make the behavior understandable before a developer reaches for a debugger.

## 3. C# Language Standards

MCPTools targets **.NET 10** and **C# 14**. Use modern C# features when they improve clarity, safety, or maintainability.

### Nullable Reference Types

Nullable reference types must be enabled and respected.

Use nullable annotations to communicate whether a value may be absent.

Good:

```csharp
public sealed class TemplateOptions
{
    public string TemplateRootPath { get; init; } = "Templates";
    public string? DefaultNamespace { get; init; }
}
```

Avoid suppressing nullable warnings unless there is a clear reason.

```csharp
var template = LoadTemplate(name)!; // Avoid unless the invariant is proven.
```

### Implicit vs Explicit Types

Use `var` when the type is obvious from the right side of the assignment.

Good:

```csharp
var request = new GenerateCrudRequest
{
    EntityName = "Customer",
    OutputPath = "src/Generated"
};
```

Use explicit types when they improve readability.

Good:

```csharp
IReadOnlyList<string> generatedFiles = await generator.GenerateAsync(request, cancellationToken);
```

Avoid:

```csharp
var result = await service.ExecuteAsync(input, cancellationToken); // Unclear if the result type matters.
```

### Expression-Bodied Members

Use expression-bodied members for simple members where readability is improved.

Good:

```csharp
public bool HasWarnings => Warnings.Count > 0;

public override string ToString() => Name;
```

Avoid expression-bodied members for complex logic.

```csharp
public bool IsValid => Name.Length > 0 && Path.Exists(OutputPath) && Options.Count > 0; // Prefer a method.
```

### File-Scoped Namespaces

Use file-scoped namespaces for new C# files.

Good:

```csharp
namespace MCPTools.Core.Tools;

public sealed class GenerateCrudTool
{
}
```

Avoid block-scoped namespaces in new files unless required by generated code or legacy compatibility.

### Pattern Matching

Use pattern matching when it improves clarity.

Good:

```csharp
if (request is null)
{
    throw new ToolValidationException("Request cannot be null.");
}

if (result is { Succeeded: false, Errors.Count: > 0 })
{
    logger.LogWarning("Tool completed with validation errors.");
}
```

Avoid overly dense patterns that obscure behavior.

### Records

Use records for immutable data contracts, value-like models, requests, responses, descriptors, and metadata.

Good:

```csharp
public sealed record ToolMetadata(
    string Name,
    string Description,
    ToolCategory Category);
```

Use classes when behavior, lifecycle, identity, or dependency injection is involved.

### Required Properties

Use `required` properties for request and options models when the caller must provide a value.

Good:

```csharp
public sealed class AnalyzeSolutionRequest
{
    public required string SolutionPath { get; init; }
}
```

Required properties should not replace validation. They help object initialization but do not guarantee semantic correctness.

### Primary Constructors

Primary constructors may be used when they reduce ceremony and keep dependencies clear.

Good:

```csharp
public sealed class TemplateRenderer(
    ITemplateLoader templateLoader,
    ILogger<TemplateRenderer> logger)
{
    public async ValueTask<string> RenderAsync(
        string templateName,
        object model,
        CancellationToken cancellationToken = default)
    {
        var template = await templateLoader.LoadAsync(templateName, cancellationToken);
        logger.LogDebug("Rendering template {TemplateName}", templateName);
        return template.Render(model);
    }
}
```

Avoid primary constructors when initialization logic, validation, or multiple overloads would make the type harder to read.

## 4. File Organization

### One Public Type Per File

Each public type should be placed in its own file.

Good:

```text
GenerateCrudTool.cs
GenerateCrudRequest.cs
GenerateCrudResponse.cs
```

Internal helper types may be colocated only when they are small, private to the file, and not useful elsewhere.

### File Naming

File names must match the public type name.

| Type | File |
| --- | --- |
| `GenerateCrudTool` | `GenerateCrudTool.cs` |
| `ITemplateEngine` | `ITemplateEngine.cs` |
| `ToolValidationException` | `ToolValidationException.cs` |
| `McpToolsOptions` | `McpToolsOptions.cs` |

### Folder Organization

Place files in folders that match their responsibility.

```text
MCPTools.Core/
|-- Tools/
|-- Services/
|-- Models/
|-- Interfaces/
|-- Exceptions/
|-- Extensions/
|-- Configuration/
|-- Utilities/
|-- Templates/
```

### Namespace Alignment

Namespaces should align with project and folder structure.

```csharp
namespace MCPTools.Core.Tools;
namespace MCPTools.Core.Services;
namespace MCPTools.Core.Configuration;
```

## 5. Class Design

### Single Responsibility Principle

Each class should have one primary responsibility.

Good:

```csharp
public sealed class ProjectAnalyzer
{
    public ProjectAnalysisResult Analyze(string projectPath)
    {
        // Project analysis only.
    }
}
```

Avoid combining unrelated behavior:

```csharp
public sealed class ProjectAnalyzer
{
    public ProjectAnalysisResult Analyze(string projectPath) { }
    public string RenderTemplate(string templateName) { }
    public void WriteFiles(string outputPath) { }
}
```

### Constructor Injection

Use constructor injection for required dependencies.

```csharp
public sealed class GenerateCrudTool(
    ICrudGenerator generator,
    ILogger<GenerateCrudTool> logger)
{
}
```

Dependencies should be explicit and immutable.

### Small Classes

Classes should stay focused and cohesive. If a class grows many dependencies, methods, or responsibilities, split it into smaller services.

### Static Classes

Use static classes only for:

- Extension methods.
- Pure utility methods.
- Constants.
- Stateless helpers.

Avoid static classes for services that require configuration, logging, I/O, or test substitution.

### Avoid God Objects

A God Object is a class that knows too much or does too much.

Warning signs:

- Many unrelated public methods.
- Large constructor dependency lists.
- Multiple reasons to change.
- Direct access to many infrastructure concerns.
- Business logic mixed with logging, file I/O, template rendering, and protocol handling.

## 6. Method Design

### Method Naming

Methods should use PascalCase and begin with a verb or verb phrase.

Examples:

```csharp
ExecuteAsync
ValidateRequest
RenderTemplateAsync
AnalyzeSolutionAsync
RegisterTool
```

### Method Length

Methods should be short enough to understand at a glance. As a guideline, prefer methods under 30 lines. Longer methods are acceptable only when the logic is linear, clear, and difficult to split meaningfully.

### Single Responsibility

A method should perform one logical operation.

Good:

```csharp
public async ValueTask<GenerateCrudResponse> ExecuteAsync(
    GenerateCrudRequest request,
    CancellationToken cancellationToken = default)
{
    ValidateRequest(request);

    var result = await generator.GenerateAsync(request, cancellationToken);

    return CreateResponse(result);
}
```

### Parameter Limits

Prefer no more than three parameters for public methods. Use request or options objects for larger input sets.

Avoid:

```csharp
GenerateCrud("Customer", "App.Models", "src", true, false, true);
```

Prefer:

```csharp
await tool.ExecuteAsync(new GenerateCrudRequest
{
    EntityName = "Customer",
    Namespace = "App.Models",
    OutputPath = "src",
    IncludeRepository = true,
    IncludeService = false,
    IncludeController = true
}, cancellationToken);
```

### Return Values

Return meaningful values. Avoid returning `null` for expected outcomes when an empty collection, result object, or optional value communicates intent better.

Good:

```csharp
public IReadOnlyList<ToolMetadata> GetRegisteredTools()
{
    return registeredTools;
}
```

### Early Returns

Use early returns to reduce nesting and improve clarity.

Good:

```csharp
public ValidationResult Validate(GenerateCrudRequest request)
{
    if (string.IsNullOrWhiteSpace(request.EntityName))
    {
        return ValidationResult.Failure("Entity name is required.");
    }

    if (string.IsNullOrWhiteSpace(request.OutputPath))
    {
        return ValidationResult.Failure("Output path is required.");
    }

    return ValidationResult.Success();
}
```

## 7. Interface Design

### When to Create Interfaces

Create an interface when:

- Multiple implementations are expected.
- The dependency crosses an architectural boundary.
- The type needs to be mocked or substituted in tests.
- The contract is part of the public extension model.
- The implementation depends on infrastructure such as file systems, databases, or external services.

Do not create an interface automatically for every class.

### Interface Segregation

Interfaces should be small and focused.

Good:

```csharp
public interface ITemplateRenderer
{
    ValueTask<string> RenderAsync(
        string templateName,
        object model,
        CancellationToken cancellationToken = default);
}
```

Avoid:

```csharp
public interface IToolServices
{
    ValueTask<string> RenderTemplateAsync(string name, object model);
    ValueTask WriteFileAsync(string path, string content);
    ValueTask<ProjectAnalysisResult> AnalyzeProjectAsync(string path);
    ValueTask<IReadOnlyList<string>> QueryDatabaseAsync(string connectionString);
}
```

### Naming Conventions

Interfaces must use the `I` prefix.

Examples:

- `ITool`
- `ITemplateEngine`
- `IProjectAnalyzer`
- `IFileSystemService`

### Avoid Empty Interfaces

Empty marker interfaces should be avoided unless they are part of a deliberate framework extension mechanism and are clearly documented.

## 8. Dependency Injection

MCPTools uses standard .NET dependency injection through `Microsoft.Extensions.DependencyInjection`.

### Constructor Injection

Constructor injection is the default pattern for required dependencies.

```csharp
public sealed class AnalyzeSolutionTool(
    IProjectAnalyzer projectAnalyzer,
    ILogger<AnalyzeSolutionTool> logger)
{
}
```

### Lifetime Selection

| Lifetime | Use For |
| --- | --- |
| Singleton | Stateless, thread-safe services and immutable shared metadata. |
| Scoped | Execution-scoped services when a host provides request scope. |
| Transient | Lightweight services and tools that should be created per use. |

Avoid singleton services that depend on scoped services.

### Avoid Service Locator

Do not resolve dependencies directly from `IServiceProvider` inside application logic.

Avoid:

```csharp
var renderer = serviceProvider.GetRequiredService<ITemplateRenderer>();
```

Prefer:

```csharp
public sealed class GenerateDocumentationTool(ITemplateRenderer renderer)
{
}
```

### Registration Conventions

Use extension methods for framework registration.

```csharp
services
    .AddMcpTools()
    .AddMcpTool<GenerateCrudTool>()
    .AddMcpTool<AnalyzeSolutionTool>();
```

## 9. Exception Handling

### Custom Exceptions

Use framework-specific exceptions for known framework failures.

Examples:

```csharp
public class McpToolsException : Exception
public class ToolValidationException : McpToolsException
public class ToolExecutionException : McpToolsException
public class TemplateException : McpToolsException
```

### Validation Exceptions

Use validation exceptions for invalid input or invalid preconditions.

```csharp
if (string.IsNullOrWhiteSpace(request.EntityName))
{
    throw new ToolValidationException("Entity name is required.");
}
```

### Never Swallow Exceptions

Do not catch exceptions without handling, logging, or rethrowing them.

Avoid:

```csharp
try
{
    await generator.GenerateAsync(request, cancellationToken);
}
catch
{
}
```

### Preserve Stack Traces

Use `throw;` to rethrow the current exception.

Good:

```csharp
catch (IOException ex)
{
    logger.LogError(ex, "Failed to write generated file {FilePath}", filePath);
    throw;
}
```

Avoid:

```csharp
catch (IOException ex)
{
    throw ex;
}
```

### Logging Exceptions

Log exceptions at the boundary where they are handled or translated. Avoid duplicate logging at every layer.

## 10. Logging

MCPTools uses `ILogger<T>` from `Microsoft.Extensions.Logging`.

### Structured Logging

Use structured logging templates.

Good:

```csharp
logger.LogInformation(
    "Executing tool {ToolName} with request id {RequestId}",
    toolName,
    requestId);
```

Avoid:

```csharp
logger.LogInformation($"Executing tool {toolName} with request id {requestId}");
```

### Log Levels

| Level | Use For |
| --- | --- |
| `Trace` | Very detailed diagnostic information, usually disabled by default. |
| `Debug` | Developer diagnostics useful during troubleshooting. |
| `Information` | Normal lifecycle events such as tool start and completion. |
| `Warning` | Recoverable issues, degraded behavior, or skipped optional work. |
| `Error` | Failed operations that require attention. |
| `Critical` | Severe failures that may compromise the host process. |

### Sensitive Data

Never log:

- API keys.
- Access tokens.
- Passwords.
- Connection strings.
- Private source code unless explicitly approved by the host.
- Raw request payloads that may contain secrets.

### Correlation IDs

Future framework versions should support correlation IDs for tracing tool executions across MCP clients, hosts, services, and logs.

When available, include correlation IDs in structured log messages.

## 11. Async Programming

### Async All the Way

Use async APIs for I/O-bound operations and avoid blocking async code.

Avoid:

```csharp
var content = File.ReadAllText(path);
var result = service.ExecuteAsync(request).Result;
```

Prefer:

```csharp
var content = await File.ReadAllTextAsync(path, cancellationToken);
var result = await service.ExecuteAsync(request, cancellationToken);
```

### CancellationToken

Public async methods should accept a `CancellationToken` when work may be long-running or cancellable.

```csharp
public ValueTask<GenerateCrudResponse> ExecuteAsync(
    GenerateCrudRequest request,
    CancellationToken cancellationToken = default);
```

Pass the token to downstream async calls.

### Avoid async void

Use `async Task` or `async ValueTask`. `async void` is allowed only for event handlers.

### ConfigureAwait Guidance

Library code may use `ConfigureAwait(false)` when appropriate, especially in lower-level reusable components.

Application or host code may omit it when synchronization context behavior is desired.

### Exception Handling in Async Methods

Handle async exceptions using normal `try`/`catch` around awaited calls.

```csharp
try
{
    return await tool.ExecuteAsync(request, cancellationToken);
}
catch (ToolValidationException)
{
    throw;
}
catch (Exception ex)
{
    logger.LogError(ex, "Tool execution failed.");
    throw new ToolExecutionException("Tool execution failed.", ex);
}
```

## 12. Configuration

### Strongly Typed Options

Use strongly typed options for configuration.

```csharp
public sealed class TemplateOptions
{
    public string TemplateRootPath { get; init; } = "Templates";
    public int MaxTemplateSize { get; init; } = 1024 * 1024;
}
```

Register options through the standard options pattern.

```csharp
services
    .AddOptions<TemplateOptions>()
    .Bind(configuration.GetSection("MCPTools:Templates"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### appsettings.json

Use `appsettings.json` for default host configuration.

```json
{
  "MCPTools": {
    "Templates": {
      "TemplateRootPath": "Templates",
      "MaxTemplateSize": 1048576
    }
  }
}
```

### Environment Variables

Environment variables may override configuration in deployment or local development environments.

Example:

```text
MCPTools__Templates__TemplateRootPath=Templates
```

### Avoid Hardcoded Values

Avoid hardcoded paths, limits, timeouts, and environment-specific values in framework code. Use options, constants, or well-documented defaults.

## 13. Testing Standards

MCPTools should be designed and maintained with automated testing as a first-class concern.

### Unit Tests

Unit tests should cover:

- Validation behavior.
- Successful execution paths.
- Known failure paths.
- Boundary conditions.
- Serialization-friendly request and response models.
- Service behavior independent of MCP clients.

### Arrange-Act-Assert

Use the Arrange-Act-Assert pattern.

```csharp
[Fact]
public async Task ExecuteAsync_ReturnsGeneratedFiles_WhenRequestIsValid()
{
    // Arrange
    var request = new GenerateCrudRequest
    {
        EntityName = "Customer",
        OutputPath = "Generated"
    };

    var tool = CreateTool();

    // Act
    var response = await tool.ExecuteAsync(request);

    // Assert
    Assert.True(response.Succeeded);
    Assert.NotEmpty(response.GeneratedFiles);
}
```

### Test Naming

Use descriptive test names.

Recommended pattern:

```text
MethodName_ExpectedBehavior_WhenCondition
```

Examples:

- `ExecuteAsync_ReturnsResponse_WhenRequestIsValid`
- `ExecuteAsync_ThrowsToolValidationException_WhenEntityNameIsMissing`
- `RenderAsync_ReturnsRenderedTemplate_WhenTemplateExists`

### Mocking

Use mocks or test doubles for external dependencies such as:

- File systems.
- Databases.
- Network services.
- Clocks.
- External processes.
- Host-specific services.

Do not mock simple value objects or framework primitives unnecessarily.

### Code Coverage Goals

Coverage should focus on meaningful behavior, not vanity percentages.

Recommended goals:

| Area | Coverage Expectation |
| --- | --- |
| Core abstractions | High coverage for contract behavior. |
| Tool validation | High coverage for success and failure paths. |
| Services | High coverage for business logic. |
| Utilities | High coverage for deterministic helpers. |
| Host integrations | Integration tests where practical. |

## 14. Documentation

### XML Comments

Public APIs should include XML comments when they are part of the framework surface area or extension model.

```csharp
/// <summary>
/// Renders a template using the provided model.
/// </summary>
public interface ITemplateRenderer
{
    /// <summary>
    /// Renders the specified template asynchronously.
    /// </summary>
    ValueTask<string> RenderAsync(
        string templateName,
        object model,
        CancellationToken cancellationToken = default);
}
```

Avoid comments that merely repeat the code.

### README Updates

Update `README.md` when changes affect:

- Getting started guidance.
- Public APIs.
- Supported features.
- Project setup.
- Example usage.

### Markdown Documentation

Update Markdown documents when changes affect:

- Architecture.
- Tool lifecycle.
- Naming conventions.
- Coding standards.
- MCP integration.
- Template engine behavior.
- Roadmap or design decisions.

### Public API Documentation

Public APIs should be documented with enough detail for external developers to use them correctly without reading the implementation.

## 15. Performance Guidelines

Performance should be considered in framework design, especially for code paths used by many tools.

### Guidelines

- Avoid unnecessary allocations in hot paths.
- Prefer async I/O for file, database, and network operations.
- Use streaming when processing large files.
- Cache immutable metadata or parsed templates when appropriate.
- Avoid repeated reflection where cached descriptors can be used.
- Avoid premature optimization before behavior and design are clear.
- Measure performance before introducing complex optimizations.

Example:

```csharp
private readonly ConcurrentDictionary<string, CompiledTemplate> _templateCache = new();
```

Caching should be bounded or invalidated when data may change.

## 16. Security Guidelines

MCPTools may interact with source code, file systems, databases, templates, and external services. Security must be considered by default.

### Input Validation

Validate all external input, including:

- MCP request payloads.
- File paths.
- Template names.
- Database identifiers.
- Project paths.
- Command or process arguments.

### Never Trust External Data

Data from MCP clients, files, templates, environment variables, and external services must be treated as untrusted until validated.

### Secure File Handling

File system tools must:

- Normalize paths.
- Prevent path traversal.
- Respect workspace boundaries.
- Avoid destructive writes unless explicitly requested.
- Prefer safe abstractions for testability and policy enforcement.

### Secrets

Never store secrets in source code, samples, tests, or documentation.

Use:

- Environment variables.
- User secrets for local development.
- Secret stores in production environments.

### Safe Logging

Logs must not expose secrets, credentials, access tokens, private user data, or sensitive source code.

## 17. Code Review Checklist

Use this checklist when reviewing pull requests.

### Design

- [ ] The change follows Clean Architecture boundaries.
- [ ] The change follows SOLID principles.
- [ ] New abstractions are justified by real extension or testing needs.
- [ ] No AI-client-specific dependency was added to the core framework.
- [ ] Responsibilities are separated across tools, services, utilities, and models.

### Code Quality

- [ ] Names follow `NamingConventions.md`.
- [ ] Public types are in matching files.
- [ ] Methods are focused and readable.
- [ ] No God Objects, deep inheritance, or unnecessary static state were introduced.
- [ ] Magic strings and magic numbers are avoided or centralized.

### Dependency Injection

- [ ] Required dependencies use constructor injection.
- [ ] Service lifetimes are appropriate.
- [ ] Service locator usage is avoided.
- [ ] Registration APIs are consistent with framework conventions.

### Error Handling and Logging

- [ ] Known failures use framework-specific exceptions or result models.
- [ ] Exceptions are not swallowed.
- [ ] Stack traces are preserved.
- [ ] Structured logging is used.
- [ ] Sensitive data is not logged.

### Async and Configuration

- [ ] Async methods are used for I/O-bound operations.
- [ ] `CancellationToken` is accepted and passed through where appropriate.
- [ ] No blocking calls on async work were introduced.
- [ ] Configuration uses strongly typed options.
- [ ] Hardcoded environment-specific values are avoided.

### Testing and Documentation

- [ ] Unit tests cover success, validation failure, and relevant error paths.
- [ ] Test names are descriptive.
- [ ] Public APIs include useful XML comments.
- [ ] Relevant Markdown documentation was updated.
- [ ] Examples or samples were updated when public behavior changed.

## 18. Common Anti-Patterns

| Anti-Pattern | Problem |
| --- | --- |
| God classes | Concentrate too many responsibilities and become hard to change safely. |
| Long methods | Hide multiple operations and make testing difficult. |
| Magic strings | Create duplication and fragile behavior. |
| Magic numbers | Hide intent and make tuning difficult. |
| Duplicate code | Increases maintenance cost and bug risk. |
| Tight coupling | Makes components difficult to test, replace, and reuse. |
| Deep inheritance | Makes behavior hard to reason about and extend. |
| Service locator | Hides dependencies and weakens compile-time safety. |
| Static mutable state | Creates concurrency and test isolation problems. |
| Client-specific core code | Breaks MCPTools platform independence. |
| Swallowed exceptions | Makes failures invisible and diagnostics unreliable. |
| Unstructured responses | Makes tool results difficult for MCP clients and tests to consume. |

## 19. Summary

MCPTools code should be clear, maintainable, testable, secure, and platform-neutral.

The coding philosophy is:

- Prefer clarity over cleverness.
- Keep classes and methods focused.
- Use modern C# features when they improve the design.
- Make dependencies explicit.
- Validate external input.
- Log with structure and care.
- Test behavior that matters.
- Keep the core framework independent from any specific AI platform.

These standards ensure that MCPTools can grow as a professional open-source .NET framework while remaining approachable for contributors and dependable for enterprise use.
