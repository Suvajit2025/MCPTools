# MCPTools Design Decisions

This document is the official Architecture Decision Record (ADR) log for **MCPTools**, an enterprise-grade open-source .NET 10 framework for building Model Context Protocol (MCP) tools and developer automation utilities.

ADRs document significant architecture decisions, the reasons behind them, alternatives considered, and long-term consequences. This log should evolve as the framework grows.

## ADR Status Values

| Status | Meaning |
| --- | --- |
| Proposed | The decision is under discussion and has not yet been finalized. |
| Accepted | The decision is approved and should guide implementation. |
| Deprecated | The decision has been replaced or is no longer recommended. |

## ADR-001: Framework Independence

| Field | Value |
| --- | --- |
| ADR Number | ADR-001 |
| Title | Framework Independence |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCPTools is intended to support tools that can be used with Claude Code, ChatGPT, GitHub Copilot, Cursor, VS Code, custom MCP clients, and future MCP-compatible environments.

The MCP ecosystem is still evolving. If the framework depends directly on one AI provider, client SDK, or MCP server implementation, it will become harder to reuse, test, package, and maintain.

### Decision

MCPTools must remain independent of any AI provider and any specific MCP server implementation.

The core framework will provide reusable .NET abstractions for tools, services, metadata, registration, lifecycle, templates, logging, configuration, and execution. Provider-specific or server-specific integrations must be optional packages or host-level concerns.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Couple directly to one AI provider | Creates vendor lock-in and prevents use across other MCP-compatible clients. |
| Build only for one MCP server implementation | Limits adoption and makes the framework dependent on external server lifecycle decisions. |
| Place provider adapters in the core package | Forces all users to accept dependencies they may not need. |

### Consequences

- The core framework remains portable and reusable.
- MCPTools can be adopted by multiple hosts and clients.
- Optional integration packages may be needed for provider-specific features.
- More abstraction is required at integration boundaries.
- Documentation must clearly distinguish framework responsibilities from server and client responsibilities.

### Future Review

Review this decision if the MCP ecosystem standardizes around a required server SDK or if the framework introduces optional integration packages that risk leaking provider-specific concerns into the core.

## ADR-002: Tool-Oriented Architecture

| Field | Value |
| --- | --- |
| ADR Number | ADR-002 |
| Title | Tool-Oriented Architecture |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCP exposes executable capabilities as tools. MCPTools needs a consistent model for code generation, database inspection, file operations, project analysis, documentation generation, template rendering, and future automation features.

### Decision

Everything executable in MCPTools is implemented as a **Tool**.

A Tool is a focused executable capability with a request model, response model, metadata, validation behavior, lifecycle, logging, and error handling.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Use different models for generators, analyzers, and utilities | Creates inconsistent lifecycle and discovery behavior. |
| Expose services directly as MCP capabilities | Couples internal implementation details to external clients. |
| Treat tools as thin wrappers around static functions | Reduces testability, metadata quality, lifecycle control, and dependency injection support. |

### Consequences

- All executable capabilities follow the same lifecycle.
- Tool discovery and registration are consistent.
- Tools can be tested independently from MCP clients.
- Services remain reusable behind tools.
- Some simple operations may require small request and response models, but this improves consistency.

### Future Review

Review when introducing plugins, batched execution, or long-running workflows to ensure the Tool abstraction remains sufficient.

## ADR-003: Clean Architecture

| Field | Value |
| --- | --- |
| ADR Number | ADR-003 |
| Title | Clean Architecture |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCPTools must support long-term maintainability, provider independence, testability, and modular growth. The framework will include core abstractions, tools, services, templates, utilities, optional integrations, and future plugin packages.

### Decision

MCPTools will adopt Clean Architecture principles.

Core contracts and domain logic should remain independent from infrastructure, hosting, MCP server implementation details, AI clients, file system implementations, database providers, and optional integrations.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Layer all code by technical convenience only | Encourages coupling and unclear ownership boundaries. |
| Put protocol and tool logic in the same layer | Makes tools harder to test and reuse outside MCP. |
| Use a monolithic framework package for all features | Makes optional capabilities difficult to isolate and version. |

