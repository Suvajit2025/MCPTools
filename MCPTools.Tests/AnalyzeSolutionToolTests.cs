using MCPTools.Core.Interfaces;
using MCPTools.Core.Models.Solution;
using MCPTools.Core.Models.Solution.Syntax;
using MCPTools.Core.Tools.Solution;
using Microsoft.Extensions.Logging.Abstractions;
using SyntaxClassModel = MCPTools.Core.Models.Solution.Syntax.ClassModel;

namespace MCPTools.Tests;

public sealed class AnalyzeSolutionToolTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsSummaryStatistics_WhenSolutionIsAnalyzed()
    {
        var solutionPath = CreateSolutionFile();
        var solution = CreateSolutionModel(solutionPath);
        var tool = new AnalyzeSolutionTool(
            new FakeSolutionScanner(solution),
            new FakeRoslynParser(solution),
            new FakeDependencyAnalyzer(),
            NullLogger<AnalyzeSolutionTool>.Instance);

        var result = await tool.ExecuteAsync(new AnalyzeSolutionRequest
        {
            SolutionPath = solutionPath
        });

        Assert.True(result.Success);
        Assert.Equal("SampleSolution", result.SolutionName);
        Assert.Equal(1, result.ProjectCount);
        Assert.Equal(1, result.NamespaceCount);
        Assert.Equal(5, result.ClassCount);
        Assert.Equal(1, result.InterfaceCount);
        Assert.Equal(2, result.MethodCount);
        Assert.Equal(2, result.PropertyCount);
        Assert.Equal(2, result.DependencyCount);
        Assert.Contains("EmployeeController", result.ControllersDetected);
        Assert.Contains("EmployeeRepository", result.RepositoriesDetected);
        Assert.Contains("EmployeeService", result.ServicesDetected);
        Assert.Contains("EmployeeDto", result.DtosDetected);
        Assert.Contains("Employee", result.EntitiesDetected);
        Assert.Equal("net10.0", result.TargetFrameworks["Sample.Api"]);
        Assert.Equal("Exe", result.OutputTypes["Sample.Api"]);
        Assert.Single(result.ProjectReferences);
    }

    private static string CreateSolutionFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), "MCPTools.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var solutionPath = Path.Combine(directory, "SampleSolution.slnx");
        File.WriteAllText(solutionPath, "<Solution />");
        return solutionPath;
    }

    private static SolutionModel CreateSolutionModel(string solutionPath)
    {
        return new SolutionModel
        {
            Name = "SampleSolution",
            Path = solutionPath,
            Projects =
            [
                new ProjectModel
                {
                    Name = "Sample.Api",
                    Path = Path.Combine(Path.GetDirectoryName(solutionPath)!, "Sample.Api.csproj"),
                    TargetFramework = "net10.0",
                    OutputType = "Exe",
                    References =
                    [
                        new ProjectReferenceModel
                        {
                            Name = "Sample.Core",
                            Path = "Sample.Core.csproj"
                        }
                    ],
                    SourceFiles =
                    [
                        new SourceFileModel
                        {
                            Name = "EmployeeController.cs",
                            Path = Path.Combine(Path.GetDirectoryName(solutionPath)!, "EmployeeController.cs"),
                            Extension = ".cs",
                            Namespace = "Sample.Api.Controllers",
                            Namespaces =
                            [
                                new NamespaceModel
                                {
                                    Name = "Sample.Api.Controllers",
                                    Classes =
                                    [
                                        new SyntaxClassModel
                                        {
                                            Name = "EmployeeController",
                                            Namespace = "Sample.Api.Controllers",
                                            BaseTypes =
                                            [
                                                new BaseTypeModel
                                                {
                                                    Name = "ControllerBase"
                                                }
                                            ],
                                            Methods =
                                            [
                                                new MethodModel
                                                {
                                                    Name = "GetAsync"
                                                },
                                                new MethodModel
                                                {
                                                    Name = "PostAsync"
                                                }
                                            ],
                                            Properties =
                                            [
                                                new PropertyModel
                                                {
                                                    Name = "Service"
                                                }
                                            ]
                                        },
                                        new SyntaxClassModel
                                        {
                                            Name = "EmployeeRepository",
                                            Namespace = "Sample.Infrastructure.Repositories"
                                        },
                                        new SyntaxClassModel
                                        {
                                            Name = "EmployeeService",
                                            Namespace = "Sample.Application.Services",
                                            Properties =
                                            [
                                                new PropertyModel
                                                {
                                                    Name = "Repository"
                                                }
                                            ]
                                        },
                                        new SyntaxClassModel
                                        {
                                            Name = "EmployeeDto",
                                            Namespace = "Sample.Application.Dtos"
                                        },
                                        new SyntaxClassModel
                                        {
                                            Name = "Employee",
                                            Namespace = "Sample.Domain.Entities"
                                        }
                                    ],
                                    Interfaces =
                                    [
                                        new InterfaceModel
                                        {
                                            Name = "IEmployeeService"
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        };
    }

    private sealed class FakeSolutionScanner(SolutionModel solution) : ISolutionScanner
    {
        public Task<SolutionModel> ScanAsync(
            string solutionPath,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(solution);
        }
    }

    private sealed class FakeRoslynParser(SolutionModel solution) : IRoslynParser
    {
        public Task<SourceFileModel> ParseFileAsync(
            string sourceFilePath,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(solution.Projects[0].SourceFiles[0]);
        }

        public Task<IReadOnlyList<SourceFileModel>> ParseFilesAsync(
            IEnumerable<string> sourceFilePaths,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(solution.Projects[0].SourceFiles);
        }

        public Task<SolutionModel> ParseSolutionAsync(
            SolutionModel solutionModel,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(solution);
        }
    }

    private sealed class FakeDependencyAnalyzer : IDependencyAnalyzer
    {
        public Task<IReadOnlyList<DependencyModel>> AnalyzeSolutionAsync(
            SolutionModel solution,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<DependencyModel> dependencies =
            [
                new DependencyModel
                {
                    SourceName = "EmployeeController",
                    Name = "IEmployeeService",
                    Relationship = "ConstructorInjection"
                },
                new DependencyModel
                {
                    SourceName = "EmployeeService",
                    Name = "IEmployeeRepository",
                    Relationship = "ConstructorInjection"
                }
            ];

            return Task.FromResult(dependencies);
        }

        public Task<IReadOnlyList<DependencyModel>> AnalyzeProjectAsync(
            ProjectModel project,
            CancellationToken cancellationToken = default)
        {
            return AnalyzeSolutionAsync(new SolutionModel { Projects = [project] }, cancellationToken);
        }

        public Task<IReadOnlyList<DependencyModel>> AnalyzeFileAsync(
            string sourceFilePath,
            string? projectName = null,
            IReadOnlySet<string>? knownSolutionTypes = null,
            CancellationToken cancellationToken = default)
        {
            return AnalyzeSolutionAsync(new SolutionModel(), cancellationToken);
        }
    }
}
