namespace Silver.Notebooks;

using System;
using System.Collections.Generic;
using System.Net;
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
public class HighlightedCodeFormatter : Runtime
{
    public static void Register()
    {
        var html = new HtmlString(Resources.GenerateScriptElement("https://unpkg.com/@antfu/shiki@0.5.2/dist/index.unpkg.iife.js", "shikilib"));
        KernelInvocationContext.Current?.Display(html, "text/html");
        html = new HtmlString(Resources.GenerateScriptElement("https://unpkg.com/@antfu/shiki-renderer-svg@0.5.2/dist/index.iife.min.js", "shikirendererlib"));
        KernelInvocationContext.Current?.Display(html, "text/html");
        html = new HtmlString(Resources.GenerateScriptElement("https://dokans3fs-test-1.us-east-1.linodeobjects.com/shikiLanguages.js", "shikilanguageslib"));
        KernelInvocationContext.Current?.Display(html, "text/html");
        Formatter.Register<TmHighlightedCode>((t, writer) =>
        {
            /*
            if (!Resources.ShikiLibLoaded)
            {
                var html = new HtmlString(Resources.GenerateScriptElement("https://unpkg.com/@antfu/shiki@0.5.2/dist/index.unpkg.iife.js", "shikilib"));
                
                html.WriteTo(writer, HtmlEncoder.Default);
                Resources.ShikiLibLoaded = true;
            }
            if (!Resources.ShikiRendererLibLoaded)
            {
                var html = new HtmlString(Resources.GenerateScriptElement("https://unpkg.com/@antfu/shiki-renderer-svg@0.5.2/dist/index.iife.min.js", "shikirendererlib"));
                html.WriteTo(writer, HtmlEncoder.Default);
                Resources.ShikiRendererLibLoaded = true;
            }
            if (!Resources.ShikiLanguagesLoaded)
            {
                var html = new HtmlString(Resources.GenerateScriptElement("https://dokans3fs-test-1.us-east-1.linodeobjects.com/shikiLanguages.js", "shikilanguageslib"));


                //var l = DefaultHttpClient.GetStringAsync("https://dokans3fs-test-1.us-east-1.linodeobjects.com/shikiLanguages.js").Result;
                //var html = new HtmlString("<script type = \"text/javascript\">\n" + l + " </script>");
                html.WriteTo(writer, HtmlEncoder.Default);
                Resources.ShikiLanguagesLoaded = true;
            }*/
            var h = new HtmlString(t.DrawWithShiki(Guid.NewGuid().ToString("N")));
            h.WriteTo(writer, HtmlEncoder.Default);
        }, HtmlFormatter.MimeType);

        Kernel.Current.SendAsync(new DisplayValue(new FormattedValue("text/markdown", $"Added formatter for TmHighlightedCode to .NET Interactive kernel {Kernel.Current.Name}.")));
    }
}