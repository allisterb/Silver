namespace Silver.Drawing;

using Microsoft.Msagl.Drawing;

using AGL.Drawing.Gdi;
public class Drawing : Runtime 
{
    public static void Draw(Graph graph, string filename, GraphFormat format, int width = 2000, int height = 2000, double rotateBy = 0.0)
    {
        WarnIfFileExists(filename);
        switch (format)
        {
            case GraphFormat.BMP:
                var bmp = Gdi.DrawBmp(graph, width, height, rotateBy);
                File.WriteAllBytes(filename, bmp);
                break;

            case GraphFormat.PNG:
                var png = Gdi.DrawBmp(graph, width, height, rotateBy);
                File.WriteAllBytes(filename, png);
                break;

            case GraphFormat.DOT:
                File.WriteAllText(filename, DOTWriter.Write(graph));
                break;

            case GraphFormat.DGML:
                using (var fdgml = new FileStream(filename, FileMode.Create))
                {
                    DGMLWriter.Write(fdgml, graph);
                }
                break;

            case GraphFormat.SVG:
                using (var fsvg = new FileStream(filename, FileMode.Create))
                {
                    var svgWriter = new SvgGraphWriter(fsvg, graph);
                    svgWriter.Write();
                }
                break;

            case GraphFormat.XML:
                using (var fxml = new FileStream(filename, FileMode.Create))
                {
                    var xmlWriter = new GraphWriter(fxml, graph);
                    xmlWriter.Write();
                }
                break;
        }
        Info("Saved graph to {0} file {1}.", format, filename);
    }
}
