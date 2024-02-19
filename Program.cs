using System.Diagnostics;
using System.Text.RegularExpressions;

var sw = Stopwatch.StartNew();
var repositoryDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".."));
Console.WriteLine("Searching directory: " + repositoryDirectory);

var searchConfigs = new Dictionary<string, string>
{
    { "*.csproj", "<TargetFramework>(.*?)</TargetFramework>" }
};

var versionProjects = new Dictionary<string, List<string>>();
foreach (var (projectSearchPattern, versionRegex) in searchConfigs)
{
    var projectFiles = Directory.GetFiles(repositoryDirectory, projectSearchPattern, SearchOption.AllDirectories);

    foreach (var projectFile in projectFiles)
    {
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
    Console.WriteLine($"{version}");
    writer.WriteLine($"{version}");
    foreach (var project in projects)
    {
        Console.WriteLine($"\t{project}");
        writer.WriteLine($"\t{project}");
    }
}

Console.WriteLine($"Results written to: {filePath}");
Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");
