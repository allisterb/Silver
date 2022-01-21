namespace Silver.Drawing;

using System.DrawingCore;
using System.DrawingCore.Imaging;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Drawing;
using AGL.Drawing.Gdi;
public static class Graph 
{
    public static Microsoft.Msagl.Drawing.Graph FromDot(string dot) => Dot2Graph.Parser.Parse(dot.ToStream(), out var line, out var col, out var msg);

    public static void ToBmp(Microsoft.Msagl.Drawing.Graph graph)
    {
        using (Bitmap bmp = new Bitmap(400, 400))
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(System.DrawingCore.Color.White);
            Rectangle rect = new Rectangle(0, 0, 400, 400);
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

    public static DrawGraph(Microsoft.Msagl.Drawing.Graph graph)
    {

    }
}
