using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
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
                var diagnostic = Diagnostic.Create(GetErrorDescriptor("SC0001"), node.GetLocation(), xx.ToFullString());
                ctx?.ReportDiagnostic(diagnostic);
                return diagnostic;
            }
            else
            {
                return null;
            }
        }

        public static Diagnostic GetDiagnostic(string id, CSharpSyntaxTree syntaxTree, int line, int col, params object[] args) =>
            Diagnostic.Create(GetErrorDescriptor(id), Location.Create(syntaxTree, TextSpan.FromBounds(0, 10)), args);
      
        public static DiagnosticDescriptor GetErrorDescriptor(string id) =>
            new DiagnosticDescriptor(id, RM.GetString($"{id}_Title"), RM.GetString($"{id}_MessageFormat"), Category,
                DiagnosticSeverity.Error, true, RM.GetString($"{id}_Description"));
        #endregion

        #region Fields
        internal static string[] DiagnosticIds = { "SC0001" };
        internal static ImmutableArray<DiagnosticDescriptor> Errors;
        internal static string Category = "Smart Contract";
        internal static System.Resources.ResourceManager RM = Resources.ResourceManager;
        #endregion
    }
}
