namespace Silver.CodeAnalysis.IL;

public class Visitor : MetadataTraverser 
{
    #region Constructors
    public Visitor(AnalyzerState? state = null) : base()
	{
		
		this.state = state ?? new();
	}
    #endregion

    #region Fields
    public Dictionary<string, object> state; 
    #endregion
}

public class MethodVisitor : Visitor
{
    #region Constructors
    public MethodVisitor(System.Action<IMethodDefinition, AnalyzerState> action, AnalyzerState state) :
		base(state)
    {
        this.action = action;
    }
    #endregion

    #region Overriden members
    public override void TraverseChildren(ITypeDefinition typeDefinition)
    {
        if (typeDefinition.IsSmartContract() || (bool) state["all"])
        {
            base.TraverseChildren(typeDefinition);
        }
        else
        {
            Runtime.Debug("Not traversing non-contract type {0}.", TypeHelper.GetTypeName(typeDefinition.ResolvedType));
        }
    }

    public override void TraverseChildren(IMethodDefinition methodBody)
    {
        Runtime.Debug("Entering method {0}.", methodBody.Name);
        action(methodBody, state);
        base.TraverseChildren(methodBody);
    }
    #endregion

    #region Fields
    System.Action<IMethodDefinition, AnalyzerState> action;
    #endregion
}

public class SummaryVisitor : Visitor
{
    #region Constructors
    public SummaryVisitor(AnalyzerState? state = null) :
        base(state)
    {
        this.state.Add("summary", new Dictionary<string, object>());
        this.state.Add("types", new List<ITypeDefinition>());
        this.state.Add("structs", new List<ITypeDefinition>());
        this.state.Add("methods", new List<IMethodDefinition>()); 
    }
    #endregion

    #region Overriden members
    public override void TraverseChildren(ITypeDefinition typeDefinition)
    {
        if (typeDefinition.IsSmartContract() || typeDefinition.IsStruct || typeDefinition.IsEnum || (bool) state["all"])
        {
            types.Add(typeDefinition);
            if (typeDefinition.IsStruct) structs.Add(typeDefinition); 
            base.TraverseChildren(typeDefinition);
        }
        else
        {
            Runtime.Debug("Not traversing non-contract type {0}", TypeHelper.GetTypeName(typeDefinition.ResolvedType));
        }
    }

    public override void TraverseChildren(IMethodDefinition m) => methods.Add(m);
    #endregion

    #region Fields
    public List<ITypeDefinition> types = new();
    public List<ITypeDefinition> structs = new();
    public List<IMethodDefinition> methods = new();
    #endregion
}


