namespace Silver.CodeAnalysis.IL;

using Backend.Analyses;
using Backend.Model;

using Microsoft.Msagl.Drawing;

using Silver.Metadata;

#region Records
public record Summary(
    ITypeDefinition[] Classes, ITypeDefinition[] Structs, ITypeDefinition[] Enums, IMethodDefinition[] Methods,
    IPropertyDefinition[] Properties, IFieldDefinition[] Fields);
#endregion

public partial class Analyzer : Runtime
{
    #region Constructors
    public Analyzer(string fileName, AnalyzerState? state = null, bool loadrefs = false)
    {
        AssemblyFile = new FileInfo(FailIfFileNotFound(fileName));
        Host = new PeReader.DefaultHost();
        Module = (IModule) this.Host.LoadUnitFrom(fileName);
        State = state ?? new();
        if (Module is null || Module == Dummy.Module || Module == Dummy.Assembly)
        {
            moduleTypeDefinitions = Array.Empty<INamedTypeDefinition>();
            Error("The file {0} is not a valid CLR module or assembly.", fileName);
            return;
        }
        var pdbFileName = Path.ChangeExtension(fileName, "pdb");
        if (loadrefs)
        {
            var refs = Module.AssemblyReferences;
            foreach (var r in refs)
            {
                var a = AssemblyMetadata.TryResolve(r, AssemblyFile.DirectoryName!);
                if (a is not null)
                {
                    Host.LoadUnitFrom(a.File.FullName);
                    Info("Resolved assembly reference {0} to {1}.", r.Name.Value, a.File.FullName);
                }
                else
                {
                    Warn("Did not resolve assembly reference {0}.", r.Name.Value);
                }
            }
        }
        
        Types.Initialize(Host);
        if (File.Exists(pdbFileName))
        {
            PdbReader = new PdbReader(fileName, pdbFileName, this.Host, true);
        }
        var _moduleTypeDefinitions = from a in Host.LoadedUnits.OfType<IModule>()
                                from t in a.GetAllTypes()
                                select t;
        moduleTypeDefinitions = _moduleTypeDefinitions.ToArray();
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
        var classes = CollectClasses();
        var methods = CollectMethods();
        var structs = CollectStructs();
        var enums = CollectEnums();
        var properties = CollectProperties();
        var fields = CollectFields();	
        return new(classes, structs, enums, methods, properties, fields);
    }

    public Graph GetCallGraph()
    {
        FailIfNotInitialized();
        if (State.ContainsKey("cg"))
        {
            Info("Using cached call graph.");
            return State.Get<Graph>("cg");
        }
        using var op = Begin("Creating call graph for methods in assembly {0}", AssemblyFile.Name);
        var methods = CollectMethods();
        var cha = new ClassHierarchyCallGraphAnalysis(Host);
        cha.OnNewMethodFound = (m) =>
        {
            if (PdbReader is not null && m.Locations.Any() && PdbReader.GetPrimarySourceLocationsFor(m.Locations).Any())
            {
                Warn("The method {0} at location(s) {1} was not analyzed.", m.GetUniqueName(), PdbReader.GetPrimarySourceLocationsFor(m.Locations).Select(l => PrintLocation(l)));
            }
            else
            {
                Warn("The method {0} was not analyzed.", m.GetUniqueName());
            }
            return false;            
        };
        var cg = cha.Analyze(methods);
        var g = new Graph();
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
            foreach (var cs in calllsites)
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
        op.Complete();
        return g;
    }

    public Graph GetControlFlowGraph()
    {
        FailIfNotInitialized();
        if (State.ContainsKey("cfg"))
        {
            Info("Using cached control-flow graphs.");
            return State.Get<Graph>("cfg");
        }
        using var op = Begin("Creating control-flow graph for methods in assembly {0}", AssemblyFile.Name);
        var methods = CollectMethods();
        Graph g = new Graph();
        
        g.Kind = "cfg";
        
        var sourceEmitterOutput = new MonochromeSourceEmitterOutput();
        foreach (var method in methods)
        {
            var cfg = new ControlFlowAnalysis(MethodBodyProvider.Instance.GetBody(method)).GenerateNormalControlFlow();
            foreach (var cfgNode in cfg.Nodes)
            {
                var nid = method.GetUniqueId(cfgNode.Id);
                var node = g.Nodes.FirstOrDefault(n => n.Id == nid);
                if (node is null)
                {
                    switch (cfgNode.Kind)
                    {
                        case CFGNodeKind.Exit:
                            break;
                        default:
                            node = new Node(nid);
                            node.LabelText = PrintCFGNodeLabel(method, cfgNode, sourceEmitterOutput);
                            if (method.IsSmartContractMethod() && cfgNode.Kind == CFGNodeKind.Entry)
                            {
                                node.Attr.FillColor = Color.Yellow;
                            }
                            else
                            {
                                node.Attr.FillColor = Color.White;
                            }
                            g.AddNode(node);
                            break;
                    }
                }
            }
            foreach (var cfgNode in cfg.Nodes)
            {
                var node = g.FindNode(method.GetUniqueId(cfgNode.Id));
                foreach (var successor in cfgNode.Successors)
                {
                    if (successor.Kind == CFGNodeKind.Exit) continue;
                    var snid = method.GetUniqueId(successor.Id);
                    var snode = g.FindNode(snid);
                    g.AddEdge(node.Id, snode.Id);
                }
            }
            //File.WriteAllText(Path.Combine(AssemblyFile.DirectoryName!, method.Name.Value), SerializeCFGToDGML(cfg));
            Info("Created CFG nodes for method {0}.", method.GetUniqueName());
        }
        op.Complete();
        return g;
    }

