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

namespace Silver.Notebooks
{
    public class MermaidFormatter : Runtime
    {
        private static void AppendJsCode(StringBuilder stringBuilder, string divId, string functionName, Uri? libraryUri, string? libraryVersion, string? cacheBuster, string markdown)
        {
            libraryVersion ??= "1.0.0";
            stringBuilder.AppendLine($@"{functionName} = () => {{");
            if (libraryUri is not null)
            {
                var libraryAbsoluteUri = Regex.Replace(libraryUri.AbsoluteUri, @"(\.js)$", string.Empty);
                cacheBuster ??= Guid.NewGuid().ToString("N");
                stringBuilder.AppendLine($@" 
        (require.config({{ 'paths': {{ 'context': '{libraryVersion}', 'mermaidUri' : '{libraryAbsoluteUri}', 'urlArgs': 'cacheBuster={cacheBuster}' }}}}) || require)(['mermaidUri'], (mermaid) => {{");
            }
            else
            {
                stringBuilder.AppendLine($@"
        configureRequireFromExtension('Mermaid','{libraryVersion}')(['Mermaid/mermaid.min'], (mermaid) => {{");
            }

            stringBuilder.AppendLine($@"
            let renderTarget = document.getElementById('{divId}');
            mermaid.mermaidAPI.render( 
                'mermaid_{divId}', 
                `{markdown}`, 
                g => {{
                    renderTarget.innerHTML = g 
                }});
        }},
        (error) => {{
            console.log(error);
        }});
}}");
        }
        
        private static IHtmlContent GenerateHtml(MermaidLanguage markdown, Uri? libraryUri, string? libraryVersion, string? cacheBuster)
        {
            var requireUri = new Uri("https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
            var divId = Guid.NewGuid().ToString("N");
            var code = new StringBuilder();
            var functionName = $"loadMermaid_{divId}";
            code.AppendLine($"<div style=\"background-color:{markdown.Background}\">");

            code.AppendLine(@"<script type=""text/javascript"">");
            AppendJsCode(code, divId, functionName, libraryUri, libraryVersion, cacheBuster, markdown.ToString());
            code.AppendLine(JavascriptUtilities.GetCodeForEnsureRequireJs(requireUri, functionName));
            code.AppendLine("</script>");
            var style = string.Empty;
            if (!string.IsNullOrWhiteSpace(markdown.Width) || !string.IsNullOrWhiteSpace(markdown.Width))
            {
                style = " style=\"";

                if (!string.IsNullOrWhiteSpace(markdown.Width))
                {
                    style += $" width:{markdown.Width}; ";
                }

                if (!string.IsNullOrWhiteSpace(markdown.Height))
                {
                    style += $" height:{markdown.Height}; ";
                }

                style += "\" ";
            }
            code.AppendLine($"<div id=\"{divId}\"{style}></div>");
            code.AppendLine("</div>");

            var html = new HtmlString(code.ToString());
            return html;
        }

        public static void Register()
        {
            var cacheBuster = Guid.NewGuid().ToString("N");
            Formatter.Register<MermaidLanguage>((markdown, writer) =>
            {
                var html = GenerateHtml(markdown, new Uri(@"https://cdn.jsdelivr.net/npm/mermaid@9.1.1/dist/mermaid.min.js", UriKind.Absolute),
                    "9.1.1",
                    cacheBuster);
                html.WriteTo(writer, HtmlEncoder.Default);
            }, HtmlFormatter.MimeType);
            Kernel.Current.SendAsync(
                new DisplayValue(new FormattedValue(
                    "text/markdown",
                    $"Added formatter for Mermaid lanfuage to .NET Interactive kernel {Kernel.Current.Name}.")));
        }
    }
}
