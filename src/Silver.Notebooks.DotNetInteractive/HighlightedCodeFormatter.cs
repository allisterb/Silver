﻿namespace Silver.Notebooks;

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
        Resources.ShikiLibLoaded = true;
        html = new HtmlString(Resources.GenerateScriptElement("https://unpkg.com/@antfu/shiki-renderer-svg@0.5.2/dist/index.iife.min.js", "shikirendererlib"));
        KernelInvocationContext.Current?.Display(html, "text/html");
        Resources.ShikiRendererLibLoaded = true;
        html = new HtmlString(Resources.GenerateScriptElement("https://dokans3fs-test-1.us-east-1.linodeobjects.com/shikiLanguages.js", "shikilanguageslib"));
        KernelInvocationContext.Current?.Display(html, "text/html");
        Resources.ShikiLanguagesLoaded = true;
        Formatter.Register<TmHighlightedCode>((t, writer) =>
        {
            var h = new HtmlString(t.DrawWithShiki(Guid.NewGuid().ToString("N")));
            h.WriteTo(writer, HtmlEncoder.Default);
        }, HtmlFormatter.MimeType);

        Kernel.Current.SendAsync(new DisplayValue(new FormattedValue("text/markdown", $"Added formatter for TextMate highlighted code using Shiki to .NET Interactive kernel {Kernel.Current.Name}.")));
    }
}