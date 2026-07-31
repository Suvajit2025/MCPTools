# CLAUDE.md

## Purpose

This file provides repository-specific instructions for **Claude Code** when working in the MCPTools project.

Claude Code should use this document together with `AGENTS.md` and the repository documentation before making code, architecture, testing, or documentation changes.

## Project Summary

**MCPTools** is a reusable .NET 10 framework for building Model Context Protocol (MCP) tools and developer automation utilities.

The framework is provider-agnostic and must never become tied to a specific AI platform, AI client, model provider, or MCP server implementation.

MCPTools follows:

- Clean Architecture.
- SOLID principles.
- Dependency injection.
- Template-based code generation.
- Tool-oriented design.

The framework should remain modular, testable, extensible, and suitable for enterprise-grade open-source development.

## Repository Structure

The intended repository structure is:

```text
src/
docs/
samples/
```

| Folder | Purpose |
| --- | --- |
| `src/` | Framework source code, console hosts, optional packages, and test projects. |
| `docs/` | Architecture, standards, lifecycle, integration, roadmap, and design decision documentation. |
| `samples/` | Example tools, sample hosts, and reference implementations. |

Early repository versions may keep some projects and Markdown files at the repository root. Preserve the existing layout unless a structural change is explicitly requested and documented.

## Development Principles

Claude Code must:

- Follow the architecture described in `Architecture.md`.
- Follow the coding rules in `CodingStandards.md`.
- Follow the naming rules in `NamingConventions.md`.
- Follow the lifecycle rules in `ToolLifecycle.md`.
- Follow integration guidance in `MCPIntegration.md`.
- Follow template guidance in `TemplateEngine.md`.
- Respect accepted ADRs in `DesignDecisions.md`.
- Implement every executable feature as a `Tool` when appropriate.
- Prefer composition over inheritance.
- Avoid unnecessary dependencies.
- Keep classes small and focused.
- Use constructor injection for required dependencies.
- Keep business logic out of templates.
- Keep MCPTools independent from specific AI providers and MCP clients.
- Avoid breaking architectural changes unless `DesignDecisions.md` is updated with a new or revised ADR.

## Code Generation Rules

When generating or modifying code, Claude Code should:

- Produce production-quality C#.
- Follow .NET 10 and modern C# best practices.
- Use nullable reference types correctly.
- Prefer async APIs for I/O-bound or long-running work.
- Accept and pass `CancellationToken` where appropriate.
- Use XML documentation for public APIs.
- Avoid placeholder implementations unless explicitly requested.
- Add meaningful exception handling.
- Use structured logging through `ILogger<T>`.
- Use strongly typed request, response, options, and result models.
- Keep code readable, maintainable, and testable.
- Avoid raw JSON or primitive parameter lists in internal tool contracts.

## Documentation Rules

Whenever architecture or public behavior changes, update documentation as needed.

Documentation expectations:

- Update `Architecture.md` when design, layering, dependencies, or project structure change.
- Update `Roadmap.md` when planned features, milestones, or release scope change.
- Update `DesignDecisions.md` when an architectural decision changes.
- Update `ToolLifecycle.md` when tool execution behavior changes.
- Update `TemplateEngine.md` when template behavior changes.
- Update `MCPIntegration.md` when MCP-facing integration behavior changes.
- Update `README.md` when setup, usage, or public examples change.

Documentation should use professional GitHub Markdown and remain concise, accurate, and current.

## Testing Rules

When adding or changing features, Claude Code should:

- Add unit tests for new behavior.
- Do not reduce existing test coverage.
- Keep code independently testable.
- Follow the Arrange-Act-Assert pattern.
- Use clear test names.
- Mock or fake external dependencies.
- Keep tests deterministic.
- Avoid tests that require live AI clients, production databases, or external infrastructure unless explicitly requested.

Recommended test naming pattern:

```text
MethodName_ExpectedBehavior_WhenCondition
```

## Pull Request Expectations

Before completing work, Claude Code should:

- Ensure the solution builds when feasible.
- Run relevant tests when feasible.
- Keep formatting consistent with the repository.
- Avoid unrelated changes.
- Avoid broad refactoring unless requested.
- Update documentation when required.
- Preserve existing public APIs unless a breaking change is intentional and documented.
- Summarize significant design decisions and verification performed.

## General Behavior

When requirements are unclear, Claude Code should:

- Ask clarifying questions.
- Explain architectural trade-offs.
- Prefer maintainability over cleverness.
- Avoid over-engineering.
- Respect existing architecture and documentation.
- Make the smallest responsible change that satisfies the requirement.

Claude Code should behave like a careful open-source maintainer: preserve the framework vision, keep changes reviewable, and leave the repository easier to understand than it was found.

## Security Rules

Claude Code must treat all tools as capability-restricted.

Never assume permission to execute operations outside the exposed tool interface.

Never bypass validation implemented by tools.

Database tools must follow the principle of least privilege.

Unless explicitly designed otherwise:

- Never generate tools that execute arbitrary SQL.
- Never execute INSERT, UPDATE, DELETE, DROP, ALTER, TRUNCATE, or EXEC statements.
- Prefer metadata inspection over command execution.
- Treat all user input as untrusted.
- Use parameterized queries.
- Never expose secrets, passwords, or connection strings in logs or generated code.

## AI Independence

MCPTools is AI-provider agnostic.

Claude Code must never:

- Assume Claude is the only client.
- Assume ChatGPT is the only client.
- Assume Gemini is the only client.
- Assume a specific MCP server implementation.

Every tool should be callable by any compliant MCP client.

## Tool Design Principles

Each Tool should have a single responsibility.

A Tool should:

- Accept strongly typed request models.
- Return strongly typed response models.
- Be deterministic whenever possible.
- Avoid hidden side effects.
- Avoid modifying external systems unless explicitly designed for that purpose.
- Prefer analysis over execution.

## Source Code Analysis

When analyzing C# projects:

- Prefer Roslyn over string parsing.
- Never modify source code by regular expressions.
- Use syntax trees and semantic models.
- Preserve formatting whenever possible.
- Preserve comments and XML documentation.

## Performance

Avoid unnecessary file scans.

Reuse parsed models when possible.

Avoid loading entire solutions repeatedly.

Prefer asynchronous streaming APIs for large repositories.

Cache metadata when appropriate.

## Compatibility

Public APIs should remain backward compatible.

Breaking changes require:

- Documentation update
- Design decision update
- Version increment
- Migration guidance

## Extensibility

Prefer extending the framework through:

- New Tools
- New Providers
- New Templates
- New Parsers

Avoid modifying existing components when extension points are available.

Follow the Open/Closed Principle.

## Non-Goals

MCPTools is not:

- An AI agent.
- A chatbot.
- A language model.
- A replacement for Visual Studio.
- A database administration tool.

MCPTools provides reusable capabilities that AI clients can invoke through tools.

## Versioning

Follow Semantic Versioning.

Patch:
- Bug fixes

Minor:
- New tools
- New providers

Major:
- Breaking changes

## Long-Term Vision

MCPTools aims to become a provider-agnostic Software Engineering Platform capable of:

- Code Generation
- Database Understanding
- Solution Intelligence
- Code Transformation
- Project Analysis
- Architecture Analysis
- Documentation Generation
- AI Tool Integration

without becoming coupled to any specific programming domain or AI ecosystem.

