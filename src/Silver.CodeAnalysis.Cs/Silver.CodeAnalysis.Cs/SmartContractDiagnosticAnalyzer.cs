namespace Silver.CodeAnalysis.Cs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    
    using static SyntaxAnalyzer;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SmartContractDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        #region Constructors
        static SmartContractDiagnosticAnalyzer()
        {
            Errors = ImmutableArray.Create(DiagnosticIds.Select(i => GetErrorDescriptor(i)).ToArray());
        }
        #endregion

        #region Overriden members
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = SyntaxAnalyzer.Errors;

        public override void Initialize(AnalysisContext context)
        {
            if (!System.Diagnostics.Debugger.IsAttached) context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            
            context.RegisterSyntaxNodeAction(ctx => AnalyzeUsingDirective((UsingDirectiveSyntax) ctx.Node, ctx), 
                SyntaxKind.UsingDirective);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeNamespaceDecl((NamespaceDeclarationSyntax) ctx.Node, ctx), 
                SyntaxKind.NamespaceDeclaration);
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Namespace, SymbolKind.NamedType);
        }
        #endregion

       
    }
}
