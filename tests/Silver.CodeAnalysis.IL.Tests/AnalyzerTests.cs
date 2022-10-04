namespace Silver.CodeAnalysis.IL.Tests;

using Xunit;

using Silver.Notebooks;

public class Analyzer
{
    [Fact]
    public void CanGetAnalyzer()
    {
        var an = IL.GetAnalyzer(@"..\..\..\..\..\examples\DAOContract\bin\Debug\netcoreapp2.1\DAOContract.dll");
        Assert.Null(an);

    }

    [Fact]
    public void CanGetCallGraph()
    {
        var an = IL.GetAnalyzer(@"..\..\..\..\..\examples\DAOContract\bin\Debug\netcoreapp2.1\DAOContract.dll");
        Assert.NotNull(an);
        var g = an.GetCallGraph();
        Assert.NotNull(g);
    }

    [Fact]
    public void CanGetControlFlowGraph()
    {
        var an = IL.GetAnalyzer(@"..\..\..\..\..\examples\DAOContract\bin\Debug\netcoreapp2.1\DAOContract.dll");
        Assert.NotNull(an);
        var g = an.GetControlFlowGraph();
        Assert.NotNull(g);
    }
}

