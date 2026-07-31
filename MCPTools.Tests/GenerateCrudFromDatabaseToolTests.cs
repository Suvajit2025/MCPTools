using MCPTools.Core.Configuration;
using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Schema;
using MCPTools.Core.Services;
using MCPTools.Core.TemplateEngine;
using MCPTools.Core.Tools.Crud;
using MCPTools.Core.Tools.Database;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace MCPTools.Tests;

public sealed class GenerateCrudFromDatabaseToolTests
{
    [Fact]
    public async Task ExecuteAsync_GeneratesCrudArtifacts_WhenTableExists()
    {
        var templateRoot = CreateTemplateRoot();
        var outputDirectory = Path.Combine(Path.GetTempPath(), "MCPTools.Tests", Guid.NewGuid().ToString("N"));
        var tool = CreateTool(templateRoot, outputDirectory);

        var response = await tool.ExecuteAsync(new GenerateCrudFromDatabaseRequest
        {
            TableName = "Employee",
            Namespace = "Demo.HRMS",
            OutputDirectory = outputDirectory,
            OverwriteExistingFiles = true
        });

        Assert.Equal("Employee", response.TableName);
        Assert.Equal("Employee", response.EntityName);
        Assert.True(response.CrudGeneration.Success);
        Assert.Equal(16, response.CrudGeneration.GeneratedFiles.Count);
        Assert.All(response.CrudGeneration.GeneratedFiles, file => Assert.True(File.Exists(file)));
    }

    private static GenerateCrudFromDatabaseTool CreateTool(string templateRoot, string outputDirectory)
    {
        var templateOptions = Options.Create(new TemplateOptions
        {
            TemplateRoot = templateRoot,
            CacheTemplates = false
        });
        var outputOptions = Options.Create(new OutputOptions
        {
            OutputDirectory = outputDirectory,
            OverwriteExistingFiles = true
        });
        var databaseOptions = Options.Create(new DatabaseConnectionOptions
        {
            Database = "DemoHRMS"
        });

        var templateEngine = new TemplateEngine();
        var placeholderBuilder = new PlaceholderBuilder();
        var fileGenerator = new FileGenerator(outputOptions);
        var templateDiscoveryService = new TemplateDiscoveryService(templateOptions);
        var namingConventionService = new NamingConventionService();
        var crudTool = new GenerateCrudTool(
            templateEngine,
            placeholderBuilder,
            fileGenerator,
            templateDiscoveryService,
            namingConventionService);

        return new GenerateCrudFromDatabaseTool(
            new FakeSchemaProvider(),
            placeholderBuilder,
            fileGenerator,
            templateDiscoveryService,
            namingConventionService,
            templateEngine,
            crudTool,
            NullLogger<GenerateCrudFromDatabaseTool>.Instance,
            outputOptions,
            databaseOptions);
    }

    private static string CreateTemplateRoot()
    {
        var templateRoot = Path.Combine(Path.GetTempPath(), "MCPTools.Tests", Guid.NewGuid().ToString("N"), "Templates");
        var templates = new[]
        {
            "Domain/Entity.tpl",
            "Domain/IRepository.tpl",
            "Infrastructure/Repository.tpl",
            "Application/IService.tpl",
            "Application/Service.tpl",
            "Api/Controller.tpl",
            "Application/Dto.tpl",
            "Application/CreateRequest.tpl",
            "Application/UpdateRequest.tpl",
            "Application/Response.tpl",
            "Application/Mapping.tpl",
            "SqlServer/InsertProcedure.tpl",
            "SqlServer/UpdateProcedure.tpl",
            "SqlServer/DeleteProcedure.tpl",
            "SqlServer/GetByIdProcedure.tpl",
            "SqlServer/GetAllProcedure.tpl"
        };

        foreach (var template in templates)
        {
            var path = Path.Combine(templateRoot, template);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, "{{Namespace}} {{EntityName}} {{TableName}} {{PrimaryKey}} {{PrimaryKeyType}}");
        }

        return templateRoot;
    }

    private sealed class FakeSchemaProvider : ISchemaProvider
    {
        public Task<List<TableSchema>> GetTablesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<TableSchema> { CreateTable() });
        }

        public Task<TableSchema?> GetTableAsync(
            string tableName,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<TableSchema?>(CreateTable());
        }

        public Task<bool> TableExistsAsync(
            string tableName,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(string.Equals(tableName, "Employee", StringComparison.OrdinalIgnoreCase));
        }

        private static TableSchema CreateTable()
        {
            return new TableSchema
            {
                Name = "Employee",
                Schema = "dbo",
                PrimaryKey = new PrimaryKeySchema
                {
                    Name = "PK_Employee",
                    Columns = ["EmployeeId"]
                },
                Columns =
                [
                    new ColumnSchema
                    {
                        Name = "EmployeeId",
                        DataType = "int",
                        IsPrimaryKey = true,
                        IsIdentity = true
                    },
                    new ColumnSchema
                    {
                        Name = "FirstName",
                        DataType = "nvarchar",
                        MaxLength = 100
                    }
                ]
            };
        }
    }
}
