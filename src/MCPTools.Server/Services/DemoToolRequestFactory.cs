using System.Text.Json;
using MCPTools.Core.Tools.Crud;
using MCPTools.Server.Models;
using Microsoft.Extensions.Configuration;

namespace MCPTools.Server.Services;

/// <summary>
/// Creates sample requests for the MCPTools server demonstration host.
/// </summary>
public sealed class DemoToolRequestFactory
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoToolRequestFactory"/> class.
    /// </summary>
    /// <param name="configuration">The host configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is <see langword="null"/>.</exception>
    public DemoToolRequestFactory(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Attempts to create a demonstration request for the specified tool.
    /// </summary>
    /// <param name="descriptor">The tool descriptor.</param>
    /// <param name="request">When this method returns, contains the demonstration request if supported.</param>
    /// <returns><see langword="true"/> when a demonstration request was created; otherwise, <see langword="false"/>.</returns>
    public bool TryCreateRequest(ToolDescriptor descriptor, out McpRequest? request)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        if (descriptor.ToolType.Name.Equals(nameof(GenerateCrudTool), StringComparison.OrdinalIgnoreCase)
            || descriptor.ToolName.Equals("generate-crud", StringComparison.OrdinalIgnoreCase))
        {
            request = new McpRequest
            {
                RequestId = Guid.NewGuid().ToString("N"),
                ToolName = descriptor.ToolName,
                Input = JsonSerializer.SerializeToElement(CreateGenerateCrudRequest())
            };
            return true;
        }

        request = null;
        return false;
    }

    private GenerateCrudRequest CreateGenerateCrudRequest()
    {
        return new GenerateCrudRequest
        {
            EntityName = GetValue("Demo:GenerateCrud:EntityName", "Employee"),
            PluralEntityName = GetValue("Demo:GenerateCrud:PluralEntityName", "Employees"),
            TableName = GetValue("Demo:GenerateCrud:TableName", "Employees"),
            PrimaryKey = GetValue("Demo:GenerateCrud:PrimaryKey", "EmployeeId"),
            PrimaryKeyType = GetValue("Demo:GenerateCrud:PrimaryKeyType", "int"),
            Namespace = GetValue("Demo:GenerateCrud:Namespace", "Demo.HRMS"),
            OutputDirectory = GetValue("Demo:GenerateCrud:OutputDirectory", "Generated"),
            Author = GetValue("Demo:GenerateCrud:Author", "MCPTools"),
            CompanyName = GetValue("Demo:GenerateCrud:CompanyName", "MCPTools"),
            GenerateRepository = true,
            GenerateService = true,
            GenerateController = true,
            GenerateDto = true,
            GenerateInterface = true,
            OverwriteExistingFiles = true
        };
    }

    private string GetValue(string key, string defaultValue)
    {
        return string.IsNullOrWhiteSpace(_configuration[key])
            ? defaultValue
            : _configuration[key]!;
    }
}
