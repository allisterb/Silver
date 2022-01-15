namespace Silver.CodeAnalysis.IL
{

	using Microsoft.Cci;
	using Backend;
	using Backend.Analyses;
	using Backend.Serialization;
	using Backend.Transformations;
	using Backend.Utils;


	public class Class1
    {
		

		public void CanAnalyzeClassHierarchy()
		{

			const string root = @"..\..\..";
			//const string root = @"C:"; // casa
			//const string root = @"C:\Users\Edgar\Projects"; // facu

			const string input = @"C:\Projects\Silver\tests\TestProjects\Silver.CodeAnalysis.IL.TestProject1\bin\Debug\net6.0\Silver.CodeAnalysis.IL.TestProject1.dll";

			using (var host = new PeReader.DefaultHost())
			using (var assembly = new Console2.Assembly(host))
			{
				assembly.Load(input);

				Types.Initialize(host);

				var visitor = new Console2.MethodVisitor(host, assembly.PdbReader);
				visitor.Traverse(assembly.Module);
			}

			System.Console.WriteLine("Done!");

			/*
			var host = new PeReader.DefaultHost();

			var path = Path.Combine(Runtime.AssemblyLocation, @"C:\Projects\Silver\tests\TestProjects\Silver.CodeAnalysis.IL.TestProject1\bin\Debug\net6.0\Silver.CodeAnalysis.IL.TestProject1.dll");
		
			//host.Assemblies.Add(assembly);

			PlatformTypes.Resolve(host);

			var loader = new Loader(host);
			loader.LoadAssembly(path);
			VisitMethods();
			//host.
			//loader.LoadCoreAssembly();
			//VisitMethods();
			var type = new BasicType("ExamplesPointsTo")
			{
				ContainingAssembly = new AssemblyReference("Silver.CodeAnalysis.IL.TestProject1"),
				ContainingNamespace = "Test"
			};

			var typeDefinition = host.ResolveReference(type);

			var method = new MethodReference("Example1", PlatformTypes.Void)
			{
				ContainingType = type,
			};
			*/

		}
		
		/*
		private void VisitMethods()
		{

			var allDefinedMethods = from a in host.Assemblies
									from t in a.RootNamespace.GetAllTypes()
									from m in t.Members.OfType<MethodDefinition>()
									where m.HasBody
									select m;

			foreach (var method in allDefinedMethods)
			{
				VisitMethod(method);
			}

		}

		private void VisitMethod(MethodDefinition method)
		{
			System.Console.WriteLine(method.ToSignatureString());

			var methodBodyBytecode = method.Body;
			var disassembler = new Disassembler(method);
			var methodBody = disassembler.Execute();
			method.Body = methodBody;

			var cfAnalysis = new ControlFlowAnalysis(method.Body);
			//var cfg = cfAnalysis.GenerateNormalControlFlow();
			var cfg = cfAnalysis.GenerateExceptionalControlFlow();

			var dgml_CFG = DGMLSerializer.Serialize(cfg);

			var domAnalysis = new DominanceAnalysis(cfg);
			domAnalysis.Analyze();
			domAnalysis.GenerateDominanceTree();

			var loopAnalysis = new NaturalLoopAnalysis(cfg);
			loopAnalysis.Analyze();

			var domFrontierAnalysis = new DominanceFrontierAnalysis(cfg);
			domFrontierAnalysis.Analyze();

			var splitter = new WebAnalysis(cfg);
			splitter.Analyze();
			splitter.Transform();

			methodBody.UpdateVariables();

			var typeAnalysis = new TypeInferenceAnalysis(cfg, method.ReturnType);
			typeAnalysis.Analyze();

			// Copy Propagation
			var forwardCopyAnalysis = new ForwardCopyPropagationAnalysis(cfg);
			forwardCopyAnalysis.Analyze();
			forwardCopyAnalysis.Transform(methodBody);

			var backwardCopyAnalysis = new BackwardCopyPropagationAnalysis(cfg);
			backwardCopyAnalysis.Analyze();
			backwardCopyAnalysis.Transform(methodBody);

			// Points-To
			var pointsTo = new PointsToAnalysis(cfg, method);
			var result = pointsTo.Analyze();

			var ptg = result[cfg.Exit.Id].Output;
			//ptg.RemoveVariablesExceptParameters();
			ptg.RemoveTemporalVariables();

			var dgml_PTG = DGMLSerializer.Serialize(ptg);

			// Live Variables
			var liveVariables = new LiveVariablesAnalysis(cfg);
			var livenessInfo = liveVariables.Analyze();

			// SSA
			var ssa = new StaticSingleAssignment(methodBody, cfg);
			ssa.Transform();
			ssa.Prune(livenessInfo);

			methodBody.UpdateVariables();

			//var dot = DOTSerializer.Serialize(cfg);
			//var dgml = DGMLSerializer.Serialize(cfg);

			//dgml = DGMLSerializer.Serialize(host, typeDefinition);
		}
		*/
	}
}