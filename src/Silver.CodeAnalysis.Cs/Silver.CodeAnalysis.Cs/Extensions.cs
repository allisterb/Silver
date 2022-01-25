﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
namespace Silver.CodeAnalysis.Cs
{
    public static class Extensions
    {
        public static Diagnostic Report(this Diagnostic diagnostic, SyntaxNodeAnalysisContext ctx)
        {
            if (diagnostic != null) ctx.ReportDiagnostic(diagnostic);
            return diagnostic;
        }

        public static Diagnostic Report(this Diagnostic diagnostic, OperationAnalysisContext ctx)
        {
            if (diagnostic != null) ctx.ReportDiagnostic(diagnostic);
            return diagnostic;
        }
    }
}
