using System.Diagnostics;
using System.Text.RegularExpressions;

var sw = Stopwatch.StartNew();
var repositoryDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".."));
Console.WriteLine("Searching directory: " + repositoryDirectory);

const string csproj = "*.csproj";

var searchConfigs = new List<string>
{
    "<TargetFramework>(.*?)</TargetFramework>",
    "<TargetFrameworkVersion>(.*?)</TargetFrameworkVersion>",
};

var ignoredPaths = new List<string>
{
    "WM2", "WM-2", "WindmanagerFrontend", "Blazor", "Razor"
};

var versionProjects = new Dictionary<string, List<string>>();
foreach (var versionRegex in searchConfigs)
{
    var projectFiles = Directory.GetFiles(repositoryDirectory, csproj, SearchOption.AllDirectories);

    foreach (var projectFile in projectFiles)
    {
        if (ignoredPaths.Any(s => projectFile.Contains(s)))
        {
            continue;
        }

        var content = File.ReadAllText(projectFile);
        var match = Regex.Match(content, versionRegex);
        if (match.Success)
        {
            var version = match.Groups[1].Value;
            if (!versionProjects.ContainsKey(version))
            {
                versionProjects[version] = [];
            }

            versionProjects[version].Add(projectFile);
        }
    }
}

var fileName = $"DotNetProjectVersions-{DateTime.Now:yyyyMMdd}.txt";
var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
var filePath = Path.Combine(userPath, "Downloads", fileName);
using var writer = new StreamWriter(filePath);

foreach (var (version, projects) in versionProjects)
{
    writer.WriteLine($"{version}");
    foreach (var project in projects)
    {
        writer.WriteLine($"\t{project}");
    }
}

Console.WriteLine($"Results written to: {filePath}");
Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");