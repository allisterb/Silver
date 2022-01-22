using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Diagnostics;


namespace Silver.CodeAnalysis.Cs
{
    public class SyntaxAnalyzer
    {
        #region Methods
        public static Diagnostic AnalyzeUsingDirective(UsingDirectiveSyntax node, SyntaxNodeAnalysisContext? ctx = null)
        {
            var ns = node.DescendantNodes().OfType<IdentifierNameSyntax>().First();
            if (ns.ToFullString() != "Stratis.SmartContracts")
            {
                var diagnostic = Diagnostic.Create(GetErrorDescriptor("SC0001"), ns.GetLocation(), ns.ToFullString());
                ctx?.ReportDiagnostic(diagnostic);
                return diagnostic;
            }
            else
            {
                return null;
            }
        }

        public static Diagnostic AnalyzeNamespaceDecl(NamespaceDeclarationSyntax node, SyntaxNodeAnalysisContext? ctx = null)
        {
            var ns = node.DescendantNodes().First();
            var diagnostic = Diagnostic.Create(GetErrorDescriptor("SC0002"), ns.GetLocation(), ns.ToFullString());
            ctx?.ReportDiagnostic(diagnostic);
            return diagnostic;
        }

        public static Diagnostic AnalyzeClassDecl(ClassDeclarationSyntax node, SyntaxNodeAnalysisContext ctx) =>
            AnalyzeClassDecl(node, ctx.SemanticModel).Report(ctx);

        
        public static Diagnostic AnalyzeClassDecl(ClassDeclarationSyntax node, SemanticModel model)
        {
            var classSymbol = model.GetDeclaredSymbol(node) as ITypeSymbol;
            if (classSymbol.BaseType is null || classSymbol.BaseType.ToDisplayString() != "Stratis.SmartContracts.SmartContract")
            {
                return Diagnostic.Create(GetErrorDescriptor("SC0003"), node.GetLocation(), classSymbol.Name);
            }
            else
            {
                return null;
            }
        }
        public static Diagnostic ReportSyntaxNodeDiagnostic(SyntaxNodeAnalysisContext ctx, Diagnostic diagnostic)
        {
            if (diagnostic != null)
            {
                ctx.ReportDiagnostic(diagnostic);
            }
            return diagnostic;
        }

        public static DiagnosticDescriptor GetErrorDescriptor(string id) =>
            new DiagnosticDescriptor(id, RM.GetString($"{id}_Title"), RM.GetString($"{id}_MessageFormat"), Category,
                DiagnosticSeverity.Error, true, RM.GetString($"{id}_Description"));
        
        #endregion

        #region Fields
        internal static string[] DiagnosticIds = { "SC0001", "SC0002", "SC0003" };
        internal static ImmutableArray<DiagnosticDescriptor> Errors;
        internal static string Category = "Smart Contract";
        internal static System.Resources.ResourceManager RM = Resources.ResourceManager;
        #endregion
    }
}
