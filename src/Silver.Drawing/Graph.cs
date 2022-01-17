namespace Silver.Drawing;

using System.Drawing;
using Microsoft.Msagl;
using Microsoft.Msagl.Drawing;
public class Graph : Runtime
{
    public Microsoft.Msagl.Drawing.Graph FromDot(string dot) => Dot2Graph.Parser.Parse(dot.ToStream(), out var line, out var col, out var msg);

    public void ToBmp(Microsoft.Msagl.Drawing.Graph g)
    {
        //var renderer = new Microsoft.Msagl.Miscellaneous..GraphRenderer(graph);
    }
}
