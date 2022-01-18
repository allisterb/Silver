using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.CodeAnalysis.IL
{
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
        public override void TraverseChildren(IMethodDefinition methodBody)
        {
            action(methodBody, state);
        }
        #endregion

        #region Fields
        AnalyzerState state;
        System.Action<IMethodDefinition, AnalyzerState> action;
        #endregion
    }

}
