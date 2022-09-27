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
    public void CanGetControlFlowGraph()
    {
        var an = IL.GetAnalyzer(@"..\..\..\..\..\examples\DAOContract\bin\Debug\netcoreapp2.1\DAOContract.dll");
        Assert.Null(an.GetCallGraph());
    }
}

