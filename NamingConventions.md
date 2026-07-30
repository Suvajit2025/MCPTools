# MCPTools Naming Conventions

## 1. Purpose

This document defines the official naming conventions for **MCPTools**, an enterprise-grade open-source .NET 10 framework for building Model Context Protocol (MCP) tools and developer automation utilities.

Consistent naming is essential because it:

- Improves readability and discoverability.
- Reduces ambiguity across projects, folders, namespaces, and APIs.
- Makes the framework easier to maintain and extend.
- Supports predictable contribution patterns for open-source developers.
- Aligns MCPTools with Microsoft .NET Design Guidelines.
- Reinforces Clean Architecture, SOLID principles, and domain-driven naming where appropriate.

Names should communicate intent clearly. A well-named type, member, folder, or file should help a developer understand its responsibility before reading its implementation.

## 2. General Naming Rules

### Casing Rules

| Item | Convention | Example |
| --- | --- | --- |
| Classes | PascalCase | `GenerateCrudTool` |
| Records | PascalCase | `GenerateCrudRequest` |
| Interfaces | PascalCase with `I` prefix | `ITemplateEngine` |
| Methods | PascalCase | `ExecuteAsync` |
| Properties | PascalCase | `OutputPath` |
| Public fields | PascalCase | `DefaultTimeout` |
| Constants | PascalCase | `MaxTemplateSize` |
| Parameters | camelCase | `cancellationToken` |
| Local variables | camelCase | `templatePath` |
| Private fields | camelCase with `_` prefix | `_templateRenderer` |
| Namespaces | PascalCase | `MCPTools.Core.Tools` |
| Folders | PascalCase | `Services` |
| Markdown files | PascalCase | `Architecture.md` |
| Template files | kebab-case or lowercase descriptive names | `controller.sbn` |

### PascalCase

Use PascalCase for type names, namespaces, methods, properties, constants, folders, projects, and documentation files.

Good:

```csharp
public sealed class ProjectAnalyzer
{
    public ProjectAnalysisResult AnalyzeProject(string projectPath)
    {
        // ...
    }
}
```

Avoid:

```csharp
public sealed class project_analyzer
{
    public ProjectAnalysisResult analyze_project(string project_path)
    {
        // ...
    }
}
```

### camelCase

Use camelCase for parameters and local variables.

Good:

```csharp
public Task RenderAsync(string templateName, object model, CancellationToken cancellationToken)
{
    var outputPath = GetOutputPath(templateName);
}
```

Avoid:

```csharp
public Task RenderAsync(string TemplateName, object Model, CancellationToken CancellationToken)
{
    var OutputPath = GetOutputPath(TemplateName);
}
```

### Avoid Abbreviations

Use full words unless an abbreviation is widely accepted in the .NET ecosystem.

| Prefer | Avoid |
| --- | --- |
| `Configuration` | `Config` |
| `GenerateCrudRequest` | `GenCrudReq` |
| `ProjectAnalysisResult` | `ProjAnalysisRes` |
| `DatabaseConnection` | `DbConn` |
| `CancellationToken` | `CancelToken` |

Accepted common abbreviations:

- `MCP`
- `CLI`
- `API`
- `HTTP`
- `JSON`
- `XML`
- `SQL`
- `ID`
- `URI`
- `URL`

### Avoid Hungarian Notation

Do not prefix names with type information.

| Prefer | Avoid |
| --- | --- |
| `name` | `strName` |
| `count` | `intCount` |
| `isEnabled` | `boolIsEnabled` |
| `tools` | `listTools` |

### Use Meaningful Names

Names should describe purpose, not implementation details.

| Prefer | Avoid |
| --- | --- |
| `TemplateRenderer` | `TemplateHelper` |
| `ProjectAnalyzer` | `ProjectUtil` |
| `ToolExecutionContext` | `DataObject` |
| `GeneratedFile` | `Item` |
| `ValidationResult` | `ResultData` |

