namespace Silver.Drawing;

using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public string DrawWithJSTree()
    {
        var html = new StringBuilder();
        html.AppendLine("<li>");
        html.AppendLine(Data);
        if (Children.Count > 0)
        {
            html.AppendLine("<ul>");
            foreach (var child in Children)
            {
                html.AppendLine(child.DrawWithJSTree());
            }
            html.AppendLine("</ul>");
        }

        html.AppendLine("</li>");
        return html.ToString(); 
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
        var html = new StringBuilder();
        html.AppendLine("<div id=\"\">");
        html.AppendLine("<ul>");
        foreach(var node in TreeNodes)
        {
            html.AppendLine(node.DrawWithJSTree());
        }
        html.AppendLine("</ul>");
        html.AppendLine("</div>");
        return html.ToString();
    }
}

