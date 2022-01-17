using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CCIProvider;
using Model;
using Model.Types;
using Backend.Analyses;
using Backend.Serialization;
using Backend.Transformations;
using Backend.Utils;
using Model.ThreeAddressCode.Values;
using Backend.Model;
using Tac = Model.ThreeAddressCode.Instructions;
using Bytecode = Model.Bytecode;
using Xunit;

namespace Silver.CodeAnalysis.IL
{
    public class AssemblyTests
    {
		Host host = new();
		[Fact]
		public void CanAnalyzeClassHierarchy()
		{
			
			
			var path =  Path.Combine(Runtime.AssemblyLocation, "Silver.CodeAnalysis.IL.TestProject1.dll");
			Assert.True(File.Exists(path));	
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

			//var methodDefinition = host.ResolveReference(method) as MethodDefinition;


			

			// Testing method calls inlining
			var methodDefinition = host.ResolveReference(method) as MethodDefinition;
			var methodCalls = methodDefinition.Body.Instructions.OfType<Tac.MethodCallInstruction>().ToList();

			foreach (var methodCall in methodCalls)
			{
				var callee = host.ResolveReference(methodCall.Method) as MethodDefinition;
				methodDefinition.Body.Inline(methodCall, callee.Body);
			}

			methodDefinition.Body.UpdateVariables();

			type = new BasicType("ExamplesCallGraph")
			{
				ContainingAssembly = new AssemblyReference("Silver.CodeAnalysis.IL.TestProject1"),
				ContainingNamespace = "Test"
			};

			method = new MethodReference("Example1", PlatformTypes.Void)
			{
				ContainingType = type,
			};

			methodDefinition = host.ResolveReference(method) as MethodDefinition;

			var ch = new ClassHierarchy();
			ch.Analyze(host);

			var dgml = DGMLSerializer.Serialize(ch);

			var chcga = new ClassHierarchyAnalysis(ch);
			var roots = host.GetRootMethods();
			var cg = chcga.Analyze(host, roots);

			dgml = DGMLSerializer.Serialize(cg);
		}

		[Fact]
		public void CanEvaluateGenerics()
		{
			var path = Path.Combine(Runtime.AssemblyLocation, "Silver.CodeAnalysis.IL.TestProject1.dll");

			var host = new Host();

			PlatformTypes.Resolve(host);

			var loader = new Loader(host);
			loader.LoadAssembly(path);
			//loader.LoadCoreAssembly();

			var assembly = new AssemblyReference("Silver.CodeAnalysis.IL.TestProject1");

			var typeA = new GenericParameterReference(GenericParameterKind.Type, 0);
			var typeB = new GenericParameterReference(GenericParameterKind.Type, 1);

			var typeNestedClass = new BasicType("NestedClass")
			{
				ContainingAssembly = assembly,
				ContainingNamespace = "Test",
				GenericParameterCount = 2,
				ContainingType = new BasicType("ExamplesGenerics")
				{
					ContainingAssembly = assembly,
					ContainingNamespace = "Test",
					GenericParameterCount = 1
				}
			};

			//typeNestedClass.ContainingType.GenericArguments.Add(typeA);
			//typeNestedClass.GenericArguments.Add(typeB);

			var typeDefinition = host.ResolveReference(typeNestedClass);

			if (typeDefinition == null)
			{
				System.Console.WriteLine("[Error] Cannot resolve type:\n{0}", typeNestedClass);
			}

			var typeK = new GenericParameterReference(GenericParameterKind.Method, 0);
			var typeV = new GenericParameterReference(GenericParameterKind.Method, 1);

			var typeKeyValuePair = new BasicType("KeyValuePair")
			{
				ContainingAssembly = new AssemblyReference(typeof(System.Object).Assembly.GetName().Name),
				ContainingNamespace = "System.Collections.Generic",
				GenericParameterCount = 2
			};

			typeKeyValuePair.GenericArguments.Add(typeK);
			typeKeyValuePair.GenericArguments.Add(typeV);
			typeDefinition = host.ResolveReference(typeKeyValuePair);
			var methodExampleGenericMethod = new MethodReference("ExampleGenericMethod", typeKeyValuePair)
			{
				ContainingType = typeNestedClass,
				GenericParameterCount = 2
			};

			//methodExampleGenericMethod.GenericArguments.Add(typeK);
			//methodExampleGenericMethod.GenericArguments.Add(typeV);

			methodExampleGenericMethod.Parameters.Add(new MethodParameterReference(0, typeA));
			methodExampleGenericMethod.Parameters.Add(new MethodParameterReference(1, typeB));
			methodExampleGenericMethod.Parameters.Add(new MethodParameterReference(2, typeK));
			methodExampleGenericMethod.Parameters.Add(new MethodParameterReference(3, typeV));
			methodExampleGenericMethod.Parameters.Add(new MethodParameterReference(4, typeKeyValuePair));

			var methodDefinition = host.ResolveReference(methodExampleGenericMethod) as MethodDefinition;

			if (methodDefinition == null)
			{
				System.Console.WriteLine("[Error] Cannot resolve method:\n{0}", methodExampleGenericMethod);
			}

			var methodExample = new MethodReference("Example", PlatformTypes.Void)
			{
				ContainingType = new BasicType("ExamplesGenericReferences")
				{
					ContainingAssembly = assembly,
					ContainingNamespace = "Silver.CodeAnalysis.IL.TestProject1.Test"
				}
			};

			methodDefinition = host.ResolveReference(methodExample) as MethodDefinition;

			if (methodDefinition == null)
			{
				System.Console.WriteLine("[Error] Cannot resolve method:\n{0}", methodExample);
			}

			var calls = methodDefinition.Body.Instructions.OfType<Bytecode.MethodCallInstruction>();

			foreach (var call in calls)
			{
				methodDefinition = host.ResolveReference(call.Method) as MethodDefinition;

				if (methodDefinition == null)
				{
					System.Console.WriteLine("[Error] Cannot resolve method:\n{0}", call.Method);
				}
			}
		}

