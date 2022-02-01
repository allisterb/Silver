namespace Silver.CodeAnalysis.IL;

using Backend.Analyses;
using Backend.Model;

using Microsoft.Msagl.Drawing;

#region Records
public record Summary(
	List<ITypeDefinition> Types, List<ITypeDefinition> Structs, List<ITypeDefinition> Enums, List<IMethodDefinition> Methods,
	List<IPropertyDefinition> Properties, List<IFieldDefinition> Fields);
#endregion

public partial class Analyzer : Runtime
{
    #region Constructors
    public Analyzer(string fileName, bool all = false, AnalyzerState? state = null)
	{
		AssemblyFile = new FileInfo(fileName);
		Host = new PeReader.DefaultHost();
		Module = this.Host.LoadUnitFrom(fileName) as IModule;
		State = state ?? new();
		if (Module is null || Module == Dummy.Module || Module == Dummy.Assembly)
		{
			moduleTypeDefinitions = Array.Empty<INamedTypeDefinition>();
			Error("The file {0} is not a valid CLR module or assembly.", fileName);
			return;
		}
		State.AddIfNotExists("all", all);
		var pdbFileName = Path.ChangeExtension(fileName, "pdb");
		Types.Initialize(Host);
		if (File.Exists(pdbFileName))
		{
			PdbReader = new PdbReader(fileName, pdbFileName, this.Host, true);
		}
		moduleTypeDefinitions = from a in Host.LoadedUnits.OfType<IModule>()
									from t in a.GetAllTypes()
									select t;
			//from m in t.Members.OfType<IMethodDefinition>()
			//where m.Body != null
			//select m;
		Initialized = true;
	}
    #endregion

    #region Properties
    public FileInfo AssemblyFile { get; init; }
	
	public IMetadataHost Host { get; init; }
	public IModule? Module { get; init; }
	public PdbReader? PdbReader { get; init; }
	public AnalyzerState State { get; init; }
	#endregion

	#region Methods
	public Summary GetSummary()
    {
		FailIfNotInitialized();
		var summary = new SummaryVisitor(this.State);
		summary.Traverse(Module);
		return new(summary.types, summary.structs, summary.enums, summary.methods, summary.properties, summary.fields);
    }

	public Graph? GetCallGraph()
    {
		FailIfNotInitialized();
		using var op = Begin("Creating call graph for methods in assembly {0}", AssemblyFile.Name);
		var cha = new ClassHierarchyCallGraphAnalysis(Host);
		cha.OnNewMethodFound = (m =>
		{
			var disassembler = new Backend.Transformations.Disassembler(Host, m, PdbReader);
			var methodBody = disassembler.Execute();
			MethodBodyProvider.Instance.AddBody(m, methodBody);
			return true;
		});
		var methods = CollectMethods();
		var cg = cha.Analyze();
		var g = new Graph();
		if (g is null)
		{
			Error("Could not create graph drawing object.");
			return null;
		}
		foreach (var method in cg.Roots)
        {
			Node? rootNode = null;
			var calllsites = cg.GetCallSites(method);
			var inv = cg.GetInvocations(method);
			var nid = MemberHelper.GetMethodSignature(method);
			if (!g.Nodes.Any(n => n.Id == nid))
            {
				rootNode = g.AddNode(nid);
			}
			else
            {
				rootNode = g.FindNode(nid);
            }
			foreach(var cs in calllsites)
            {
				Node? csNode = null;
				var csid = MemberHelper.GetMethodSignature(cs.Caller);
				if (!g.Nodes.Any(n => nid == csid))
                {
					csNode = g.AddNode(csid);
				}
				else
                {
					csNode = g.FindNode(csid);
                }

				g.AddEdge(csNode.Id, rootNode.Id);
            }
		}
		return g;
    }
	public void GetControlFlowGraph()
	{
		FailIfNotInitialized();
		using var op = Begin("Creating control-flow graph for methods in assembly {0}", AssemblyFile.Name);
		var methods = CollectMethods();
		foreach(var method in methods)
        {
			var disassembler = new Backend.Transformations.Disassembler(Host, method, PdbReader);
			var methodBody = disassembler.Execute();
			var cfg = new ControlFlowAnalysis(methodBody).GenerateNormalControlFlow();
			File.WriteAllText(Path.Combine(AssemblyFile.DirectoryName!, method.Name.Value), SerializeCFGToDGML(cfg));
			Info("Wrote CFG for method {0}.", method.Name);
			op.Complete();
		}
    }
    internal AnalyzerState AnalyzeMethods(System.Action<IMethodDefinition, AnalyzerState> action)
	{
		FailIfNotInitialized();
		var visitor = new MethodVisitor(action, State);
		visitor.Traverse(Module);
		return visitor.state;
	}

