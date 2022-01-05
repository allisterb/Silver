namespace Silver.SpecSharp.Tests;

using System.IO;
using Xunit;

public class ProjectFileTests
{
    [Fact]
    public void CanReadXmlProjectFile()
    {
        
        var path = Path.Combine(Runtime.AssemblyLocation, "..", "..", "..", "..", "files", "Test1.sscproj");
        Assert.True(File.Exists(path));
        var proj = new XmlSpecSharpProject(path);
        Assert.NotNull(proj);
        Assert.False(proj.Initialized);
        path = Path.Combine(Runtime.AssemblyLocation, "..", "..", "..", "..", "files", "Test2.sscproj");
        Assert.True(File.Exists(path));
        var proj2 = new XmlSpecSharpProject(path);
        Assert.NotNull(proj2);
    }

    [Fact]
    public void CanReadCsProjectFile()
    {
        Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
        var path = Path.Combine(Runtime.AssemblyLocation, "..", "..", "..", "..", "files", "Test3.csproj");
        Assert.True(File.Exists(path));
        var proj = new CsSpecSharpProject(path);
    }
}
