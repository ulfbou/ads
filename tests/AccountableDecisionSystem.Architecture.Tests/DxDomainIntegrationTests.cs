using System.Xml.Linq;
using Xunit;

namespace AccountableDecisionSystem.Architecture.Tests;

public sealed class DxDomainIntegrationTests
{
    [Fact]
    public void DomainProjectReferencesExactlyTheAdmittedRuntimeProjects()
    {
        var root = RepositoryRoot();
        var project = XDocument.Load(Path.Combine(
            root,
            "src",
            "AccountableDecisionSystem.Domain",
            "AccountableDecisionSystem.Domain.csproj"));
        var references = project
            .Descendants("ProjectReference")
            .Select(element => ((string?)element.Attribute("Include"))?.Replace('\\', '/'))
            .Where(path => path is not null)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[]
            {
                "../../external/dx-domain/src/Dx.Domain.Annotations/Dx.Domain.Annotations.csproj",
                "../../external/dx-domain/src/Dx.Domain.Kernel/Dx.Domain.Kernel.csproj",
                "../../external/dx-domain/src/Dx.Domain.Primitives/Dx.Domain.Primitives.csproj"
            },
            references);
    }

    [Fact]
    public void SubmoduleMetadataUsesTheCanonicalPathAndUrl()
    {
        var lines = File.ReadAllLines(Path.Combine(RepositoryRoot(), ".gitmodules"));
        Assert.Contains("[submodule \"external/dx-domain\"]", lines);
        Assert.Contains("\tpath = external/dx-domain", lines);
        Assert.Contains("\turl = https://github.com/ulfbou/Dx.Domain.git", lines);
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AccountableDecisionSystem.slnx")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return directory!.FullName;
    }
}