		[Fact]
		public void RunInterPointsToTests()
		{
			var path = Path.Combine(Runtime.AssemblyLocation, "Silver.CodeAnalysis.IL.TestProject1.dll");


			var host = new Host();

			PlatformTypes.Resolve(host);

			var loader = new Loader(host);
			loader.LoadAssembly(path);
			//loader.LoadCoreAssembly();

			var methodReference = new MethodReference("Example6", PlatformTypes.Void)
			//var methodReference = new MethodReference("Example6", PlatformTypes.Void)
			//var methodReference = new MethodReference("ExampleDelegateCaller", PlatformTypes.Void)
			{
				ContainingType = new BasicType("ExamplesPointsTo", TypeKind.ReferenceType)
				{
					ContainingAssembly = new AssemblyReference("Silver.CodeAnalysis.IL.TestProject1"),
					ContainingNamespace = "Test"
				}
			};

			//var parameter = new MethodParameterReference(0, PlatformTypes.Boolean);
			//methodReference.Parameters.Add(parameter);
			//parameter = new MethodParameterReference(1, PlatformTypes.Boolean);
			//methodReference.Parameters.Add(parameter);

			//methodReference.ReturnType = new BasicType("Node", TypeKind.ReferenceType)
			//{
			//	ContainingAssembly = new AssemblyReference("Test"),
			//	ContainingNamespace = "Test"
			//};

			methodReference.Resolve(host);

			var programInfo = new ProgramAnalysisInfo();
			var pta = new InterPointsToAnalysis(programInfo);

			var cg = pta.Analyze(methodReference.ResolvedMethod);
			var dgml_CG = DGMLSerializer.Serialize(cg);

			//System.IO.File.WriteAllText(@"cg.dgml", dgml_CG);

			var esca = new EscapeAnalysis(programInfo, cg);
			var escapeResult = esca.Analyze();

			var fea = new FieldEffectsAnalysis(programInfo, cg);
			var effectsResult = fea.Analyze();

			foreach (var method in cg.Methods)
			{
				MethodAnalysisInfo methodInfo;
				var ok = programInfo.TryGet(method, out methodInfo);
				if (!ok) continue;

				InterPointsToInfo pti;
				ok = methodInfo.TryGet(InterPointsToAnalysis.INFO_IPTA_RESULT, out pti);

				if (ok)
				{
					var ptg = pti.Output;
					ptg.RemoveTemporalVariables();
					//ptg.RemoveVariablesExceptParameters();
					var dgml_PTG = DGMLSerializer.Serialize(ptg);

					//System.IO.File.WriteAllText(@"ptg.dgml", dgml_PTG);
				}

				EscapeInfo escapeInfo;
				ok = escapeResult.TryGetValue(method, out escapeInfo);

				FieldEffectsInfo effectsInfo;
				ok = effectsResult.TryGetValue(method, out effectsInfo);
			}
		}
		#region Helpers
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

        #endregion


    }
}
