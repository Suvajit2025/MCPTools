using MCPTools.Server.Extensions;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMCPToolsServer(builder.Configuration);

using var host = builder.Build();

await host.RunAsync();
