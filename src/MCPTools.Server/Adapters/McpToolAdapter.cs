using System.Text.Json;
using System.Text.Json.Nodes;
using MCPTools.Server.Models;
using MCPTools.Server.Services;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace MCPTools.Server.Adapters;

/// <summary>
/// Adapts discovered MCPTools tools to official MCP SDK protocol results.
/// </summary>
public sealed class McpToolAdapter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly ToolCatalog _toolCatalog;
    private readonly McpRequestProcessor _requestProcessor;
    private readonly JsonSchemaBuilder _jsonSchemaBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpToolAdapter"/> class.
    /// </summary>
    /// <param name="toolCatalog">The discovered MCPTools tool catalog.</param>
    /// <param name="requestProcessor">The existing request processor used to invoke tools.</param>
    /// <param name="jsonSchemaBuilder">The JSON Schema builder used to expose tool contracts.</param>
    public McpToolAdapter(
        ToolCatalog toolCatalog,
        McpRequestProcessor requestProcessor,
        JsonSchemaBuilder jsonSchemaBuilder)
    {
        _toolCatalog = toolCatalog ?? throw new ArgumentNullException(nameof(toolCatalog));
        _requestProcessor = requestProcessor ?? throw new ArgumentNullException(nameof(requestProcessor));
        _jsonSchemaBuilder = jsonSchemaBuilder ?? throw new ArgumentNullException(nameof(jsonSchemaBuilder));
    }

    /// <summary>
    /// Lists every discovered MCPTools tool as an MCP protocol tool.
    /// </summary>
    /// <param name="request">The MCP list tools request context.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The MCP tools list result.</returns>
    public ValueTask<ListToolsResult> ListToolsAsync(
        RequestContext<ListToolsRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        return ValueTask.FromResult(new ListToolsResult
        {
            Tools = CreateProtocolTools()
        });
    }

    /// <summary>
    /// Creates MCP protocol tool definitions from the discovered MCPTools catalog.
    /// </summary>
    /// <returns>The MCP protocol tool definitions.</returns>
    public IList<Tool> CreateProtocolTools()
    {
        return _toolCatalog.Tools.Select(CreateProtocolTool).ToArray();
    }

    /// <summary>
    /// Invokes a discovered MCPTools tool from an MCP call tool request.
    /// </summary>
    /// <param name="request">The MCP call tool request context.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The MCP call tool result.</returns>
    public async ValueTask<CallToolResult> CallToolAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var parameters = request.Params;
        var input = JsonSerializer.SerializeToElement(
            parameters?.Arguments ?? new Dictionary<string, JsonElement>(),
            SerializerOptions);
        var result = await _requestProcessor.ProcessAsync(
            new McpRequest
            {
                RequestId = request.JsonRpcRequest.Id.ToString(),
                ToolName = parameters?.Name ?? string.Empty,
                Input = input
            },
            cancellationToken);

        return result.Success
            ? CreateSuccessResult(result)
            : CreateErrorResult(result);
    }

    private Tool CreateProtocolTool(ToolDescriptor descriptor)
    {
        return new Tool
        {
            Name = descriptor.ToolName,
            Title = descriptor.DisplayName,
            Description = descriptor.Description,
            InputSchema = _jsonSchemaBuilder.BuildSchema(descriptor.RequestType),
            OutputSchema = _jsonSchemaBuilder.BuildSchema(descriptor.ResponseType),
            Annotations = new ToolAnnotations
            {
                Title = descriptor.DisplayName
            },
            Meta = new JsonObject
            {
                ["category"] = descriptor.Category,
                ["version"] = descriptor.Version,
                ["author"] = descriptor.Author,
                ["supportedFrameworkVersion"] = descriptor.SupportedFrameworkVersion,
                ["tags"] = new JsonArray(descriptor.Tags.Select(tag => JsonValue.Create(tag)).ToArray<JsonNode?>())
            }
        };
    }

    private static CallToolResult CreateSuccessResult(McpResult result)
    {
        var text = result.Result?.GetRawText() ?? string.Empty;

        return new CallToolResult
        {
            Content =
            [
                new TextContentBlock
                {
                    Text = text
                }
            ],
            StructuredContent = result.Result,
            IsError = false
        };
    }

    private static CallToolResult CreateErrorResult(McpResult result)
    {
        JsonElement? structuredContent = result.Error is null
            ? null
            : JsonSerializer.SerializeToElement(result.Error, SerializerOptions);

        return new CallToolResult
        {
            Content =
            [
                new TextContentBlock
                {
                    Text = result.Error?.Message ?? "Tool execution failed."
                }
            ],
            StructuredContent = structuredContent,
            IsError = true
        };
    }
}
