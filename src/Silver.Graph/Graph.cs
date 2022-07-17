namespace Silver;

using Satsuma;
using MsAglGraph =  Microsoft.Msagl.Drawing.Graph;
using InternalNode = Satsuma.Node;


public class Graph : IBuildableGraph, IDestroyableGraph, IGraph
{
    #region Types
    private class NodeAllocator : IdAllocator
	{
		public Graph? Parent;
		public NodeAllocator() : base() { }
		protected override bool IsAllocated(long id) { return Parent is not null && Parent.HasNode(new InternalNode(id)); }
	}

	private class ArcAllocator : IdAllocator
	{
		public Graph? Parent;
		public ArcAllocator() : base() { }
		protected override bool IsAllocated(long id) { return Parent is not null && Parent.HasArc(new Arc(id)); }
	}

	private class ArcProperties
	{
		public InternalNode U { get; private set; }
		public InternalNode V { get; private set; }
		public bool IsEdge { get; private set; }

		public ArcProperties(InternalNode u, InternalNode v, bool isEdge)
		{
			U = u;
			V = v;
			IsEdge = isEdge;
		}
	}
    #endregion

    #region Constructors
    public Graph()
	{
		this.graph = new();

		nodeAllocator = new NodeAllocator() { Parent = this };
		arcAllocator = new ArcAllocator() { Parent = this };

		nodes = new HashSet<InternalNode>();
		arcs = new HashSet<Arc>();
		arcProperties = new Dictionary<Arc, ArcProperties>();
		nodeMap = new Dictionary<long, string>();
		edges = new HashSet<Arc>();

		nodeArcs_All = new Dictionary<InternalNode, List<Arc>>();
		nodeArcs_Edge = new Dictionary<InternalNode, List<Arc>>();
		nodeArcs_Forward = new Dictionary<InternalNode, List<Arc>>();
		nodeArcs_Backward = new Dictionary<InternalNode, List<Arc>>();

		Kind = "";
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

	public InternalNode AddNode()
	{
		var id = nodeAllocator.Allocate();
		InternalNode InternalNode = new InternalNode(id);
		nodes.Add(InternalNode);
		nodeMap.Add(id, id.ToString());
		graph.AddNode(id.ToString());
		return InternalNode;
	}

	public InternalNode AddNode(InternalNode InternalNode)
	{
		if (nodes.Contains(InternalNode))
		{
			throw new ArgumentException($"This graph already contains the InternalNode {InternalNode.Id}");
		}
		nodes.Add(InternalNode);
		nodeMap.Add(InternalNode.Id, InternalNode.Id.ToString());
		graph.AddNode(InternalNode.Id.ToString());
		return InternalNode;
	}

	public Node AddNode(Node node)
	{
		nodes.Add(node.InternalNode);
		nodeMap.Add(node.Id.GetHashCode(), node.Id);
		graph.AddNode(node.MsAglNode);
		return node;
	}

	public Node AddNode(string id) => AddNode(new Node(id));

	public Node FindNode(string id) => HasNode(id) ? new Node(id, graph.FindNode(id), nodes.Single(n => n.Id == n.Id.GetHashCode())) : throw new ArgumentException($"The node {id} does not exist.");

	public Graph AddEdge(string src, string tgt, string? label = null)
	{
		var u = new InternalNode(src.GetHashCode());
		var v = new InternalNode(tgt.GetHashCode());
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

	public Graph AddEdge(Node u, Node v, string? label = null) => AddEdge(u.Id, v.Id, label);

	public Arc AddArc(InternalNode u, InternalNode v, Directedness directedness)
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

	public bool DeleteNode(InternalNode InternalNode)
	{
		if (!nodes.Remove(InternalNode)) return false;
		graph.RemoveNode(graph.FindNode(nodeMap[InternalNode.Id]));
		Func<Arc, bool> arcsToRemove = (a => (U(a) == InternalNode || V(a) == InternalNode));

		// remove arcs from nodeArcs_... of other ends of the arcs going from "InternalNode"
		foreach (InternalNode otherNode in Nodes())
		{
			if (otherNode != InternalNode)
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

		nodeArcs_All.Remove(InternalNode);
		nodeArcs_Edge.Remove(InternalNode);
		nodeArcs_Forward.Remove(InternalNode);
		nodeArcs_Backward.Remove(InternalNode);
		nodeMap.Remove(InternalNode.Id);
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

	public InternalNode U(Arc arc) => arcProperties[arc].U;

	public InternalNode V(Arc arc) => arcProperties[arc].V;

	public bool IsEdge(Arc arc) => arcProperties.ContainsKey(arc) && arcProperties[arc].IsEdge;

	private HashSet<Arc> ArcsInternal(ArcFilter filter)
	{
		return filter == ArcFilter.All ? arcs : edges;
	}

	private static readonly List<Arc> EmptyArcList = new List<Arc>();
	private List<Arc> ArcsInternal(InternalNode v, ArcFilter filter)
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

	public IEnumerable<InternalNode> Nodes() => Nodes();

	public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All) => ArcsInternal(filter);

	public IEnumerable<Arc> Arcs(InternalNode u, ArcFilter filter = ArcFilter.All) => ArcsInternal(u, filter);

	public IEnumerable<Arc> Arcs(InternalNode u, InternalNode v, ArcFilter filter = ArcFilter.All)
	{
		foreach (var arc in ArcsInternal(u, filter))
			if (this.Other(arc, u) == v) yield return arc;
	}

	public int NodeCount() => nodes.Count;

	public int ArcCount(ArcFilter filter = ArcFilter.All) => ArcsInternal(filter).Count;

	public int ArcCount(InternalNode u, ArcFilter filter = ArcFilter.All) => ArcsInternal(u, filter).Count;

	public int ArcCount(InternalNode u, InternalNode v, ArcFilter filter = ArcFilter.All)
	{
		int result = 0;
		foreach (var arc in ArcsInternal(u, filter))
			if (this.Other(arc, u) == v) result++;
		return result;
	}

	public bool HasNode(InternalNode InternalNode) => nodes.Contains(InternalNode);

	public bool HasNode(string id) => nodes.Contains(new InternalNode(id.GetHashCode()));

	public bool HasNode(Node node) => HasNode(node.Id);
	public bool HasArc(Arc arc) => arcs.Contains(arc);
    #endregion

    #region Properties
    public string Kind { get; set; }

	internal MsAglGraph MsAglGraph
    {
		get => graph;
    }
	#endregion

	#region Fields
	private MsAglGraph graph;

	private NodeAllocator nodeAllocator;
	private ArcAllocator arcAllocator;

	private HashSet<InternalNode> nodes;
	private HashSet<Arc> arcs;
	private Dictionary<Arc, ArcProperties> arcProperties;
	private Dictionary<long, string> nodeMap;
	private HashSet<Arc> edges;

	private Dictionary<InternalNode, List<Arc>> nodeArcs_All;
	private Dictionary<InternalNode, List<Arc>> nodeArcs_Edge;
	private Dictionary<InternalNode, List<Arc>> nodeArcs_Forward;
	private Dictionary<InternalNode, List<Arc>> nodeArcs_Backward;
	#endregion

}