### Consequences

- Dependencies must point toward stable abstractions.
- Optional features should live in separate packages or modules.
- Tool logic is easier to test without infrastructure.
- Contributors must respect layer boundaries.
- More up-front design discipline is required.

### Future Review

Review when creating major optional packages such as `MCPTools.Roslyn`, `MCPTools.CLI`, `MCPTools.Plugins`, or `MCPTools.McpServer`.

## ADR-004: Dependency Injection

| Field | Value |
| --- | --- |
| ADR Number | ADR-004 |
| Title | Dependency Injection |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCPTools requires testable, replaceable, and host-integrated services. .NET applications commonly use `Microsoft.Extensions.DependencyInjection` as the default DI abstraction.

### Decision

MCPTools will use `Microsoft.Extensions.DependencyInjection` as the default dependency injection model.

Required dependencies should be supplied through constructor injection. Framework services and tools should be registered through `IServiceCollection` extension methods.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Build a custom DI container | Adds unnecessary complexity and reduces host compatibility. |
| Use service locator patterns | Hides dependencies and weakens testability. |
| Require a third-party DI container | Adds avoidable dependency and limits host flexibility. |

### Consequences

- Framework registration follows common .NET patterns.
- Tools and services are easy to test with mocks or substitutes.
- Hosts can integrate MCPTools into existing service containers.
- Lifetime selection must be documented and reviewed.
- Advanced DI features may require host-specific configuration.

### Future Review

Review if plugin loading requires additional composition patterns beyond standard `IServiceCollection` registration.

## ADR-005: Template-Based Code Generation

| Field | Value |
| --- | --- |
| ADR Number | ADR-005 |
| Title | Template-Based Code Generation |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCPTools will generate source code, configuration files, documentation, SQL scripts, and other artifacts. Hardcoded string generation quickly becomes difficult to maintain and customize.

### Decision

All generated output should be produced from reusable templates rather than hardcoded strings.

The framework will provide template engine abstractions so the rendering implementation can evolve. Scriban is a recommended future implementation, but it is not required as an immediate core dependency.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Generate output with string concatenation | Difficult to maintain, test, customize, and review. |
| Hardcode one rendering engine into core | Reduces replaceability and increases core dependency weight. |
| Allow each tool to implement its own rendering approach | Produces inconsistent output and duplicates logic. |

### Consequences

- Generated artifacts are easier to customize.
- Template output can be tested with snapshot or golden-file tests.
- Business logic remains separated from generated content.
- Template versioning and validation become important framework concerns.
- Template authors need clear naming and context conventions.

### Future Review

Review when adding Scriban support, template marketplace capabilities, custom template overrides, or template versioning.

## ADR-006: Strongly Typed Requests and Responses

| Field | Value |
| --- | --- |
| ADR Number | ADR-006 |
| Title | Strongly Typed Requests and Responses |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

Tools need clear input and output contracts. MCP communication is typically JSON-based, but internal framework code should remain strongly typed for safety and maintainability.

### Decision

Every Tool accepts a Request object and returns a Response object.

Primitive parameter lists should be avoided for executable tool contracts.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Use primitive parameter lists | Hard to version, validate, serialize, and document. |
| Use untyped dictionaries everywhere | Weakens compile-time safety and discoverability. |
| Use raw JSON as the primary internal contract | Couples domain logic to serialization concerns. |

### Consequences

- Tool contracts are easier to validate and document.
- Request and response models map naturally to MCP schemas.
- Versioning is easier when adding optional fields.
- Unit tests can construct explicit inputs and assert structured outputs.
- Small tools may require additional model types, but the consistency is worth the cost.

### Future Review

Review when adding schema generation, backwards-compatible contract versioning, or dynamic plugin tools.

## ADR-007: Centralized Logging

| Field | Value |
| --- | --- |
| ADR Number | ADR-007 |
| Title | Centralized Logging |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCPTools tools may execute inside different hosts and MCP servers. Logging must integrate with standard .NET host logging without imposing a specific observability vendor.

