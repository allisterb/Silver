namespace Silver.Notebooks;

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;

using Microsoft.Msagl.Drawing;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;

using AGL.Drawing.Gdi;
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

    private static PocketView CreateImgTag(Graph g, string id, int height, int width)
    {
        var imgdata = $"data:image/png;base64,{Convert.ToBase64String(Gdi.DrawPng(g))}";
        return (PocketView) img[id: id, src: imgdata, height: height, width: width]();
    }
}
