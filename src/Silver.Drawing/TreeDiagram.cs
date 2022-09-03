namespace Silver.Drawing;

using System.Collections.Generic;
using System.Linq;

public class TreeNode
{
    public string Data { get; set; }

    public Dictionary<string, string> Attributes { get; set; }

    public List<TreeNode> Children { get; set; }

    public TreeNode(string data, Dictionary<string, string>? attributes =  null, List<TreeNode>? children = null)
    {
        Data = data;
        Attributes = attributes ?? new Dictionary<string, string>();
        Children = children ?? new List<TreeNode>();
    }
}

public class TreeDiagram
{
    public List<TreeNode> TreeNodes { get; set;}

    public TreeDiagram(IEnumerable<TreeNode> treeNodes)
    {
        TreeNodes = treeNodes.ToList();
    }

    public string DrawWithJSTree()
    {

    }
}