### Decision

MCPTools will use `Microsoft.Extensions.Logging` throughout the framework.

Logging should use `ILogger<T>` and structured message templates.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Use `Console.WriteLine` | Not suitable for libraries, tests, or enterprise hosts. |
| Depend on a specific logging provider | Introduces unnecessary vendor coupling. |
| Avoid framework logging | Makes diagnostics, support, and performance analysis difficult. |

### Consequences

- Hosts control log providers and sinks.
- Structured logs support filtering, correlation, and observability.
- Sensitive data handling must be documented and enforced.
- Logging responsibility must be clear to avoid duplicate logs.

### Future Review

Review when correlation IDs, metrics, tracing, or OpenTelemetry integration are introduced.

## ADR-008: Configuration Strategy

| Field | Value |
| --- | --- |
| ADR Number | ADR-008 |
| Title | Configuration Strategy |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCPTools requires configuration for templates, tools, logging behavior, plugin settings, optional integrations, and execution policies. Static configuration makes tests and host integration difficult.

### Decision

MCPTools will use strongly typed configuration through the .NET Options pattern.

Static configuration should be avoided.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Static configuration classes | Hard to test, override, and isolate between executions. |
| Raw configuration access throughout code | Spreads string keys and weakens validation. |
| Environment variables only | Too limited for structured framework configuration. |

### Consequences

- Configuration is strongly typed and easier to validate.
- Hosts can bind from `appsettings.json`, environment variables, user secrets, or custom providers.
- Options can be tested and overridden per scenario.
- Configuration section names and defaults must be documented.

### Future Review

Review when adding plugin configuration, tenant-specific configuration, or enterprise policy configuration.

## ADR-009: Plugin-Friendly Design

| Field | Value |
| --- | --- |
| ADR Number | ADR-009 |
| Title | Plugin-Friendly Design |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCPTools is expected to grow through optional modules and community-contributed tools. The framework should allow new tools and services to be added without modifying core internals.

### Decision

MCPTools will be designed so new Tools and Services can be added with minimal modification to the framework.

The framework should expose stable abstractions, metadata contracts, registration APIs, and optional extension packages.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Hardcode all tools in the core registry | Prevents external extensibility and creates central bottlenecks. |
| Require core changes for every new tool | Does not scale for open-source contribution. |
| Use reflection-only discovery without explicit contracts | Can be fragile and difficult to validate. |

### Consequences

- External packages can contribute tools.
- The core framework stays smaller and more stable.
- Tool metadata and registration contracts must be carefully versioned.
- Plugin loading introduces future security and compatibility considerations.

### Future Review

Review before implementing assembly scanning, dynamic loading, plugin manifests, or a tool marketplace.

## ADR-010: Testing Strategy

| Field | Value |
| --- | --- |
| ADR Number | ADR-010 |
| Title | Testing Strategy |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCPTools is a framework intended for reuse in automation workflows. Defects in tool execution, validation, template rendering, or file operations can create poor developer experiences or unsafe output.

### Decision

All core framework components should be unit testable.

Testing should focus on behavior, contracts, validation, failure handling, and deterministic output. Core tools and services should be testable without a live MCP client or AI provider.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Test only through MCP server integration | Slower, more fragile, and couples tests to hosting details. |
| Rely primarily on manual testing | Not sufficient for framework quality or open-source contribution. |
| Test only public happy paths | Misses validation, error handling, and edge cases. |

### Consequences

- Code must be designed for dependency injection and isolation.
- File systems, clocks, databases, and external services should be abstracted where needed.
- Template output should be tested with deterministic fixtures.
- Some integration tests will still be required for host and MCP server scenarios.

### Future Review

Review as tooling expands to include CLI, plugins, MCP server SDK, Roslyn, database providers, and cloud integrations.

## ADR-011: Provider-Agnostic MCP Integration

| Field | Value |
| --- | --- |
| ADR Number | ADR-011 |
| Title | Provider-Agnostic MCP Integration |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

MCPTools should integrate with the MCP ecosystem while remaining compatible with many AI clients and host environments.

