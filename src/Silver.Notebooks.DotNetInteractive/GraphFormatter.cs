namespace Silver.Notebooks;

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;


using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Drawing;
using AGL.Drawing.Gdi;

using Node = Microsoft.Msagl.Drawing.Node;
using Edge = Microsoft.Msagl.Drawing.Edge;
using GeometryNode = Microsoft.Msagl.Core.Layout.Node;
using GeometryEdge = Microsoft.Msagl.Core.Layout.Edge;
using GeometryPoint = Microsoft.Msagl.Core.Geometry.Point;

public class GraphFormatter
{
    public static void Register()
    {
        Formatter.Register((Graph g, FormatContext context) =>
        {
            var id = Guid.NewGuid().ToString("N");
            var imgTag = CreateImgTag(g, id, 1000, 1000);
            context.Writer.Write(imgTag);
            return true;
        }, HtmlFormatter.MimeType);

        Kernel.Current.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added formatter for AGL graphs to .NET Interactive kernel {Kernel.Current.Name}.")));
    }

    public static SugiyamaLayoutSettings GetSugiyamaLayout(int minNodeWidth = 2000, int minNodeHeight = 1000, double rotateBy = 0.0)
    {
        SugiyamaLayoutSettings sugiyamaSettings = new SugiyamaLayoutSettings
        {
            Transformation = PlaneTransformation.Rotation(rotateBy),
            EdgeRoutingSettings = { EdgeRoutingMode = EdgeRoutingMode.SugiyamaSplines },
            MinNodeHeight = minNodeHeight,
            MinNodeWidth = minNodeWidth

        };
        return sugiyamaSettings;
    }

    public static string Draw(Graph graph, int width = 2000, int height = 2000, double rotateBy = 0.0)
    {
        graph.GeometryGraph = new GeometryGraph();
        var layout = GetSugiyamaLayout(5, 10, rotateBy);
        graph.LayoutAlgorithmSettings = layout;
        int nodeWidth = graph.Nodes.Max(n => n.LabelText.Length) * 9;
        foreach (Node n in graph.Nodes)
        {
            var gn = new GeometryNode(CurveFactory.CreateRectangle(nodeWidth, 40, new GeometryPoint()), n);
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
        new LayeredLayout(graph.GeometryGraph, layout).Run();

        using Bitmap bmp = new Bitmap(width, height);
        using Graphics g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.Clear(System.DrawingCore.Color.White);
        Rectangle rect = new Rectangle(0, 0, width, height);

        GdiUtils.SetGraphTransform(graph.GeometryGraph, rect, g);
        GdiUtils.Draw2(rect, graph, graph.GeometryGraph, g);
        using (MemoryStream ms = new MemoryStream())
        {
            bmp.Save(ms, ImageFormat.Png);
            return Convert.ToBase64String(ms.ToArray());
        }
    }
    private static PocketView CreateImgTag(Microsoft.Msagl.Drawing.Graph g, string id, int height, int width)
    {
        var imgdata = $"data:image/png;base64,{Draw(g, width, height)}";
        return (PocketView) img[id: id, src: imgdata, height: height, width: width]();
    }
}
