namespace Silver.CodeAnalysis.IL;

using Backend.Analyses;
using Backend.Model;
using Backend.Serialization;
public partial class Analyzer
{
    public AnalyzerState GetControlFlowGraphs()
    {
        System.Action<IMethodDefinition, AnalyzerState> a = (m, state) =>
        {
            var mb = new Backend.Transformations.Disassembler(Host, m, PdbReader).Execute();
            var cfAnalysis = new ControlFlowAnalysis(mb);
            var cfg = cfAnalysis.GenerateNormalControlFlow();
            if (cfg is null)
            {
                Error("Could not get control-flow graph for method {0}.", m.Name);
            }
            else
            {
                state.Add(m.Name.Value, DOTSerializer.Serialize(cfg));
            }
        };
        return AnalyzeMethods(a);
    }
}

