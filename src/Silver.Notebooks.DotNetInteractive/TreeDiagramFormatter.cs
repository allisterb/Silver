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
        var html = new HtmlString("<link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/jstree/3.2.1/themes/default/style.min.css\" />");
        KernelInvocationContext.Current?.Display(html, "text/html");
        Resources.JSTreeCssLoaded = true;
        Formatter.Register<TreeDiagram>((t, writer) =>
        {
            var config = "{\"types\" : {\"default\" : {\"icon\" : \"fa fa-question-circle\"}, \"demo\" : {\"icon\" : \"glyphicon glyphicon-ok\"}},\"plugins\" : [\"types\"]}";

            var id = Guid.NewGuid().ToString("N");
            var s = $@"<script>(require.config({{ 'paths': {{ 'jstree' : 'https://cdnjs.cloudflare.com/ajax/libs/jstree/3.2.1/jstree.min' }}}}) || require)(['jstree'], (jstree) => {{$('#{id}').jstree({config});}});</script>";
            var h = new HtmlString(t.DrawWithJSTree(id) + s);
            h.WriteTo(writer, HtmlEncoder.Default);
        }, HtmlFormatter.MimeType);
        
        Kernel.Current.SendAsync(
            new DisplayValue(new FormattedValue(
                "text/markdown",
                $"Added formatter for jsTree diagrams to .NET Interactive kernel {Kernel.Current.Name}.")));
    }
}