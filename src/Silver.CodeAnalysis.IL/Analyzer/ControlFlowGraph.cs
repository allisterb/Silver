namespace Silver.CodeAnalysis.IL;

using Backend.Analyses;
public partial class Analyzer
{
    public static void GetControlFlowGraph(string fileName)
    {
        var analyzer = new Analyzer(FailIfFileNotFound(fileName));
        System.Action<IMethodDefinition, AnalyzerState> a = (methodDefinition, state) =>
        {
            var m = new Backend.Transformations.Disassembler(analyzer.Host, methodDefinition, analyzer.PdbReader).Execute();
            var cfAnalysis = new ControlFlowAnalysis(m);
            var cfg = cfAnalysis.GenerateNormalControlFlow();
            
        };
        analyzer.AnalyzeMethods(a);
    }
}

