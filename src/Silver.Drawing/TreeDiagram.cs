﻿namespace Silver.Drawing;

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

    public TreeNode AddNode(string data, Dictionary<string, string>? attributes = null, List<TreeNode>? children = null)
    {
        var tn = new TreeNode(data, attributes, children);
        Children.Add(tn);
        return tn;
    }
    public string DrawWithJSTree()
    {
        var html = new StringBuilder();
        string a = (Attributes.Count > 0) ? " " + Attributes.Select(kv => kv.Key + "=" + "'" + kv.Value + "'").Aggregate((a, b) => a + " " + b) : "";
        html.AppendLine($"<li{a}>");
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

    public TreeDiagram(params TreeNode[] treeNodes)
    {
        TreeNodes = treeNodes.ToList();
    }

    public string DrawWithJSTree(string id)
    {
        var html = new StringBuilder();
        html.AppendLine($"<div id=\"{id}\">");
        html.AppendLine("<ul>");
        foreach(var node in TreeNodes)
        {
            html.AppendLine(node.DrawWithJSTree());
        }
        html.AppendLine("</ul>");
        html.AppendLine("</div>");
        //html.AppendLine($"<script>$('#{id}').jstree();</script>");
        return html.ToString();
    }
}

