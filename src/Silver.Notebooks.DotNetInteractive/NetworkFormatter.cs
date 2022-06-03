namespace Silver.Notebooks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.DotNet.Interactive.Http;

using Newtonsoft.Json;

using Silver.Drawing.VisJS;
public class NetworkFormatter : Runtime
{
    private static void AppendJsCode(StringBuilder stringBuilder, string divId, string functionName, Uri libraryUri, string? libraryVersion, string cacheBuster, Network data)
    {
        libraryVersion ??= "0.0.0";
        stringBuilder.AppendLine($@"{functionName} = () => {{");
        var libraryAbsoluteUri = Regex.Replace(libraryUri.AbsoluteUri, @"(\.js)$", string.Empty);
        cacheBuster ??= Guid.NewGuid().ToString("N");
        stringBuilder.AppendLine($@"(require.config({{ 'paths': {{ 'context': '{libraryVersion}', 'visjs' : '{libraryAbsoluteUri}', 'urlArgs': 'cacheBuster={cacheBuster}' }}}}) || require)(['visjs'], (visjs) => {{");
        stringBuilder.AppendLine($@"
        let container = document.getElementById('{divId}');
        let data = {{
                        nodes: new visjs.DataSet({JsonConvert.SerializeObject(data.Nodes)}), 
                        edges: new visjs.DataSet({JsonConvert.SerializeObject(data.Edges)})
                    }};
        let options = {JsonConvert.SerializeObject(data.Options)};
        let network = new visjs.Network(container, data, options); 
        }},
        (error) => {{
            console.log(error);
        }});
        }}");
    }
    private static IHtmlContent GenerateHtml(Network network, Uri libraryUri, string? libraryVersion, string cacheBuster, string width, string height)
    {
        var requireUri = new Uri("https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
        var divId = Guid.NewGuid().ToString("N");
        var code = new StringBuilder();
        var functionName = $"loadVisjs_{divId}";
        code.AppendLine("<div>");

        code.AppendLine(@"<script type=""text/javascript"">");
        AppendJsCode(code, divId, functionName, libraryUri, libraryVersion, cacheBuster, network);
        code.AppendLine(JavascriptUtilities.GetCodeForEnsureRequireJs(requireUri, functionName));
        code.AppendLine("</script>");

        code.AppendLine($"<div id=\"{divId}\" style=\"height:{height}; width:{width}\"></div>");
        code.AppendLine("</div>");

        var html = new HtmlString(code.ToString());
        return html;
    }
    public static void Register()
    {
        var cacheBuster = Guid.NewGuid().ToString("N");
        Formatter.Register<Network>((network, writer) =>
        {
            var html = GenerateHtml(network, new Uri("https://visjs.github.io/vis-network/standalone/umd/vis-network.min.js", UriKind.Absolute),
                "0.0.0", cacheBuster, network.Width, network.Height);
            Debug("Generated HTML code {0} for DOT code {1} of width {2} and height {3}.", html, JsonConvert.SerializeObject(network), network.Width, network.Height);
            html.WriteTo(writer, HtmlEncoder.Default);
        }, HtmlFormatter.MimeType);
        Kernel.Current.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added formatter for VisJS networks to .NET Interactive kernel {Kernel.Current.Name}.")));
    }
}

