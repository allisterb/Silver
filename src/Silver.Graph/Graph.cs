namespace Silver;

using Satsuma;
using MsAglGraph =  Microsoft.Msagl.Drawing.Graph;

public class Graph : IBuildableGraph, IDestroyableGraph, IGraph
{
    #region Classes
    private class NodeAllocator : IdAllocator
	{
		public Graph? Parent;
		public NodeAllocator() : base() { }
		protected override bool IsAllocated(long id) { return Parent is not null && Parent.HasNode(new Node(id)); }
	}

	private class ArcAllocator : IdAllocator
	{
		public Graph? Parent;
		public ArcAllocator() : base() { }
		protected override bool IsAllocated(long id) { return Parent is not null && Parent.HasArc(new Arc(id)); }
	}

	private class ArcProperties
	{
		public Node U { get; private set; }
		public Node V { get; private set; }
		public bool IsEdge { get; private set; }

		public ArcProperties(Node u, Node v, bool isEdge)
		{
			U = u;
			V = v;
			IsEdge = isEdge;
		}
	}
    #endregion

    #region Fields
    private MsAglGraph graph;

	private NodeAllocator nodeAllocator;
	private ArcAllocator arcAllocator;

	private HashSet<Node> nodes;
	private HashSet<Arc> arcs;
	private Dictionary<Arc, ArcProperties> arcProperties;
	private Dictionary<long, string> nodeMap;
	private HashSet<Arc> edges;

	private Dictionary<Node, List<Arc>> nodeArcs_All;
	private Dictionary<Node, List<Arc>> nodeArcs_Edge;
	private Dictionary<Node, List<Arc>> nodeArcs_Forward;
	private Dictionary<Node, List<Arc>> nodeArcs_Backward;
    #endregion

    #region Constructors
    public Graph()
	{
		this.graph = new();

		nodeAllocator = new NodeAllocator() { Parent = this };
		arcAllocator = new ArcAllocator() { Parent = this };

		nodes = new HashSet<Node>();
		arcs = new HashSet<Arc>();
		arcProperties = new Dictionary<Arc, ArcProperties>();
		nodeMap = new Dictionary<long, string>();
		edges = new HashSet<Arc>();

		nodeArcs_All = new Dictionary<Node, List<Arc>>();
		nodeArcs_Edge = new Dictionary<Node, List<Arc>>();
		nodeArcs_Forward = new Dictionary<Node, List<Arc>>();
		nodeArcs_Backward = new Dictionary<Node, List<Arc>>();
	}
    #endregion

    #region Methods
    /// Deletes all nodes and arcs of the adaptor.
    public void Clear()
	{
		nodeAllocator.Rewind();
		arcAllocator.Rewind();

		nodes.Clear();
		arcs.Clear();
		arcProperties.Clear();
		edges.Clear();

		nodeArcs_All.Clear();
		nodeArcs_Edge.Clear();
		nodeArcs_Forward.Clear();
		nodeArcs_Backward.Clear();
	}

	public Node AddNode()
	{
		var id = nodeAllocator.Allocate();
		Node node = new Node(id);
		nodes.Add(node);
		nodeMap.Add(id, id.ToString());
		graph.AddNode(id.ToString());
		return node;
	}

	public Graph AddNode(string id)
	{
		Node node = new Node(id.GetHashCode());
		nodes.Add(node);
		nodeMap.Add(id.GetHashCode(), id);
		graph.AddNode(id);
		return this;
	}

	public Node AddNode(Node node)
	{
		if (nodes.Contains(node))
		{
			throw new ArgumentException($"This graph already contains the node {node.Id}");
		}
		nodes.Add(node);
		nodeMap.Add(node.Id, node.Id.ToString());
		graph.AddNode(node.Id.ToString());
		return node;
	}

	public Graph AddEdge(string src, string tgt, string? label = null)
	{
		var u = new Node(src.GetHashCode());
		var v = new Node(tgt.GetHashCode());
		if (!this.HasNode(u))
		{
			AddNode(u);
		}
		if (!this.HasNode(v))
		{
			AddNode(v);

		}
		AddArc(u, v, Directedness.Directed);
		if (label is null)
        {
			graph.AddEdge(src, tgt);
        }
		else
        {
			graph.AddEdge(src, label, tgt);
		}
		return this;
	}
	
	public Arc AddArc(Node u, Node v, Directedness directedness)
	{
		if (ArcCount() == int.MaxValue) throw new InvalidOperationException("Error: too many arcs!");
		Arc a = new Arc(arcAllocator.Allocate());
		arcs.Add(a);
		bool isEdge = (directedness == Directedness.Undirected);
		arcProperties[a] = new ArcProperties(u, v, isEdge);

		Utils.MakeEntry(nodeArcs_All, u).Add(a);
		Utils.MakeEntry(nodeArcs_Forward, u).Add(a);
		Utils.MakeEntry(nodeArcs_Backward, v).Add(a);

		if (isEdge)
		{
			edges.Add(a);
			Utils.MakeEntry(nodeArcs_Edge, u).Add(a);
		}

		if (v != u)
		{
			Utils.MakeEntry(nodeArcs_All, v).Add(a);
			if (isEdge)
			{
				Utils.MakeEntry(nodeArcs_Edge, v).Add(a);
				Utils.MakeEntry(nodeArcs_Forward, v).Add(a);
				Utils.MakeEntry(nodeArcs_Backward, u).Add(a);
			}
		}
		
		return a;
	}

