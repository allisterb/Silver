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
    public static void Draw(Microsoft.Msagl.Drawing.Graph graph, int width = 2000, int height = 1000)
    {
        graph.GeometryGraph = new GeometryGraph(); 
        var layout = GetSugiyamaLayout(5, 10);
        graph.LayoutAlgorithmSettings = layout;
        foreach (Node n in graph.Nodes)
        {
            var gn = new GeometryNode(CurveFactory.CreateRectangle(200, 40, new GeometryPoint()), n);
            graph.GeometryGraph.Nodes.Add(gn);
            n.GeometryNode = gn;
        }
        foreach (Edge edge in graph.Edges)
        {
            var sn = graph.FindGeometryNode(edge.Source);
            var tn = graph.FindGeometryNode(edge.Target);
            var ge = new GeometryEdge(sn, tn);
            ge.UserData = edge;
            ge.EdgeGeometry.TargetArrowhead = new Arrowhead();
            graph.GeometryGraph.Edges.Add(ge);
            edge.GeometryEdge = ge;
        }
        graph.GeometryGraph.UpdateBoundingBox();
        using Bitmap bmp = new Bitmap(width, height);
        using Graphics g = Graphics.FromImage(bmp);
        g.Clear(System.DrawingCore.Color.White);
        
        Rectangle rect = new Rectangle(0, 0, width, height);
        new LayeredLayout(graph.GeometryGraph, layout).Run();
        GdiUtils.SetGraphTransform(graph.GeometryGraph, rect, g);
        GdiUtils.Draw2(rect, graph, graph.GeometryGraph, g);
        bmp.Save("graph.bmp", ImageFormat.Bmp);
        
    }

    public static SugiyamaLayoutSettings GetSugiyamaLayout(int minNodeWidth = 20, int minNodeHeight = 10)
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

    public static Microsoft.Msagl.Drawing.Graph FromDot(string dot) => Dot2Graph.Parser.Parse(dot.ToStream(), out var line, out var col, out var msg);
}