### Singular vs Plural

Use singular names for one concept and plural names for collections or folders that contain multiple items.

| Use | For | Example |
| --- | --- | --- |
| Singular | One type or object | `ToolMetadata` |
| Plural | Collection property | `GeneratedFiles` |
| Plural | Folder containing many related types | `Services` |
| Singular | Folder representing one concept or bounded capability | `Configuration` |

Good:

```csharp
public IReadOnlyList<GeneratedFile> GeneratedFiles { get; init; } = [];
public ToolMetadata Metadata { get; init; } = new();
```

## 3. Solution Naming

The solution file should use the framework name.

| Type | Name |
| --- | --- |
| Main solution | `MCPTools.sln` |
| Future solution filter | `MCPTools.slnf` |
| XML solution file format | `MCPTools.slnx` |

The solution name should remain stable and should not include environment, developer, branch, or feature names.

Avoid:

- `MCPTools.Dev.sln`
- `MCPTools_New.sln`
- `Suvajit.MCPTools.sln`
- `MCPTools.Final.sln`

## 4. Project Naming

Project names must begin with the `MCPTools` root namespace and use PascalCase segments separated by dots.

### Current Projects

| Project | Purpose |
| --- | --- |
| `MCPTools.Core` | Core framework abstractions, models, services, interfaces, and utilities. |
| `MCPTools.Console` | Console host for local execution, diagnostics, and examples. |
| `MCPTools.Tests` | Automated tests for framework behavior. |

### Future Projects

| Project | Purpose |
| --- | --- |
| `MCPTools.CLI` | Command-line tooling for scaffolding, running, and diagnosing tools. |
| `MCPTools.Templates` | Template engine and built-in templates. |
| `MCPTools.Roslyn` | Roslyn-based code analysis and generation features. |
| `MCPTools.Plugins` | Plugin discovery, registration, and loading infrastructure. |
| `MCPTools.McpServer` | Optional MCP server hosting integration. |
| `MCPTools.AI` | Provider-neutral AI utility abstractions. |
| `MCPTools.Cloud` | Optional cloud integration abstractions and providers. |
| `MCPTools.Git` | Git and repository automation tools. |

Avoid project names that imply a specific AI provider in the core framework.

Avoid:

- `MCPTools.ChatGPT`
- `MCPTools.Claude`
- `MCPTools.Copilot`

Provider-specific integrations, if ever required, must be optional packages and clearly isolated.

## 5. Folder Naming

Folders should use PascalCase and represent clear architectural or feature boundaries.

### Standard Folders

| Folder | Purpose | Rule |
| --- | --- | --- |
| `Tools` | Executable MCP tool implementations. | Plural because it contains multiple tools. |
| `Services` | Reusable application services. | Plural. |
| `Utilities` | Shared low-level helpers. | Plural. |
| `Templates` | Template files or template-related components. | Plural. |
| `Extensions` | Extension methods and registration helpers. | Plural. |
| `Configuration` | Options and configuration binding types. | Singular concept. |
| `Interfaces` | Interface contracts when not grouped by feature. | Plural. |
| `Models` | Request, response, result, and metadata models. | Plural. |
| `Exceptions` | Framework and feature exception types. | Plural. |
| `Constants` | Constant values grouped by domain. | Plural. |
| `Versioning` | Versioning metadata and compatibility rules. | Singular concept. |
| `Abstractions` | Core contracts and base abstractions. | Plural. |

### Feature Folder Examples

```text
MCPTools.Core/
|-- Tools/
|-- Services/
|-- Utilities/
|-- Templates/
|-- Extensions/
|-- Configuration/
|-- Interfaces/
|-- Models/
|-- Exceptions/
|-- Constants/
|-- Versioning/
|-- Abstractions/
```

Prefer folders based on responsibility. Avoid vague folders such as:

- `Helpers`
- `Common`
- `Stuff`
- `Misc`
- `Managers`

