namespace Silver;

using MsAglGraph = Microsoft.Msagl.Drawing.Graph;
using MsAglNode = Microsoft.Msagl.Drawing.Node;
using InternalNode = Satsuma.Node;

public struct Node
{
    public string Id;
    public MsAglNode MsAglNode;
    public InternalNode InternalNode;

    public Node(string id)
    {
        Id = id;
        MsAglNode = new MsAglNode(id);
        InternalNode = new InternalNode(id.GetHashCode());
    }

    public Node(string id, MsAglNode msAglNode, InternalNode internalNode)
    {
        Id=id;
        MsAglNode=msAglNode;
        InternalNode=internalNode;
    }

    public Microsoft.Msagl.Drawing.NodeAttr Attr => MsAglNode.Attr;

    public void SetLabel(string text) => MsAglNode.LabelText = text;
}