MCPTools itself is not an MCP server. It should provide infrastructure that can be hosted by an MCP server and discovered by MCP clients.

### Decision

MCPTools integrates with MCP through abstractions rather than vendor-specific APIs.

MCP servers are responsible for transport, protocol endpoints, authentication, request routing, and serialization. MCPTools is responsible for tool registration, metadata, validation, execution, services, templates, logging, and configuration.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Implement provider-specific integration in core | Creates vendor lock-in and limits future compatibility. |
| Make MCPTools itself the only supported MCP server | Restricts host choice and increases framework scope. |
| Expose raw MCP protocol details throughout tools | Couples tool logic to transport concerns. |

### Consequences

- MCPTools can work behind many server implementations.
- The framework remains easier to test outside MCP.
- Hosts must adapt MCP protocol requests to MCPTools request and response models.
- Clear integration documentation is required.

### Future Review

Review when introducing an optional MCP server SDK, MCP schema generation, or official client-specific examples.

## ADR-012: Future Roslyn Integration

| Field | Value |
| --- | --- |
| ADR Number | ADR-012 |
| Title | Future Roslyn Integration |
| Status | Accepted |
| Date | 2026-07-30 |

### Context

Roslyn can provide powerful .NET code analysis, syntax tree inspection, refactoring, diagnostics, and source generation capabilities. These features are important to the long-term roadmap but may add complexity and package weight.

### Decision

MCPTools will keep the architecture ready for Roslyn-based code analysis and source generation without introducing an immediate dependency in the core framework.

Roslyn functionality should be implemented later in an optional package such as `MCPTools.Roslyn`.

### Alternatives Considered

| Alternative | Reason Rejected |
| --- | --- |
| Add Roslyn directly to `MCPTools.Core` immediately | Increases core complexity and dependency weight before the abstraction is proven. |
| Avoid Roslyn permanently | Limits advanced .NET analysis and generation capabilities. |
| Build custom C# parsing logic | Duplicates mature compiler platform functionality and increases defect risk. |

### Consequences

- Core remains lightweight in early releases.
- Future Roslyn package can evolve independently.
- Current abstractions must avoid blocking future syntax and semantic analysis needs.
- Initial code generation may rely more heavily on templates and metadata models.

### Future Review

Review before Version 3.0 or when implementing source analysis, refactoring, diagnostics, or source generation features.

## Future ADR Process

### Adding New ADRs

New ADRs should be added when a decision has long-term architectural impact.

Examples:

- Introducing a new major package.
- Changing dependency direction.
- Adding a new hosting model.
- Choosing a third-party library for a core capability.
- Changing tool lifecycle behavior.
- Changing compatibility or versioning policy.

### Numbering Conventions

ADRs must use sequential numbering:

```text
ADR-001
ADR-002
ADR-003
```

Do not renumber existing ADRs after they are accepted. If an ADR is replaced, mark it as deprecated and reference the replacement ADR.

### Required ADR Format

Each ADR must include:

- ADR Number.
- Title.
- Status.
- Date.
- Context.
- Decision.
- Alternatives Considered.
- Consequences.
- Future Review.

### Review Process

Proposed ADRs should be reviewed by maintainers before being accepted.

The review should consider:

- Alignment with project vision.
- Impact on provider independence.
- Clean Architecture boundaries.
- Compatibility with existing public APIs.
- Testability.
- Security implications.
- Long-term maintenance cost.

### Deprecating Older Decisions

An ADR should be marked `Deprecated` when it is no longer the recommended direction.

Deprecated ADRs should:

- Remain in the log for historical context.
- Explain why the decision changed.
- Reference the newer ADR that replaces it.
- Avoid being deleted unless the repository maintainers explicitly decide otherwise.

### Versioning of ADRs

ADRs are versioned through repository history.

For significant changes:

- Prefer adding a new ADR instead of rewriting an accepted decision.
- Update the status of older ADRs when superseded.
- Reference related roadmap versions when relevant.
- Document migration implications when a decision affects public APIs.

The ADR log should remain concise, factual, and useful to future maintainers.
