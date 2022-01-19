namespace Silver.Drawing;

using System.DrawingCore;
using System.DrawingCore.Imaging;
using Microsoft.Msagl;
using Microsoft.Msagl.Drawing;
using AGL.Drawing.Gdi;
public class Graph : Runtime
{
    public Microsoft.Msagl.Drawing.Graph FromDot(string dot) => Dot2Graph.Parser.Parse(dot.ToStream(), out var line, out var col, out var msg);

    public void ToBmp(Microsoft.Msagl.Drawing.Graph graph)
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
}
