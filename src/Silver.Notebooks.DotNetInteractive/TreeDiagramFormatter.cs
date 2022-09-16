﻿namespace Silver.Notebooks;

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
            if (!Resources.JSTreeLibLoaded)
            {
                var html = new HtmlString("<script src=\"https://cdnjs.cloudflare.com/ajax/libs/jstree/3.2.1/jstree.min.js\"></script>");
                html.WriteTo(writer, HtmlEncoder.Default);
                Resources.JSTreeLibLoaded = true;
            }
            if (!Resources.JSTreeCssLoaded)
            {
                var html = new HtmlString("<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/jstree/3.2.1/themes/default/style.min.css\" />");
                html.WriteTo(writer, HtmlEncoder.Default);
                Resources.JSTreeCssLoaded = true;
            }
            var h = new HtmlString(t.DrawWithJSTree(Guid.NewGuid().ToString("N")));
            h.WriteTo(writer, HtmlEncoder.Default);
        }, HtmlFormatter.MimeType);
        
        Kernel.Current.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added formatter for jsTree diagrams to .NET Interactive kernel {Kernel.Current.Name}.")));
    }
}