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

    [Fact]
    public void CanGetTACDisassembly()
    {
        //var an = IL.GetAnalyzer(@"..\..\..\..\..\examples\DAOContract\bin\Debug\netcoreapp2.1\DAOContract.dll");
        //Assert.NotNull(an);
        //var m = Silver.Notebooks.IL.DisassembleCode
        var code = @"
            public class TestProgram 
            {
                static bool A(int[] a)
                {
                  for (int i = 0; i < 25; i++)
                  {
                      if (i > 15)
                      {
                          return true;
                      }
                  }
                  return false;
                }
            }";
        var m = IL.DisassembleCode(code);
        Assert.NotNull(m);
    }
}

