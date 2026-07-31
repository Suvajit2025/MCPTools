using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MCPTools.Server.Models;
using MCPTools.Server.Services;

namespace MCPTools.Server;

/// <summary>
/// Represents the thin MCP server host lifecycle for MCPTools.
/// </summary>
public sealed class McpHost : BackgroundService
{
    private readonly ILogger<McpHost> _logger;
    private readonly ToolCatalog _toolCatalog;
    private readonly McpRequestProcessor _requestProcessor;
    private readonly DemoToolRequestFactory _demoToolRequestFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpHost"/> class.
    /// </summary>
    /// <param name="logger">The logger used to record server lifecycle events.</param>
    /// <param name="toolCatalog">The internal catalog of discovered MCPTools tools.</param>
    /// <param name="requestProcessor">The request processor used to invoke demonstration tool requests.</param>
    /// <param name="demoToolRequestFactory">The factory used to create demonstration tool requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is <see langword="null"/>.</exception>
    public McpHost(
        ILogger<McpHost> logger,
        ToolCatalog toolCatalog,
        McpRequestProcessor requestProcessor,
        DemoToolRequestFactory demoToolRequestFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _toolCatalog = toolCatalog ?? throw new ArgumentNullException(nameof(toolCatalog));
        _requestProcessor = requestProcessor ?? throw new ArgumentNullException(nameof(requestProcessor));
        _demoToolRequestFactory = demoToolRequestFactory ?? throw new ArgumentNullException(nameof(demoToolRequestFactory));
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MCPTools server host started with {ToolCount} discovered tools.", _toolCatalog.Count);
        DisplayStartup();

        try
        {
            await RunInteractiveModeAsync(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        finally
        {
            _logger.LogInformation("MCPTools server host stopped.");
        }
    }

    private async Task RunInteractiveModeAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            WritePrompt();

            var command = await Task.Run(Console.ReadLine, cancellationToken);

            if (command is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(command))
            {
                continue;
            }

            if (command.Equals("exit", StringComparison.OrdinalIgnoreCase)
                || command.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            await ExecuteCommandAsync(command, cancellationToken);
        }
    }

    private async Task ExecuteCommandAsync(string command, CancellationToken cancellationToken)
    {
        var parts = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var commandName = parts[0];

        if (commandName.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            DisplayCommands();
            return;
        }

        if (commandName.Equals("list-tools", StringComparison.OrdinalIgnoreCase))
        {
            DisplayTools();
            return;
        }

        if (commandName.Equals("run-tool", StringComparison.OrdinalIgnoreCase))
        {
            await RunToolAsync(parts.Length > 1 ? parts[1] : null, cancellationToken);
            return;
        }

        WriteLine($"Unknown command '{commandName}'. Type 'help' for available commands.");
    }

    private async Task RunToolAsync(string? toolIdentifier, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(toolIdentifier))
        {
            WriteLine("Usage: run-tool <tool-name|tool-type>");
            return;
        }

        var descriptor = FindTool(toolIdentifier);

        if (descriptor is null)
        {
            WriteLine($"Tool '{toolIdentifier}' was not found.");
            return;
        }

        if (!_demoToolRequestFactory.TryCreateRequest(descriptor, out var request) || request is null)
        {
            WriteLine($"No demonstration request is configured for '{descriptor.ToolType.Name}'.");
            return;
        }

        var result = await _requestProcessor.ProcessAsync(request, cancellationToken);
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });

        WriteLine(json);
    }

    private ToolDescriptor? FindTool(string toolIdentifier)
    {
        return _toolCatalog.Tools.FirstOrDefault(tool =>
            tool.ToolName.Equals(toolIdentifier, StringComparison.OrdinalIgnoreCase)
            || tool.ToolType.Name.Equals(toolIdentifier, StringComparison.OrdinalIgnoreCase)
            || (tool.DisplayName?.Equals(toolIdentifier, StringComparison.OrdinalIgnoreCase) == true));
    }

    private void DisplayStartup()
    {
        WriteSeparator();
        WriteLine("MCPTools Server");
        WriteSeparator();
        WriteValue("Version", GetServerVersion());
        WriteValue("Registered Tools", string.Join(", ", _toolCatalog.Tools.Select(tool => tool.ToolName)));
        WriteValue("Tool Count", _toolCatalog.Count.ToString());
        WriteSeparator();
        DisplayCommands();
    }

    private void DisplayCommands()
    {
        WriteLine("Available Commands");
        WriteLine("list-tools");
        WriteLine("run-tool GenerateCrudTool");
        WriteLine("help");
        WriteLine("exit");
        WriteSeparator();
    }

    private void DisplayTools()
    {
        foreach (var tool in _toolCatalog.Tools)
        {
            WriteLine($"{tool.ToolName} ({tool.ToolType.Name}) - {tool.Description}");
        }
    }

    private static string GetServerVersion()
    {
        return typeof(McpHost).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? typeof(McpHost).Assembly.GetName().Version?.ToString()
            ?? "1.0.0";
    }

    private static void WritePrompt()
    {
        Console.Write("mcp-tools> ");
    }

    private static void WriteValue(string name, string value)
    {
        WriteLine($"{name}: {value}");
    }

    private static void WriteSeparator()
    {
        WriteLine("-----------------------------------");
    }

    private static void WriteLine(string value)
    {
        Console.WriteLine(value);
    }
}