    #region Collecters
    internal ITypeDefinition[] CollectTypes(string name, Func<ITypeDefinition, bool> pred, Func<ITypeDefinition, ITypeDefinition> func)
    {
        if (State.ContainsKey(name.ToLower()))
        {
            Info("{0} are already collected, reusing...", name);
            return State.Get<ITypeDefinition[]>(name.ToLower());
        }
        else
        {
            var data = moduleTypeDefinitions.Where(t => pred(t)).Select(t => func(t)).ToArray();
            State.Add(name.ToLower(), data);
            Debug("Collected {0} type(s).", data.Length);
            return data;
        }
    }

    internal ITypeDefinition[] CollectTypes(string name, Func<ITypeDefinition, bool> pred) => CollectTypes(name, pred, Identity<ITypeDefinition>());

    internal ITypeDefinition[] CollectTypes(string name, Func<ITypeDefinition, ITypeDefinition> func) => CollectTypes(name, All<ITypeDefinition>(), func);

    internal ITypeDefinition[] CollectTypes(string name) => CollectTypes(name, All<ITypeDefinition>(), Identity<ITypeDefinition>());

    internal T[] CollectMembers<T>(string name, Func<T, bool> pred, Func<T, T> func) where T : ITypeDefinitionMember
    {
        if (State.ContainsKey(name.ToLower()))
        {
            Info("{0} are already collected, reusing...", name);
            return State.Get<T[]>(name.ToLower());
        }
        else
        {
            var _data = from t in moduleTypeDefinitions
                        from d in t.Members.OfType<T>()
                        where d is not null && pred(d)
                        select func(d);
            var data = _data.ToArray();
            State.Add(name.ToLower(), data);
            Debug("Collected {0} member(s).", data.Length);
            return data;
        }
    }

    internal T[] CollectMembers<T>(string name, Func<T, bool> pred) where T : ITypeDefinitionMember => CollectMembers<T>(name, pred, Identity<T>());

    internal T[] CollectMembers<T>(string name, Func<T, T> func) where T : ITypeDefinitionMember  => CollectMembers<T>(name, All<T>(), func);

    internal T[] CollectMembers<T>(string name) where T : ITypeDefinitionMember => CollectMembers<T>(name, All<T>(), Identity<T>());

    internal ITypeDefinition[] CollectClasses() => CollectTypes("Classes", (t => t.IsClass && t.GetName() != "<Module>")).ToArray();

    internal ITypeDefinition[] CollectStructs() => CollectTypes("Structs", (t => t.IsStruct)).ToArray();

    internal ITypeDefinition[] CollectEnums() => CollectTypes("Enums", (t => t.IsEnum)).ToArray();

    internal IMethodDefinition[] CollectMethods() => CollectMembers<IMethodDefinition>("Methods", m =>
    {
        var disassembler = new Backend.Transformations.Disassembler(Host, m, PdbReader);
        MethodBodyProvider.Instance.AddBody(m, disassembler.Execute());
        return m;
    });

    internal IFieldDefinition[] CollectFields() => CollectMembers<IFieldDefinition>("Fields");

    internal IPropertyDefinition[] CollectProperties() => CollectMembers<IPropertyDefinition>("Properties");
    #endregion

