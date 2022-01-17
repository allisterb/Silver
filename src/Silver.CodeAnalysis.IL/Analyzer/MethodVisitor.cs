using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.CodeAnalysis.IL
{
	public class MethodVisitor<T> : MetadataTraverser
	{
		private IMetadataHost host;
		private ISourceLocationProvider sourceLocationProvider;
		private System.Action<IMethodDefinition, T> action;
		private T State { get; init; }
		public MethodVisitor(IMetadataHost host, ISourceLocationProvider sourceLocationProvider, T state, System.Action<IMethodDefinition, T> action)
		{
			this.host = host;
			this.sourceLocationProvider = sourceLocationProvider;
			this.State = state;
			this.action = action;
		}

		public override void TraverseChildren(IMethodDefinition m) => action(m, State);
        
    }
}
