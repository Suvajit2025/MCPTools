using MCPTools.Core.Exceptions;

namespace MCPTools.Core.Tools.Code;

internal static class SolutionPathResolver
{
    public static string Resolve(string solutionPath)
    {
        var fullPath = Path.GetFullPath(solutionPath);

        if (File.Exists(fullPath))
        {
            return fullPath;
        }

        if (!Directory.Exists(fullPath))
        {
            throw new ToolValidationException($"Solution path '{solutionPath}' does not exist.");
        }

        var solutionFiles = Directory
            .EnumerateFiles(fullPath, "*.sln", SearchOption.TopDirectoryOnly)
            .Concat(Directory.EnumerateFiles(fullPath, "*.slnx", SearchOption.TopDirectoryOnly))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return solutionFiles.Length switch
        {
            1 => solutionFiles[0],
            0 => throw new ToolValidationException($"Directory '{solutionPath}' does not contain a solution file."),
            _ => throw new ToolValidationException($"Directory '{solutionPath}' contains multiple solution files.")
        };
    }
}