## 6. Namespace Naming

Namespaces should match the project and folder structure.

### Examples

| Namespace | Purpose |
| --- | --- |
| `MCPTools.Core` | Root namespace for the core framework. |
| `MCPTools.Core.Tools` | Tool implementations and tool-related types. |
| `MCPTools.Core.Services` | Reusable framework services. |
| `MCPTools.Core.Templates` | Template engine components. |
| `MCPTools.Core.Configuration` | Options and configuration binding. |
| `MCPTools.Core.Extensions` | Extension methods. |
| `MCPTools.Core.Models` | Shared models. |
| `MCPTools.Core.Exceptions` | Exception hierarchy. |
| `MCPTools.Roslyn.Analysis` | Future Roslyn analysis components. |
| `MCPTools.CLI.Commands` | Future CLI command implementations. |

Namespace segments should be stable, descriptive, and PascalCase.

Avoid:

```csharp
namespace MCPTools.Core.Common;
namespace MCPTools.Core.Helpers;
namespace MCPTools.Core.NewStuff;
```

## 7. Class Naming

Class names should be nouns or noun phrases that describe the type's responsibility.

### Common Suffixes

| Suffix | Use For | Example |
| --- | --- | --- |
| `Tool` | Executable MCP capability. | `GenerateCrudTool` |
| `Service` | Reusable application service. | `TemplateService` |
| `Renderer` | Type that renders output. | `TemplateRenderer` |
| `Analyzer` | Type that analyzes input. | `ProjectAnalyzer` |
| `Provider` | Type that supplies data or external resources. | `ConfigurationProvider` |
| `Factory` | Type that creates objects. | `ToolFactory` |
| `Registry` | Type that stores and resolves registered items. | `ToolRegistry` |
| `Builder` | Type that incrementally constructs an object. | `ToolDescriptorBuilder` |
| `Validator` | Type that validates input or state. | `ToolRequestValidator` |
| `Context` | Type that carries execution state. | `ToolExecutionContext` |
| `Descriptor` | Type that describes metadata. | `ToolDescriptor` |
| `Options` | Type bound to configuration. | `TemplateOptions` |

### Examples

Good:

```csharp
public sealed class GenerateCrudTool
public sealed class TemplateRenderer
public sealed class ProjectAnalyzer
public sealed class ConfigurationProvider
```

Avoid:

```csharp
public sealed class GenerateCrud
public sealed class TemplateHelper
public sealed class ProjectManager
public sealed class ConfigProvider
```

## 8. Interface Naming

Interfaces must use the `I` prefix followed by a PascalCase name.

Examples:

```csharp
public interface ITool
public interface ITemplateEngine
public interface ILoggerService
public interface IProjectAnalyzer
```

The `I` prefix is the standard .NET convention and makes abstractions immediately recognizable.

### Interface Naming Guidelines

| Prefer | Avoid |
| --- | --- |
| `ITemplateEngine` | `TemplateEngineInterface` |
| `IProjectAnalyzer` | `ProjectAnalyzerContract` |
| `IToolRegistry` | `ToolRegistryBase` |
| `IFileSystemService` | `IFileHelper` |

Interfaces should describe capability, not implementation.

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

## 9. Record Naming

Records should use PascalCase and should describe immutable data, messages, metadata, or result shapes.

Examples:

```csharp
public sealed record CrudRequest;
public sealed record CrudResponse;
public sealed record ProjectAnalysisResult;
public sealed record ToolMetadata;
```

### Recommended Record Patterns

| Pattern | Example | Purpose |
| --- | --- | --- |
| `{Action}Request` | `GenerateCrudRequest` | Input for a tool operation. |
| `{Action}Response` | `GenerateCrudResponse` | Output from a tool operation. |
| `{Subject}Result` | `ProjectAnalysisResult` | Result of a service or analysis operation. |
| `{Subject}Metadata` | `ToolMetadata` | Descriptive metadata. |
| `{Subject}Descriptor` | `ToolDescriptor` | Structured description used for registration or discovery. |

