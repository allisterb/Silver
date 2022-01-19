namespace Silver.CodeAnalysis.IL;

public class Visitor : MetadataTraverser 
{
    #region Constructors
    public Visitor(IMetadataHost host, ISourceLocationProvider? sourceLocationProvider, AnalyzerState state) 
	{
		this.host = host;
		this.sourceLocationProvider = sourceLocationProvider;
		this.State = state ?? new();
	}
    #endregion

    #region Properties
    public Dictionary<string, object> State { get; init; }
	#endregion

    #region Fields
    private IMetadataHost host;
	private ISourceLocationProvider? sourceLocationProvider;
	#endregion
}

public class MethodVisitor : Visitor
{
    #region Constructors
    public MethodVisitor(IMetadataHost host, ISourceLocationProvider? sourceLocationProvider, System.Action<IMethodDefinition, AnalyzerState> action, AnalyzerState state) :
		base(host, sourceLocationProvider, state)
    {
        this.action = action;
        this.state = state;
    }
    #endregion

    #region Overriden members
    public override void TraverseChildren(ITypeDefinition typeDefinition)
    {
        if (typeDefinition.IsSmartContract())
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
        action(methodBody, state);
    }
    #endregion

    #region Fields
    AnalyzerState state;
    System.Action<IMethodDefinition, AnalyzerState> action;
    #endregion
}