    internal AnalyzerState AnalyzeMethods(System.Action<IMethodDefinition, AnalyzerState> action)
    {
        FailIfNotInitialized();
        var visitor = new MethodVisitor(action, State);
        visitor.Traverse(Module);
        return visitor.state;
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

            //var dot = DOTSerializer.Serialize(cfg);
            //var dgml = DGMLSerializer.Serialize(cfg)
        };
        analyzer.AnalyzeMethods(f);
        Info("State:{0}", analyzer.State.Keys.JoinWithSpaces());
    }

    #region Printers
    protected string PrintCFGNodeLabel(IMethodDefinition method, CFGNode node, MonochromeSourceEmitterOutput iloutput)
    {
        switch (node.Kind)
        {
            case CFGNodeKind.Entry:
                var name = method.GetUniqueName();
                var loc = this.PdbReader is not null && method.Locations is not null && method.Locations.Any() && PdbReader.GetPrimarySourceLocationsFor(method.Locations.First()).Any() ?
                    Environment.NewLine + "Location: " + PrintSourceLocation(PdbReader.GetPrimarySourceLocationsFor(method.Locations.First())) : "";
                return name + loc;

            case CFGNodeKind.Exit:
                return method.GetUniqueName() + "::" + "return";

            default:
                var il = node.Instructions.SelectMany(i => i.GetILFromBody(method.Body));
                iloutput.ClearData();
                PrintOperations(il, iloutput);
                return string.Format("Node ID: {0}{1}{2}", method.GetUniqueId(node.Id), Environment.NewLine, iloutput.Data);
        }
    }

    private void PrintOperations(IEnumerable<IOperation> operations, MonochromeSourceEmitterOutput sourceEmitterOutput)
    {
        sourceEmitterOutput.ClearData();
        foreach (var operation in operations)
        {
            sourceEmitterOutput.Write("IL_" + operation.Offset.ToString("x4") + ": ", true);
            sourceEmitterOutput.Write(operation.OperationCode.ToString());
            if (operation.Value is string)
                sourceEmitterOutput.Write(" \"" + operation.Value + "\"");
            else if (operation.Value is not null)
            {
                if (OperationCode.Br_S <= operation.OperationCode && operation.OperationCode <= OperationCode.Blt_Un)
                    sourceEmitterOutput.Write(" IL_" + ((uint)operation.Value).ToString("x4"));
                else if (operation.OperationCode == OperationCode.Switch)
                {
                    foreach (uint i in (uint[])operation.Value)
                        sourceEmitterOutput.Write(" IL_" + i.ToString("x4"));
                }

                else if (operation.OperationCode == OperationCode.Call || operation.OperationCode == OperationCode.Callvirt)
                {
                    var fullNamect = operation.Value.ToString()!;
                    var ct = fullNamect.SkipLast(1);
                    var namep = fullNamect.Split('.').Last().Split('(');
                    var name = namep.First();
                    var p = "(" + namep.Last();
                    if (name.StartsWith("get_"))
                    {
                        sourceEmitterOutput.Write(" " + "[get] " + ct + "." + name.Replace("get_", ""));

                    }
                    else if (name.StartsWith("set_"))
                    {
                        sourceEmitterOutput.Write(" " + "[set] " + ct + "." + name.Replace("set_", ""));

                    }
                    else
                    {
                        sourceEmitterOutput.Write(" " + "[method] " + fullNamect);

                    }
                }
                else
                {
                    var vt = operation.Value.GetType().Name;
                    if (vt.EndsWith("FieldDefinition") || vt.EndsWith("FieldReference"))
                    {
                        sourceEmitterOutput.Write(" [field] " + operation.Value.ToString());
                    }
                    else
                    {
                        sourceEmitterOutput.Write(" " + operation.Value);
                        //sourceEmitterOutput.Write(" " + operation.Value.GetType().FullName);
                    }
                }
            }
            sourceEmitterOutput.WriteLine("", false);
        }
    }

    private string PrintLocation(ILocation l)
    {
        string location = "";

        if (this.PdbReader != null)
        {
            foreach (IPrimarySourceLocation psloc in this.PdbReader.GetPrimarySourceLocationsFor(l))
            {
                if (psloc.Source.Length > 0)
                {
                    location = psloc.Source;
                    break;
                }
            }
        }
        return location;
    }

    private string PrintSourceLocation(IPrimarySourceLocation psloc) =>
        (psloc.Document.Name.Value + "(" + psloc.StartLine + ":" + psloc.StartColumn + ")-(" + psloc.EndLine + ":" + psloc.EndColumn + ")");

    private string PrintSourceLocation(IEnumerable<IPrimarySourceLocation> psloc) =>
        (psloc.First().Document.Name.Value + "(" + psloc.First().StartLine + ":" + psloc.First().StartColumn + ")-(" + psloc.Last().EndLine + ":" + psloc.Last().EndColumn + ")");

    public static string SerializeCFGToDot(ControlFlowGraph g) => Backend.Serialization.DOTSerializer.Serialize(g);

    public static string SerializeCFGToDGML(ControlFlowGraph g) => Backend.Serialization.DGMLSerializer.Serialize(g);
    #endregion

    #endregion

    #region Fields
    internal IEnumerable<INamedTypeDefinition> moduleTypeDefinitions;
    public static HashSet<System.Reflection.Assembly> systemAssemblies = new HashSet<System.Reflection.Assembly>
    {
        System.Reflection.Assembly.Load("System.Runtime"),
        typeof(object).Assembly,
        typeof(Enumerable).Assembly,
    };
    #endregion
}