Prefer records for immutable data transfer objects where value semantics are useful.

## 10. Enum Naming

Enum type names should be singular PascalCase nouns. Enum member names should also use PascalCase.

Examples:

```csharp
public enum ToolCategory
{
    CodeGeneration,
    Database,
    FileSystem,
    ProjectAnalysis,
    Documentation,
    Template,
    AIUtility
}

public enum ExecutionStatus
{
    Pending,
    Running,
    Succeeded,
    Failed,
    Cancelled
}

public enum TemplateType
{
    Entity,
    Controller,
    Repository,
    Service
}
```

### Enum Guidelines

- Use singular names for normal enums.
- Use plural names only for flags enums.
- Apply `[Flags]` only when values are intended to be combined.
- Avoid prefixes that repeat the enum type name.

Avoid:

```csharp
public enum ToolCategory
{
    ToolCategoryCodeGeneration,
    ToolCategoryDatabase
}
```

## 11. Exception Naming

Exception types must use PascalCase and end with `Exception`.

Examples:

```csharp
public class ToolException : Exception
public class ValidationException : ToolException
public class ConfigurationException : ToolException
public class TemplateException : ToolException
```

### Exception Hierarchy

Framework exceptions should derive from a common MCPTools base exception.

```csharp
public class McpToolsException : Exception
public class ToolException : McpToolsException
public class ToolValidationException : ToolException
public class ToolExecutionException : ToolException
public class TemplateException : McpToolsException
public class ConfigurationException : McpToolsException
```

### Exception Naming Guidelines

| Exception | Use For |
| --- | --- |
| `McpToolsException` | Base exception for framework-specific failures. |
| `ToolException` | Base exception for tool-related failures. |
| `ToolValidationException` | Invalid tool request or invalid preconditions. |
| `ToolExecutionException` | Failure while executing a valid tool request. |
| `TemplateException` | Template loading, parsing, or rendering failures. |
| `ConfigurationException` | Invalid or missing framework configuration. |

Avoid generic or unclear exception names:

- `CustomException`
- `GeneralException`
- `ErrorException`
- `BadRequestException` outside transport-specific layers.

## 12. Service Naming

Services should use clear nouns or noun phrases and should usually end with `Service` when they represent reusable application behavior.

Examples:

```csharp
public sealed class TemplateService
public sealed class FileService
public sealed class ConfigurationService
```

Some services may use a more precise suffix when it better describes the responsibility:

```csharp
public sealed class ToolRegistry
public sealed class TemplateRenderer
public sealed class ProjectAnalyzer
public sealed class ToolFactory
```

### Service Naming Guidelines

| Responsibility | Preferred Name |
| --- | --- |
| Template orchestration | `TemplateService` |
| Template rendering | `TemplateRenderer` |
| File access abstraction | `FileService` or `FileSystemService` |
| Tool registration and lookup | `ToolRegistry` |
| Configuration coordination | `ConfigurationService` |
| Project structure analysis | `ProjectAnalyzer` |

Avoid vague names:

- `ToolManager`
- `DataService`
- `HelperService`
- `CommonService`

## 13. Tool Naming

Every executable MCP capability must end with the `Tool` suffix.

Tool names should be action-oriented and should describe the operation clearly.

### Tool Naming Pattern

```text
{Verb}{Subject}Tool
```

Examples:

```csharp
public sealed class GenerateCrudTool
public sealed class GenerateSqlTool
public sealed class AnalyzeSolutionTool
public sealed class SearchFilesTool
public sealed class ReviewCodeTool
```

### Recommended Verbs

