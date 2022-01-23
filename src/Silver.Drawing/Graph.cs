namespace Silver.Drawing;

using System.DrawingCore;
using System.DrawingCore.Imaging;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Drawing;

using Node = Microsoft.Msagl.Drawing.Node;
using Edge = Microsoft.Msagl.Drawing.Edge;
using GeometryNode = Microsoft.Msagl.Core.Layout.Node;
using GeometryEdge = Microsoft.Msagl.Core.Layout.Edge;
using GeometryPoint = Microsoft.Msagl.Core.Geometry.Point;
using AGL.Drawing.Gdi;
public static class Graph 
{
    public static Microsoft.Msagl.Drawing.Graph FromDot(string dot) => Dot2Graph.Parser.Parse(dot.ToStream(), out var line, out var col, out var msg);

    public static void Draw(Microsoft.Msagl.Drawing.Graph graph, int width = 400, int height = 400)
    {
        graph.GeometryGraph = new GeometryGraph(); 
        var layout = GetSugiyamaLayout();
        graph.LayoutAlgorithmSettings = layout;

        foreach (Node n in graph.Nodes)
        {
            graph.GeometryGraph.Nodes.Add(new GeometryNode(CurveFactory.CreateRectangle(20, 10, new GeometryPoint()), n));
        }

        foreach (Edge e in graph.Edges)
        {
            GeometryNode source = graph.FindGeometryNode(e.SourceNode.Id);
            GeometryNode target = graph.FindGeometryNode(e.TargetNode.Id);
            graph.GeometryGraph.Edges.Add(new GeometryEdge(source, target));
        }
        graph.GeometryGraph.UpdateBoundingBox();
        using (Bitmap bmp = new Bitmap(width, height))
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(System.DrawingCore.Color.White);
            Rectangle rect = new Rectangle(0, 0, width, height);
            new LayeredLayout(graph.GeometryGraph, layout).Run();
            
            //GdiUtils.SetGraphTransform(graph, rect, g);

            GdiUtils.DrawFromGraph(rect, graph.GeometryGraph, g);
            bmp.Save("graph.bmp", ImageFormat.Bmp);
        }
    }

    public static SugiyamaLayoutSettings GetSugiyamaLayout(int minNodeHeight = 10, int minNodeWidth = 20)
    {
        SugiyamaLayoutSettings sugiyamaSettings = new SugiyamaLayoutSettings
        {
            Transformation = PlaneTransformation.Rotation(Math.PI / 2),
            EdgeRoutingSettings = { EdgeRoutingMode = EdgeRoutingMode.Spline },
            MinNodeHeight = minNodeHeight,
            MinNodeWidth = minNodeWidth

        };
        return sugiyamaSettings;
    }
}
