using MCPTools.Core.Configuration;
using MCPTools.Core.Extensions;
using MCPTools.Core.Tools.Crud;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile(
    Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
    optional: false,
    reloadOnChange: true);

builder.Services.AddMCPTools();
builder.Services.AddTransient<GenerateCrudTool>();

builder.Services.Configure<TemplateOptions>(
    builder.Configuration.GetSection("MCPTools:Templates"));

builder.Services.Configure<OutputOptions>(
    builder.Configuration.GetSection("MCPTools:Output"));

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
    var tool = host.Services.GetRequiredService<GenerateCrudTool>();
    var request = CreateRequest(configuration);

    WriteHeader("MCPTools CRUD Generator");

    var response = await tool.ExecuteAsync(request, cancellationTokenSource.Token);

    WriteValue("Entity", request.EntityName);
    WriteValue("Namespace", request.Namespace);
    WriteValue("Generated Files", response.GeneratedFiles.Count.ToString());
    WriteValue("Skipped Files", response.SkippedFiles.Count.ToString());
    WriteValue("Elapsed Time", $"{response.ElapsedTime.TotalMilliseconds:N0} ms");
    WriteValue("Output", request.OutputDirectory);

    WriteSeparator();

    if (!response.Success)
    {
        WriteLine(response.Message ?? "Generation failed.");
        WriteLines(response.Errors);
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

static GenerateCrudRequest CreateRequest(IConfiguration configuration)
{
    return new GenerateCrudRequest
    {
        EntityName = GetRequiredValue(configuration, "Sample:EntityName"),
        PluralEntityName = GetRequiredValue(configuration, "Sample:PluralEntityName"),
        TableName = GetRequiredValue(configuration, "Sample:TableName"),
        PrimaryKey = GetRequiredValue(configuration, "Sample:PrimaryKey"),
        PrimaryKeyType = GetRequiredValue(configuration, "Sample:PrimaryKeyType"),
        Namespace = GetRequiredValue(configuration, "Sample:Namespace"),
        OutputDirectory = GetRequiredValue(configuration, "MCPTools:Output:OutputDirectory"),
        Author = GetRequiredValue(configuration, "Sample:Author"),
        CompanyName = GetRequiredValue(configuration, "Sample:CompanyName"),
        GenerateRepository = true,
        GenerateService = true,
        GenerateController = true,
        GenerateDto = true,
        GenerateInterface = true,
        OverwriteExistingFiles = configuration.GetValue("MCPTools:Output:OverwriteExistingFiles", false)
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

static void WriteLines(IEnumerable<string> values)
{
    foreach (var value in values)
    {
        WriteLine(value);
    }
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