| Verb | Use For | Example |
| --- | --- | --- |
| `Generate` | Creating new output. | `GenerateDocumentationTool` |
| `Analyze` | Inspecting and evaluating input. | `AnalyzeSolutionTool` |
| `Search` | Finding matching items. | `SearchFilesTool` |
| `Inspect` | Reading metadata or structure. | `InspectDatabaseSchemaTool` |
| `Render` | Producing output from a template. | `RenderTemplateTool` |
| `Validate` | Checking correctness. | `ValidateProjectTool` |
| `Review` | Evaluating quality or risk. | `ReviewCodeTool` |

Avoid tool names that are vague or noun-only:

- `CrudTool`
- `SqlTool`
- `ProjectTool`
- `FilesTool`

## 14. Request and Response Naming

Every tool should have matching request and response types.

### Naming Pattern

```text
{ToolNameWithoutTool}Request
{ToolNameWithoutTool}Response
```

Examples:

| Tool | Request | Response |
| --- | --- | --- |
| `GenerateCrudTool` | `GenerateCrudRequest` | `GenerateCrudResponse` |
| `AnalyzeSolutionTool` | `AnalyzeSolutionRequest` | `AnalyzeSolutionResponse` |
| `SearchFilesTool` | `SearchFilesRequest` | `SearchFilesResponse` |
| `RenderTemplateTool` | `RenderTemplateRequest` | `RenderTemplateResponse` |
| `InspectDatabaseSchemaTool` | `InspectDatabaseSchemaRequest` | `InspectDatabaseSchemaResponse` |

Request and response types should be explicit, serializable, and stable.

Avoid:

- `GenerateCrudInput`
- `GenerateCrudOutput`
- `CrudParams`
- `CrudResultDto`

Use `Result` for service-level outcomes and `Response` for tool-level outputs.

## 15. Configuration Naming

Configuration types should end with `Options`.

Examples:

```csharp
public sealed class ToolOptions
public sealed class LoggingOptions
public sealed class TemplateOptions
public sealed class McpToolsOptions
public sealed class RoslynOptions
```

### Configuration Naming Guidelines

| Configuration Area | Type Name | Section Name |
| --- | --- | --- |
| Framework root | `McpToolsOptions` | `MCPTools` |
| Tool behavior | `ToolOptions` | `MCPTools:Tools` |
| Logging behavior | `LoggingOptions` | `MCPTools:Logging` |
| Template behavior | `TemplateOptions` | `MCPTools:Templates` |
| Roslyn behavior | `RoslynOptions` | `MCPTools:Roslyn` |

Configuration section names should be clear and should match the domain being configured.

## 16. Constant Naming

Constants should use PascalCase and should describe the value clearly.

Examples:

```csharp
public const int DefaultTimeout = 30;
public const int MaxTemplateSize = 1024 * 1024;
public static readonly string[] SupportedExtensions = [".cs", ".csproj", ".sln"];
```

### Constant Guidelines

- Use PascalCase for public constants.
- Use meaningful names that describe intent.
- Group related constants in focused static classes.
- Avoid magic numbers and repeated string literals.

Example:

```csharp
public static class TemplateConstants
{
    public const int MaxTemplateSize = 1024 * 1024;
    public const string DefaultTemplateExtension = ".sbn";
}
```

Avoid:

```csharp
public const int X = 30;
public const string Str = ".sbn";
```

## 17. Template Naming

Template files should use lowercase descriptive names. The recommended extension for Scriban templates is `.sbn`.

Examples:

```text
controller.sbn
repository.sbn
entity.sbn
service.sbn
crud-controller.sbn
crud-repository.sbn
readme.sbn
architecture-section.sbn
```

### Template Naming Strategy

| Template Type | Recommended Name |
| --- | --- |
| Entity class | `entity.sbn` |
| Controller class | `controller.sbn` |
| Repository class | `repository.sbn` |
| Service class | `service.sbn` |
| CRUD controller | `crud-controller.sbn` |
| CRUD service | `crud-service.sbn` |
| README output | `readme.sbn` |
| Architecture section | `architecture-section.sbn` |

Template names should:

