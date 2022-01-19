namespace Silver.CodeAnalysis.IL;

using Backend.Analyses;

public enum Analysis
{
	Cfg
}
public partial class Analyzer : Runtime
{
    #region Constructors
    public Analyzer(string fileName, bool all = false, AnalyzerState? state = null)
	{
		FileName = fileName;
		Host = new PeReader.DefaultHost();
		Module = this.Host.LoadUnitFrom(fileName) as IModule;
		State = state ?? new();
		State.AddIfNotExists("all", all);
		if (Module is null || Module == Dummy.Module || Module == Dummy.Assembly)
		{
			Error("The file {0} is not a valid CLR module or assembly.", fileName);
			return;
		}
		var pdbFileName = Path.ChangeExtension(fileName, "pdb");

		if (File.Exists(pdbFileName))
		{
			PdbReader = new PdbReader(fileName, pdbFileName, this.Host, true);
		}
		Types.Initialize(Host);
		Initialized = true;
	}
    #endregion

    #region Properties
    public string FileName { get; init; }
	public IMetadataHost Host { get; init; }
	public IModule? Module { get; init; }
	public PdbReader? PdbReader { get; init; }
	public AnalyzerState State { get; init; }
	#endregion

	#region Methods
	public (int, int) GetSummary()
    {
		FailIfNotInitialized();
		var visitor = new SummaryVisitor(this.State);
		visitor.Traverse(Module);
		return ((int)State["types"], (int)State["methods"]);
    }
	public AnalyzerState AnalyzeMethods(System.Action<IMethodDefinition, AnalyzerState> action)
	{
		FailIfNotInitialized();
		var visitor = new MethodVisitor(action, State);
		visitor.Traverse(Module);
		return State;
	}

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

			////var dot = DOTSerializer.Serialize(cfg);
			//var dgml = DGMLSerializer.Serialize(cfg)
		};
		analyzer.AnalyzeMethods(f);
		Info("State:{0}", analyzer.State.Keys.JoinWithSpaces());
	}
	#endregion
}