	internal IEnumerable<IMethodDefinition> CollectMethods()
    {
		if (State.ContainsKey("methods"))
        {
			Debug("Methods are already collected, reusing...");
			return State.Get<IEnumerable<IMethodDefinition>>("methods");
        }
		else
        {
			using var op = Begin("Collecting methods");
			var methods = from t in moduleTypeDefinitions
						  from m in t.Members.AsParallel().OfType<IMethodDefinition>()
						  where m.Body is not null
						  select m;
			State.Add("methods", methods);
			op.Complete();
			return methods;
		}
	}

	public static string SerializeCFGToDot(ControlFlowGraph g) => Backend.Serialization.DOTSerializer.Serialize(g);

	public static string SerializeCFGToDGML(ControlFlowGraph g) => Backend.Serialization.DGMLSerializer.Serialize(g);

	public static void Test(string fileName)
    {
		var analyzer = new Analyzer(fileName);
		System.Action<IMethodDefinition, AnalyzerState> f = (methodDefinition, state) =>
		{

			var signature = MemberHelper.GetMethodSignature(methodDefinition, NameFormattingOptions.Signature | NameFormattingOptions.ParameterName);
			state.Add(methodDefinition.Name.Value, signature + Environment.NewLine);

			if (methodDefinition.IsAbstract || methodDefinition.IsExternal) return;

			var disassembler = new Backend.Transformations.Disassembler(analyzer.Host, methodDefinition, analyzer.PdbReader);
			var methodBody = disassembler.Execute();

			//System.Console.WriteLine(methodBody);
			//System.Console.WriteLine();

			var cfAnalysis = new ControlFlowAnalysis(methodBody);
			var cfg = cfAnalysis.GenerateNormalControlFlow();
			//var cfg = cfAnalysis.GenerateExceptionalControlFlow();

			var domAnalysis = new DominanceAnalysis(cfg);
			domAnalysis.Analyze();
			domAnalysis.GenerateDominanceTree();

			var loopAnalysis = new NaturalLoopAnalysis(cfg);
			loopAnalysis.Analyze();

			var domFrontierAnalysis = new DominanceFrontierAnalysis(cfg);
			domFrontierAnalysis.Analyze();

			var splitter = new WebAnalysis(cfg, methodDefinition);
			splitter.Analyze();
			splitter.Transform();
			methodBody.UpdateVariables();

			var typeAnalysis = new TypeInferenceAnalysis(cfg, methodDefinition.Type);
			typeAnalysis.Analyze();

			var forwardCopyAnalysis = new ForwardCopyPropagationAnalysis(cfg);
			forwardCopyAnalysis.Analyze();
			forwardCopyAnalysis.Transform(methodBody);

			var backwardCopyAnalysis = new BackwardCopyPropagationAnalysis(cfg);
			backwardCopyAnalysis.Analyze();
			backwardCopyAnalysis.Transform(methodBody);

			//var pointsTo = new PointsToAnalysis(cfg);
			//var result = pointsTo.Analyze();

			var liveVariables = new LiveVariablesAnalysis(cfg);
			liveVariables.Analyze();
			//var ssa = new StaticSingleAssignment(methodBody, cfg);
			//ssa.Transform();
			//ssa.Prune(liveVariables);

			methodBody.UpdateVariables();

			//var dot = DOTSerializer.Serialize(cfg);
			//var dgml = DGMLSerializer.Serialize(cfg)
		};
		analyzer.AnalyzeMethods(f);
		Info("State:{0}", analyzer.State.Keys.JoinWithSpaces());
	}
	#endregion

	#region Fields
	internal IEnumerable<INamedTypeDefinition> moduleTypeDefinitions;
	#endregion
}

