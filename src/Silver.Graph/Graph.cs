namespace Silver.Graph;

using Satsuma;
using MsAglGraph=  Microsoft.Msagl.Drawing.Graph;
public class Graph : IGraph
{
    public Graph()
    {
        MsAglGraph = new MsAglGraph();
    }

    public MsAglGraph MsAglGraph { get; protected set; }

    public Graph AddNode(string id)
    {

        nodes.Add(id, new Node(nodes.Count));
        MsAglGraph.AddNode(id);
        return this;
    }

    public Graph AddEdge(string src, string tgt)
    {
        if  (!nodes.Any(n => n.Key == src))
        {
            throw new ArgumentException($"The node {src} does not exist in this graph.");
        }
        if (!nodes.Any(n => n.Key == tgt))
        {
            throw new ArgumentException($"The node {tgt} does not exist in this graph.");
        }
        var nsrc = nodes.Single(n => n.Key == src);
        var ntgt = nodes.Single(n => n.Key == tgt);
        arcs.Add(new Arc());
        //new Arc().
        var e = MsAglGraph.AddEdge(src, tgt);
        var a = new Arc();
        a.
        return this;
    }

    public IEnumerable<Node> Nodes() => nodes.Values;

    public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
    {
        switch (filter)
        {
            case ArcFilter.All: 
                return MsAglGraph
        }
    }
    protected Dictionary<string, Node> nodes = new Dictionary<string, Node>();
    //protected Dictionary<Ed, Arc)>()
    protected List<Arc> arcs = new List<Arc>();
}
