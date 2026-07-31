using MCPTools.Core.Configuration;
using MCPTools.Core.Extensions;
using MCPTools.Core.Models.Schema;
using MCPTools.Core.Tools.Solution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile(
    Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
    optional: false,
    reloadOnChange: true);

builder.Services.AddMCPTools();

builder.Services.Configure<TemplateOptions>(
    builder.Configuration.GetSection("MCPTools:Templates"));

builder.Services.Configure<OutputOptions>(
    builder.Configuration.GetSection("MCPTools:Output"));

builder.Services.Configure<DatabaseConnectionOptions>(
    builder.Configuration.GetSection("MCPTools:Database"));

using var host = builder.Build();
using var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};

try
{
    var configuration = host.Services.GetRequiredService<IConfiguration>();
    var tool = host.Services.GetRequiredService<AnalyzeSolutionTool>();
    var request = CreateRequest(configuration);

    WriteHeader("MCPTools Solution Analyzer");

    var response = await tool.ExecuteAsync(request, cancellationTokenSource.Token);

    WriteValue("Solution", response.SolutionName ?? "Unknown");
    WriteValue("Path", response.SolutionPath ?? request.SolutionPath);
    WriteValue("Projects", response.ProjectCount.ToString());
    WriteValue("Classes", response.ClassCount.ToString());
    WriteValue("Methods", response.MethodCount.ToString());
    WriteValue("Interfaces", response.InterfaceCount.ToString());
    WriteValue("Dependencies", response.DependencyCount.ToString());
    WriteValue("Elapsed Time", $"{response.ElapsedTime.TotalMilliseconds:N0} ms");

    WriteSeparator();

    if (!response.Success)
    {
        WriteLine(response.Message ?? "Solution analysis failed.");
        return 1;
    }

    return 0;
}
catch (OperationCanceledException)
{
    WriteLine("Generation cancelled.");
    return 2;
}
catch (Exception exception)
{
    WriteHeader("Generation Failed");
    WriteValue("Error Message", exception.Message);
    WriteInnerExceptions(exception);
    WriteValue("Stack Trace", exception.StackTrace ?? "No stack trace available.");
    WriteValue("Exit Code", "1");
    return 1;
}

static AnalyzeSolutionRequest CreateRequest(IConfiguration configuration)
{
    return new AnalyzeSolutionRequest
    {
        SolutionPath = GetRequiredValue(configuration, "SolutionAnalysis:SolutionPath")
    };
}

static string GetRequiredValue(IConfiguration configuration, string key)
{
    var value = configuration[key];

    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"Configuration value '{key}' is required.");
    }

    return value;
}

static void WriteHeader(string title)
{
    WriteSeparator();
    WriteLine(title);
    WriteSeparator();
}

static void WriteValue(string name, string value)
{
    WriteLine($"{name}: {value}");
}

static void WriteInnerExceptions(Exception exception)
{
    var innerException = exception.InnerException;

    while (innerException is not null)
    {
        WriteValue("Inner Error", innerException.Message);
        innerException = innerException.InnerException;
    }
}

static void WriteSeparator()
{
    WriteLine("-----------------------------------");
}

static void WriteLine(string value)
{
    Console.WriteLine(value);
}
