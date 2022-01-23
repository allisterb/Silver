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
    public static void Draw(Microsoft.Msagl.Drawing.Graph graph, int width = 1000, int height = 1000)
    {
        //Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph();
        graph.GeometryGraph = new GeometryGraph(); 
        var layout = GetSugiyamaLayout();
        graph.LayoutAlgorithmSettings = layout;
          foreach (Node n in graph.Nodes)
        {
            var gn = new GeometryNode(CurveFactory.CreateRectangle(200, 40, new GeometryPoint()), n);
            graph.GeometryGraph.Nodes.Add(gn);
            n.GeometryNode = gn;
        }
        //graph.AddEdge(graph.Nodes.First().Id, "foo", graph.Nodes.Last().Id);


        GeometryNode source = graph.FindGeometryNode(graph.Nodes.First().Id);
        GeometryNode target = graph.FindGeometryNode(graph.Nodes.Last().Id);
        var e = new GeometryEdge(source, target);
        e.UserData = graph.Edges.First();
        graph.Edges.First().GeometryEdge = e;
        //e.Label = new Microsoft.Msagl.Core.Layout.Label() ;
        graph.GeometryGraph.Edges.Add(e);
        
        graph.GeometryGraph.UpdateBoundingBox();
        //Microsoft.Msagl.Drawing.SvgGraphWriter.Write(graph, "graph.svg");


        using Bitmap bmp = new Bitmap(width, height);
        using Graphics g = Graphics.FromImage(bmp);
        g.Clear(System.DrawingCore.Color.White);
        
        Rectangle rect = new Rectangle(0, 0, width, height);
        new LayeredLayout(graph.GeometryGraph, layout).Run();
        GdiUtils.SetGraphTransform(graph.GeometryGraph, rect, g);
        GdiUtils.Draw2(rect, graph, graph.GeometryGraph, g);
        bmp.Save("graph.bmp", ImageFormat.Bmp);
        
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

    public static Microsoft.Msagl.Drawing.Graph FromDot(string dot) => Dot2Graph.Parser.Parse(dot.ToStream(), out var line, out var col, out var msg);
}
