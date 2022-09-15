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

using Silver.Drawing;
public class TreeDiagramFormatter : Runtime
{
    public static void Register()
    {
        Formatter.Register<TreeDiagram>((t, writer) =>
        {
            if (!JSLibs.JQueryLoaded)
            {
                //var html = new HtmlString("<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script>");
                //html.WriteTo(writer, HtmlEncoder.Default);
                
                var html = new HtmlString(JavascriptUtilities.GetCodeForEnsureRequireJs(new Uri("https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js")));
                html.WriteTo(writer, HtmlEncoder.Default);
                JSLibs.JQueryLoaded = true;
            }
            if (!JSLibs.JSTreeLoaded)
            {
                //var html = new HtmlString("<script src=\"https://cdnjs.cloudflare.com/ajax/libs/jstree/3.2.1/jstree.min.js\"></script>");
                //html.WriteTo(writer, HtmlEncoder.Default);
                var html = new HtmlString(JavascriptUtilities.GetCodeForEnsureRequireJs(new Uri("https://cdnjs.cloudflare.com/ajax/libs/jstree/3.2.1/jstree.min.js")));
                html.WriteTo(writer, HtmlEncoder.Default);
                JSLibs.JSTreeLoaded = true;
            }
            if (!Css.JSTreeLoaded)
            {
                var html = new HtmlString("<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/jstree/3.2.1/themes/default/style.min.css\" />");
                html.WriteTo(writer, HtmlEncoder.Default);
                Css.JSTreeLoaded = true;
            }

            var h = new HtmlString(t.DrawWithJSTree());
            h.WriteTo(writer, HtmlEncoder.Default);

            Kernel.Current.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added formatter for jsTree diagrams to .NET Interactive kernel {Kernel.Current.Name}.")));

        }, HtmlFormatter.MimeType);
    }
}