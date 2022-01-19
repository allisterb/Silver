namespace Silver.CodeAnalysis.IL;

public class Visitor : MetadataTraverser 
{
    #region Constructors
    public Visitor(AnalyzerState? state = null) : base()
	{
		
		this.State = state ?? new();
	}
    #endregion

    #region Properties
    public Dictionary<string, object> State { get; init; }
	#endregion

    #region Fields
    
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
        if (typeDefinition.IsSmartContract() || (bool) State["all"])
        {
            base.TraverseChildren(typeDefinition);
        }
        else
        {
            Runtime.Debug("Not traversing non-contract type {0}", TypeHelper.GetTypeName(typeDefinition.ResolvedType));
        }
        
    }

    public override void TraverseChildren(IMethodDefinition methodBody)
    {
        Runtime.Debug("Entering method {0}.", methodBody.Name);
        action(methodBody, State);
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
        State.Add("types", 0);
        State.Add("methods", 0); 
    }
    #endregion

    #region Overriden members
   
    public override void TraverseChildren(ITypeDefinition typeDefinition)
    {
        if (typeDefinition.IsSmartContract() || (bool) State["all"])
        {
            
            State["types"] = (int) State["types"] + 1;
            base.TraverseChildren(typeDefinition);
        }
        else
        {
            Runtime.Debug("Not traversing non-contract type {0}", TypeHelper.GetTypeName(typeDefinition.ResolvedType));
        }

    }

    public override void TraverseChildren(IMethodDefinition m)
    {
        State["methods"] = (int)State["methods"] + 1;
    }
    #endregion
}