	public bool DeleteNode(Node node)
	{
		if (!nodes.Remove(node)) return false;
		graph.RemoveNode(graph.FindNode(nodeMap[node.Id]));
		Func<Arc, bool> arcsToRemove = (a => (U(a) == node || V(a) == node));

		// remove arcs from nodeArcs_... of other ends of the arcs going from "node"
		foreach (Node otherNode in Nodes())
		{
			if (otherNode != node)
			{
				Utils.RemoveAll(nodeArcs_All[otherNode], arcsToRemove);
				Utils.RemoveAll(nodeArcs_Edge[otherNode], arcsToRemove);
				Utils.RemoveAll(nodeArcs_Forward[otherNode], arcsToRemove);
				Utils.RemoveAll(nodeArcs_Backward[otherNode], arcsToRemove);
			}
		}

		Utils.RemoveAll(arcs, arcsToRemove);
		Utils.RemoveAll(edges, arcsToRemove);
		Utils.RemoveAll(arcProperties, arcsToRemove);

		nodeArcs_All.Remove(node);
		nodeArcs_Edge.Remove(node);
		nodeArcs_Forward.Remove(node);
		nodeArcs_Backward.Remove(node);
		nodeMap.Remove(node.Id);
		return true;
	}

	public bool DeleteArc(Arc arc)
	{
		if (!arcs.Remove(arc)) return false;
		ArcProperties p = arcProperties[arc];
		arcProperties.Remove(arc);
		graph.RemoveEdge(graph.Edges.Single(e => e.Source == nodeMap[p.U.Id] && e.Target == nodeMap[p.V.Id]));

		Utils.RemoveLast(nodeArcs_All[p.U], arc);
		Utils.RemoveLast(nodeArcs_Forward[p.U], arc);
		Utils.RemoveLast(nodeArcs_Backward[p.V], arc);

		if (p.IsEdge)
		{
			edges.Remove(arc);
			Utils.RemoveLast(nodeArcs_Edge[p.U], arc);
		}

		if (p.V != p.U)
		{
			Utils.RemoveLast(nodeArcs_All[p.V], arc);
			if (p.IsEdge)
			{
				Utils.RemoveLast(nodeArcs_Edge[p.V], arc);
				Utils.RemoveLast(nodeArcs_Forward[p.V], arc);
				Utils.RemoveLast(nodeArcs_Backward[p.U], arc);
			}
		}

		return true;
	}

	public Node U(Arc arc) => arcProperties[arc].U;

	public Node V(Arc arc) => arcProperties[arc].V;

	public bool IsEdge(Arc arc) => arcProperties.ContainsKey(arc) && arcProperties[arc].IsEdge;

	private HashSet<Arc> ArcsInternal(ArcFilter filter)
	{
		return filter == ArcFilter.All ? arcs : edges;
	}

	private static readonly List<Arc> EmptyArcList = new List<Arc>();
	private List<Arc> ArcsInternal(Node v, ArcFilter filter)
	{
		List<Arc>? result;
		switch (filter)
		{
			case ArcFilter.All: nodeArcs_All.TryGetValue(v, out result); break;
			case ArcFilter.Edge: nodeArcs_Edge.TryGetValue(v, out result); break;
			case ArcFilter.Forward: nodeArcs_Forward.TryGetValue(v, out result); break;
			default: nodeArcs_Backward.TryGetValue(v, out result); break;
		}
		return result ?? EmptyArcList;
	}

	public IEnumerable<Node> Nodes() => Nodes();

	public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All) => ArcsInternal(filter);

	public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All) => ArcsInternal(u, filter);

	public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
	{
		foreach (var arc in ArcsInternal(u, filter))
			if (this.Other(arc, u) == v) yield return arc;
	}

	public int NodeCount() => nodes.Count;

	public int ArcCount(ArcFilter filter = ArcFilter.All) => ArcsInternal(filter).Count;

	public int ArcCount(Node u, ArcFilter filter = ArcFilter.All) => ArcsInternal(u, filter).Count;

	public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
	{
		int result = 0;
		foreach (var arc in ArcsInternal(u, filter))
			if (this.Other(arc, u) == v) result++;
		return result;
	}

	public bool HasNode(Node node) => nodes.Contains(node);

	public bool HasNode(string id) => nodes.Contains(new Node(id.GetHashCode()));

	public bool HasArc(Arc arc) => arcs.Contains(arc);
    #endregion

    #region Properties
    internal MsAglGraph MsAglGraph
    {
		get => graph;
    }
	#endregion
}

