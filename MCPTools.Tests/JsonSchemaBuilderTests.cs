using System.Text.Json;
using MCPTools.Server.Services;

namespace MCPTools.Tests;

public sealed class JsonSchemaBuilderTests
{
    [Fact]
    public void BuildSchema_CreatesArrayItemsSchema_WhenModelContainsArray()
    {
        var builder = new JsonSchemaBuilder();

        var schema = builder.BuildSchema(typeof(SampleSchemaModel));
        var names = schema.GetProperty("properties").GetProperty("names");
        var scores = schema.GetProperty("properties").GetProperty("scores");

        Assert.Equal("array", names.GetProperty("type").GetString());
        Assert.Equal("string", names.GetProperty("items").GetProperty("type").GetString());
        Assert.Equal("array", scores.GetProperty("type").GetString());
        Assert.Equal("integer", scores.GetProperty("items").GetProperty("type").GetString());
    }

    [Fact]
    public void BuildSchema_CreatesDictionaryAdditionalPropertiesSchema_WhenModelContainsDictionary()
    {
        var builder = new JsonSchemaBuilder();

        var schema = builder.BuildSchema(typeof(SampleSchemaModel));
        var metadata = schema.GetProperty("properties").GetProperty("metadata");

        Assert.Equal("object", metadata.GetProperty("type").GetString());
        Assert.Equal("string", metadata.GetProperty("additionalProperties").GetProperty("type").GetString());
    }

    [Fact]
    public void BuildSchema_CreatesNestedObjectSchema_WhenModelContainsNestedObject()
    {
        var builder = new JsonSchemaBuilder();

        var schema = builder.BuildSchema(typeof(SampleSchemaModel));
        var address = schema.GetProperty("properties").GetProperty("address");

        Assert.Equal("object", address.GetProperty("type").GetString());
        Assert.Equal("string", address.GetProperty("properties").GetProperty("line1").GetProperty("type").GetString());
    }

    [Fact]
    public void BuildSchema_CreatesEnumSchema_WhenModelContainsEnum()
    {
        var builder = new JsonSchemaBuilder();

        var schema = builder.BuildSchema(typeof(SampleSchemaModel));
        var status = schema.GetProperty("properties").GetProperty("status");
        var enumValues = status.GetProperty("enum").EnumerateArray().Select(value => value.GetString()).ToArray();

        Assert.Equal("string", status.GetProperty("type").GetString());
        Assert.Contains(nameof(SampleStatus.Active), enumValues);
        Assert.Contains(nameof(SampleStatus.Inactive), enumValues);
    }

    [Fact]
    public void BuildSchema_IncludesNullType_WhenPropertyIsNullableValueType()
    {
        var builder = new JsonSchemaBuilder();

        var schema = builder.BuildSchema(typeof(SampleSchemaModel));
        var optionalCount = schema.GetProperty("properties").GetProperty("optionalCount");
        var typeNames = optionalCount.GetProperty("type").EnumerateArray().Select(value => value.GetString()).ToArray();

        Assert.Contains("integer", typeNames);
        Assert.Contains("null", typeNames);
    }

    [Fact]
    public void BuildSchema_IncludesRequiredProperties_WhenPropertiesUseRequiredModifier()
    {
        var builder = new JsonSchemaBuilder();

        var schema = builder.BuildSchema(typeof(SampleSchemaModel));
        var required = schema.GetProperty("required").EnumerateArray().Select(value => value.GetString()).ToArray();

        Assert.Contains("name", required);
        Assert.Contains("names", required);
    }

    [Fact]
    public void BuildSchema_StopsRecursiveExpansion_WhenTypeReferencesItself()
    {
        var builder = new JsonSchemaBuilder();

        var schema = builder.BuildSchema(typeof(RecursiveSchemaModel));
        var child = schema.GetProperty("properties").GetProperty("child");

        Assert.Equal("object", child.GetProperty("type").GetString());
        Assert.True(child.TryGetProperty("additionalProperties", out var additionalProperties));
        Assert.Equal(JsonValueKind.True, additionalProperties.ValueKind);
    }

    private sealed class SampleSchemaModel
    {
        public required string Name { get; init; }

        public required IReadOnlyList<string> Names { get; init; }

        public int[] Scores { get; init; } = [];

        public Dictionary<string, string> Metadata { get; init; } = [];

        public SampleAddress Address { get; init; } = new();

        public SampleStatus Status { get; init; }

        public int? OptionalCount { get; init; }
    }

    private sealed class SampleAddress
    {
        public string? Line1 { get; init; }
    }

    private sealed class RecursiveSchemaModel
    {
        public RecursiveSchemaModel? Child { get; init; }
    }

    private enum SampleStatus
    {
        Active,
        Inactive
    }
}
