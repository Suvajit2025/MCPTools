using MCPTools.Server.Extensions;
using MCPTools.Server.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddMCPToolsServer(builder.Configuration);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithListToolsHandler((request, cancellationToken) =>
    {
        var services = request.Services
            ?? throw new InvalidOperationException("The MCP request did not include a service provider.");
        var adapter = services.GetRequiredService<McpToolAdapter>();
        return adapter.ListToolsAsync(request, cancellationToken);
    })
    .WithCallToolHandler((request, cancellationToken) =>
    {
        var services = request.Services
            ?? throw new InvalidOperationException("The MCP request did not include a service provider.");
        var adapter = services.GetRequiredService<McpToolAdapter>();
        return adapter.CallToolAsync(request, cancellationToken);
    });

using var host = builder.Build();

await host.RunAsync();
