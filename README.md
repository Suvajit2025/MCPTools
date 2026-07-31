# MCPTools

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![C%23](https://img.shields.io/badge/C%23-14-239120)](https://learn.microsoft.com/dotnet/csharp/)
[![Build](https://img.shields.io/badge/build-pending-lightgrey)](#)
[![License](https://img.shields.io/badge/license-TBD-lightgrey)](#license)

**MCPTools** is a reusable .NET 10 framework for building developer automation tools and Model Context Protocol (MCP) tools.

It helps developers create reusable tooling infrastructure instead of repeatedly rebuilding the same patterns for code generation, project analysis, file utilities, database helpers, template rendering, and future MCP integrations.

MCPTools is **provider-agnostic**. It is not tied to Claude, ChatGPT, GitHub Copilot, Cursor, Visual Studio, or any specific AI platform. The framework is designed to stay clean, modular, testable, and reusable across MCP-compatible environments.

## Why MCPTools?

Modern developer automation often starts as one-off scripts, hardcoded generators, or client-specific AI integrations. Those approaches work at first, but they become difficult to reuse, test, version, and maintain.

MCPTools exists to provide a cleaner foundation:

- Build tools once and reuse them across hosts.
- Keep business logic separate from MCP protocol concerns.
- Use strongly typed request and response models.
- Register tools through dependency injection.
- Generate output through templates instead of hardcoded strings.
- Keep the framework independent from any specific AI provider.

## Features

### Current Focus

MCPTools is currently under active development. The first phase focuses on the core framework before shipping production-ready developer tools.

Planned foundation features:

- Tool framework.
- Tool registry.
- Tool execution pipeline.
- Dependency injection support.
- Centralized logging.
- Strongly typed configuration.
- Template engine abstractions.

### Planned Tools

- CRUD Generator.
- SQL Generator.
- Repository Generator.
- Controller Generator.
- Service Generator.
- DTO Generator.
- Project Analyzer.
- File Utilities.
- Database Utilities.
- Future MCP integration support.

## Project Structure

Recommended solution layout:

```text
MCPTools.sln

src/
    MCPTools.Core
    MCPTools.Console
    MCPTools.Tests

docs/
samples/
```

| Path | Purpose |
| --- | --- |
| `src/MCPTools.Core` | Core framework abstractions, models, services, tools, configuration, logging, and utilities. |
| `src/MCPTools.Console` | Console host for local experimentation, diagnostics, and examples. |
| `src/MCPTools.Tests` | xUnit test project for framework behavior. |
| `docs/` | Architecture, lifecycle, standards, roadmap, and integration documentation. |
| `samples/` | Future sample tools and reference implementations. |

Early development versions may keep some projects and documents at the repository root while the framework structure is being finalized.

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Suvajit2025/MCPTools.git
cd MCPTools
```

### 2. Open the Solution

Open `MCPTools.sln` or `MCPTools.slnx` in Visual Studio 2026.

### 3. Restore Packages

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run Tests

```bash
dotnet test
```

### 6. Build the MCP Server

`MCPTools.Server` exposes registered MCPTools tools through the official .NET MCP SDK using the MCP stdio transport. MCP-compatible clients can launch this process and communicate using JSON-RPC over standard input and standard output.

```bash
dotnet build src/MCPTools.Server/MCPTools.Server.csproj
```

For MCP clients such as Codex in VS Code, launch the compiled server DLL directly instead of using `dotnet run`. This avoids build and restore output interfering with the stdio JSON-RPC protocol.

```json
{
  "servers": {
    "mcptools": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "${workspaceFolder}/src/MCPTools.Server/bin/Debug/net10.0/MCPTools.Server.dll"
      ],
      "cwd": "${workspaceFolder}/src/MCPTools.Server"
    }
  }
}
```

## Documentation

Start here:

- [Vision.md](Vision.md) - Project vision and long-term direction.
- [Architecture.md](Architecture.md) - Framework architecture and design boundaries.
- [ToolLifecycle.md](ToolLifecycle.md) - Standard lifecycle for all tools.
- [NamingConventions.md](NamingConventions.md) - Naming standards for projects, folders, types, and tools.
- [CodingStandards.md](CodingStandards.md) - C# and .NET coding standards.
- [TemplateEngine.md](TemplateEngine.md) - Template engine design specification.
- [MCPIntegration.md](MCPIntegration.md) - MCP ecosystem integration guidance.
- [DesignDecisions.md](DesignDecisions.md) - Architecture Decision Record log.
- [Roadmap.md](Roadmap.md) - Planned releases and future direction.

## Development Principles

MCPTools is built around a few simple principles:

- **Clean Architecture:** keep framework logic separate from hosting, protocol, and provider-specific concerns.
- **SOLID:** design small, focused, replaceable components.
- **Dependency Injection:** make dependencies explicit and testable.
- **Tool-Oriented Design:** executable capabilities should be modeled as tools.
- **Reusable Components:** prefer shared services, abstractions, and templates over duplicated logic.

## Roadmap

The initial roadmap focuses on:

1. Framework foundation.
2. Tool registry and execution pipeline.
3. Template engine.
4. Code generation tools.
5. Database and file utilities.
6. CLI and developer experience.
7. Future MCP server integration support.
8. Roslyn-powered analysis and generation.
9. Provider-neutral AI-assisted development utilities.

See [Roadmap.md](Roadmap.md) for the full roadmap.

## Contributing

Contributions are welcome. During the early development phase, the project is focused on establishing the first stable framework foundation.

Before contributing, please review:

- [AGENTS.md](AGENTS.md)
- [Architecture.md](Architecture.md)
- [CodingStandards.md](CodingStandards.md)
- [NamingConventions.md](NamingConventions.md)
- [DesignDecisions.md](DesignDecisions.md)

Larger contributions will be easier to accept after the first stable version defines the public contracts and extension model.

## License

License information will be added before the first stable release.
