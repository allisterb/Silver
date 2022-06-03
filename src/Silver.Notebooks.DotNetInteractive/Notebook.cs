namespace Silver.Notebooks;

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Msagl.Drawing;

using Silver.Drawing.VisJS;

public class Notebook
{
    public Network? Draw(Graph graph)
    {
        NetworkOptions options = new NetworkOptions();
        List<NetworkNode> nodes = new();
        foreach (var node in graph.Nodes)
        {
            //nodes.Add(new NetworkNode() {Id = node.Id, h })
        }
        return null;
    }
}

