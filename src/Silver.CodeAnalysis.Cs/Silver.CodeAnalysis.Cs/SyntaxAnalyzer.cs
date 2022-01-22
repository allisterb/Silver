using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Validator = Silver.CodeAnalysis.Cs.SmartContractSyntaxValidator;
namespace Silver.CodeAnalysis.Cs
{
    public class SyntaxAnalyzer
    {
        #region Methods
        public static Diagnostic AnalyzeUsingDirective(UsingDirectiveSyntax node, SyntaxNodeAnalysisContext? ctx = null)
        {
            var xx = node.DescendantNodes().First();
            if (xx.ToFullString() != "Stratis.SmartContracts")
            {
                var diagnostic = Diagnostic.Create(Validator.GetErrorDescriptor("SC0001"), node.GetLocation(), xx.ToFullString());
                ctx?.ReportDiagnostic(diagnostic);
                return diagnostic;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
