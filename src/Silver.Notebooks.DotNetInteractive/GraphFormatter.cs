namespace Silver.Notebooks;

using System;
using System.Text.Encodings.Web;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using static Microsoft.DotNet.Interactive.Formatting.PocketViewTags;
using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;


using Silver.Drawing.VisJS;

public class GraphFormatter : Runtime
{
    public static void Register()
    {
        var cacheBuster = Guid.NewGuid().ToString("N");
        Formatter.Register<Graph>((graph, writer) =>
        {
            var network = VisJS.Draw(graph);
            var html = NetworkFormatter.GenerateHtml(network, new Uri("https://visjs.github.io/vis-network/standalone/umd/vis-network.min.js", UriKind.Absolute),
                "0.0.0", cacheBuster, network.Width, network.Height);
            Debug("Generated HTML code {0} for VisJS network {1} of width {2} and height {3}.", html, JsonConvert.SerializeObject(network), network.Width, network.Height);
            html.WriteTo(writer, HtmlEncoder.Default);
        }, HtmlFormatter.MimeType);

        Kernel.Current.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added formatter for AGL graphs using VisJS to .NET Interactive kernel {Kernel.Current.Name}.")));
    }

    /*
    private static PocketView CreateImgTag(Graph g, string id, int height, int width)
    {
        var imgdata = $"data:image/png;base64,{Convert.ToBase64String(Gdi.DrawPng(g))}";
        return (PocketView) img[id: id, src: imgdata, height: height, width: width]();
    }
    */
}
