namespace Silver.Projects.Tests;

using System.IO;
using Xunit;


public class ProjectFileTests
{
    [Fact]
    public void CanReadXmlProjectFile()
    {
        
        var path = Path.Combine(Runtime.AssemblyLocation, "..", "..", "..", "..", "files", "Test1.sscproj");
        Assert.True(File.Exists(path));
        var proj = new XmlSilverProject(path, "Debug");
        Assert.NotNull(proj);
        Assert.False(proj.Initialized);
        path = Path.Combine(Runtime.AssemblyLocation, "..", "..", "..", "..", "files", "Test2.sscproj");
        Assert.True(File.Exists(path));
        proj = new XmlSilverProject(path, "Debug");
        var path2 = Path.Combine(Runtime.AssemblyLocation, "..", "..", "..", "..", "..", "ext", "specsharp", "Boogie", "Core", "Core.sscproj");
        Assert.True(File.Exists(path2));
        proj = new XmlSilverProject(path2, "Debug");
        Assert.True(proj.Initialized);
    }

    [Fact]
    public void CanReadMSBuildProjectFile()
    {
        var path = Path.Combine(Runtime.AssemblyLocation, "..", "..", "..", "..", "files", "Test2.csproj");
        Assert.True(File.Exists(path));
        var proj = new MSBuildSilverProject(path, "Debug");
        Assert.False(proj.Initialized);
        path = Path.Combine(Runtime.AssemblyLocation, "..", "..", "..", "..", "Silver.SpecSharp.Tests", "Silver.SpecSharp.Tests.csproj");
        Assert.True(File.Exists(path));
        proj = new MSBuildSilverProject(path, "Debug");
        Assert.True(proj.Initialized);
    }
}