- Be lowercase.
- Use kebab-case for multi-word names.
- Describe the generated artifact.
- Avoid spaces.
- Avoid environment-specific names.

Avoid:

- `ControllerTemplate.sbn`
- `Repository_New.sbn`
- `My Template.sbn`
- `FinalServiceTemplate.sbn`

## 18. Markdown Documentation Naming

Markdown documentation files should use PascalCase.

Examples:

```text
Architecture.md
Vision.md
Roadmap.md
ToolLifecycle.md
NamingConventions.md
CodingStandards.md
DesignDecisions.md
MCPIntegration.md
TemplateEngine.md
```

### Documentation Naming Guidelines

| Document Type | Pattern | Example |
| --- | --- | --- |
| Architecture guide | `{Subject}.md` | `Architecture.md` |
| Multi-word guide | PascalCase | `ToolLifecycle.md` |
| Standards document | `{Subject}Standards.md` | `CodingStandards.md` |
| Decision record index | PascalCase | `DesignDecisions.md` |

Avoid:

- `architecture.md`
- `tool-lifecycle.md`
- `naming_conventions.md`
- `README-final.md`

The only common exception is `README.md`, which should remain uppercase by convention.

## 19. Future Naming Rules

Future modules should continue to use the `MCPTools.{Module}` naming pattern.

### Future Module Names

| Area | Project Name | Namespace Example | Notes |
| --- | --- | --- | --- |
| Roslyn | `MCPTools.Roslyn` | `MCPTools.Roslyn.Analysis` | Source analysis and code generation. |
| CLI | `MCPTools.CLI` | `MCPTools.CLI.Commands` | Command-line host and commands. |
| Plugins | `MCPTools.Plugins` | `MCPTools.Plugins.Discovery` | Plugin discovery and registration. |
| AI | `MCPTools.AI` | `MCPTools.AI.Abstractions` | Provider-neutral AI utilities only. |
| Cloud | `MCPTools.Cloud` | `MCPTools.Cloud.Storage` | Optional cloud integration abstractions. |
| Git | `MCPTools.Git` | `MCPTools.Git.Tools` | Repository automation. |
| Database | `MCPTools.Database` | `MCPTools.Database.Schema` | Database inspection and utilities. |
| Documentation | `MCPTools.Documentation` | `MCPTools.Documentation.Tools` | Documentation generation tools. |

### Future Tool Examples

```csharp
public sealed class AnalyzeSyntaxTreeTool
public sealed class GenerateCommandTool
public sealed class DiscoverPluginsTool
public sealed class SummarizePromptTool
public sealed class UploadArtifactTool
public sealed class InspectGitStatusTool
```

### Future Request and Response Examples

```csharp
public sealed record AnalyzeSyntaxTreeRequest;
public sealed record AnalyzeSyntaxTreeResponse;

public sealed record DiscoverPluginsRequest;
public sealed record DiscoverPluginsResponse;

public sealed record InspectGitStatusRequest;
public sealed record InspectGitStatusResponse;
```

Provider-specific future integrations should include the provider name only in optional integration packages.

Examples:

```text
MCPTools.Integrations.OpenAI
MCPTools.Integrations.AzureOpenAI
MCPTools.Integrations.GitHub
```

Provider-specific names must not appear in `MCPTools.Core` unless they describe an open standard or protocol.

## 20. Summary

MCPTools naming should be clear, consistent, predictable, and aligned with Microsoft .NET Design Guidelines.

The naming philosophy is:

- Use names that communicate intent.
- Prefer clarity over brevity.
- Keep framework names platform-neutral.
- Use `Tool` for executable MCP capabilities.
- Use `Request` and `Response` for tool contracts.
- Use `Options` for configuration.
- Use `Exception` for framework failure types.
- Keep namespaces and folders aligned with architecture.

Consistent naming gives MCPTools a professional foundation and helps contributors build new capabilities that feel like part of one coherent framework.
