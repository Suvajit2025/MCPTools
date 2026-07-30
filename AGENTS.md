# AGENTS.md

## Purpose

This document provides general guidance for AI coding assistants working on the **MCPTools** repository.

These instructions apply regardless of AI platform or coding assistant. They are intended for Claude Code, ChatGPT, GitHub Copilot, Cursor, local agents, custom automation, and any future assistant that contributes to this repository.

AI assistants should use this document as the default repository guidance before making code, architecture, testing, or documentation changes.

## Project Overview

**MCPTools** is a reusable .NET 10 framework for building Model Context Protocol (MCP) tools and developer automation utilities.

The framework is **AI-provider independent**. It must not depend on a specific AI client, model provider, MCP server implementation, or assistant platform.

MCPTools is designed to provide reusable infrastructure for:

- Code generation.
- Database tools.
- File system tools.
- Project analysis.
- Documentation generation.
- Template engine workflows.
- AI utilities.

AI assistants must preserve this design philosophy when proposing or implementing changes.

## Repository Goals

Contributors should:

- Maintain Clean Architecture boundaries.
- Write reusable code.
- Keep the framework modular.
- Follow SOLID principles.
- Preserve backward compatibility where practical.
- Write maintainable, readable, and testable code.
- Keep framework code independent from specific AI providers and MCP clients.
- Prefer stable abstractions over one-off implementations.

## Architecture Rules

Before making structural or architectural changes, AI assistants must review the relevant project documentation.

Required references:

| Document | When to Review |
| --- | --- |
| `Architecture.md` | Before changing project structure, dependencies, layers, or major framework components. |
| `ToolLifecycle.md` | Before creating or modifying executable tools. |
| `NamingConventions.md` | Before adding projects, folders, namespaces, types, members, templates, or documentation files. |
| `CodingStandards.md` | Before implementing or refactoring code. |
| `DesignDecisions.md` | Before making architectural choices or changing established decisions. |
| `MCPIntegration.md` | Before changing MCP-facing integration behavior. |
| `TemplateEngine.md` | Before changing template loading, rendering, validation, caching, or template structure. |
| `Roadmap.md` | Before changing planned scope or release direction. |

Architecture changes must respect existing ADRs unless a new ADR is added to supersede the old decision.

## Implementation Rules

Every new feature should:

- Be implemented as a `Tool` when it represents an executable capability.
- Use clear request and response models.
- Be registered through dependency injection.
- Be independently testable.
- Use abstractions for external dependencies.
- Avoid direct dependencies on AI provider SDKs in core framework code.
- Include documentation updates when architecture, public APIs, or user-facing behavior changes.

Tools should follow this pattern:

```text
{Verb}{Subject}Tool
{Verb}{Subject}Request
{Verb}{Subject}Response
```

Example:

```text
GenerateCrudTool
GenerateCrudRequest
GenerateCrudResponse
```

## Code Quality

Code changes should maintain a professional framework standard.

Required qualities:

- Small classes.
- Small methods.
- Strong typing.
- XML documentation for public APIs.
- Meaningful structured logging.
- Proper exception handling.
- Clear validation.
- No duplicated logic.
- No unnecessary static state.
- No hidden service location.
- No business logic embedded in host or protocol layers.

Prefer code that is obvious, boring, and reliable over code that is clever but difficult to maintain.

## Documentation Policy

Whenever new functionality is added, update documentation as needed.

Documentation expectations:

- Update `README.md` when setup, usage, examples, or public behavior changes.
- Update `Roadmap.md` when milestones, priorities, or release scope change.
- Update `Architecture.md` when design, layering, dependencies, or project structure change.
- Update `DesignDecisions.md` when architectural choices change or new long-term decisions are introduced.
- Update `ToolLifecycle.md` when tool execution behavior changes.
- Update `TemplateEngine.md` when template behavior changes.
- Update `MCPIntegration.md` when MCP-facing integration behavior changes.

Documentation should be written in professional GitHub Markdown and kept concise, accurate, and current.

## Testing Policy

New code should include appropriate automated tests.

Testing requirements:

- Add unit tests for new framework behavior.
- Use clear test names.
- Follow the Arrange-Act-Assert pattern.
- Mock or fake external dependencies.
- Keep tests deterministic.
- Cover validation, success paths, and failure paths.
- Avoid tests that require live AI clients or production infrastructure.

Recommended test naming pattern:

```text
MethodName_ExpectedBehavior_WhenCondition
```

Example:

```text
ExecuteAsync_ReturnsResponse_WhenRequestIsValid
ExecuteAsync_ThrowsToolValidationException_WhenEntityNameIsMissing
```

## Security Guidelines

AI assistants must never:

- Hardcode secrets.
- Log sensitive information.
- Trust external input.
- Bypass validation.
- Store credentials in source code, tests, samples, templates, or documentation.
- Introduce destructive file system or database operations without explicit safeguards.

Security-sensitive code must:

- Validate inputs.
- Normalize file paths.
- Respect workspace boundaries.
- Avoid logging raw request payloads.
- Use least-privilege assumptions.
- Keep authentication and authorization concerns host-controlled where appropriate.

## Preferred Development Workflow

Use the following workflow for repository changes:

1. Understand the requirement.
2. Review relevant documentation.
3. Design before implementation.
4. Implement the change.
5. Add or update tests.
6. Update documentation.
7. Verify the solution builds.

For significant changes, explain the design decision and identify any tradeoffs.

## General Behavior

AI assistants should:

- Explain significant design decisions.
- Avoid unnecessary complexity.
- Prefer readability over cleverness.
- Respect the existing architecture.
- Preserve provider independence.
- Keep changes focused and maintainable.
- Ask for clarification when requirements are ambiguous or risky.
- Avoid broad rewrites unless explicitly requested.
- Avoid changing public APIs without a clear reason.
- Avoid introducing new dependencies without architectural justification.

## Final Guidance

MCPTools should remain a clean, modular, provider-agnostic .NET framework for building reusable MCP tools.

Every contribution should make the framework easier to understand, easier to extend, easier to test, and safer to use across current and future MCP-compatible environments.
