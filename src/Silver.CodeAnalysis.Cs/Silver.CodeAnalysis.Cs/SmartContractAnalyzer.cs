﻿namespace Silver.CodeAnalysis.Cs
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
    using Microsoft.CodeAnalysis.Operations;

   
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SmartContractAnalyzer : DiagnosticAnalyzer
    {
        #region Overriden members
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = Validator.Errors;

        public override void Initialize(AnalysisContext context)
        {
            if (!System.Diagnostics.Debugger.IsAttached) context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(ctx => Validator.AnalyzeUsingDirective((UsingDirectiveSyntax)ctx.Node, ctx), SyntaxKind.UsingDirective);
            context.RegisterSyntaxNodeAction(ctx => Validator.AnalyzeNamespaceDecl((NamespaceDeclarationSyntax)ctx.Node, ctx), SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(ctx => Validator.AnalyzeClassDecl((ClassDeclarationSyntax)ctx.Node, ctx), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(ctx => Validator.AnalyzeConstructor((ConstructorDeclarationSyntax)ctx.Node, ctx), SyntaxKind.ConstructorDeclaration);

            context.RegisterOperationAction(
                ctx =>
                {
                    switch (ctx.Operation)
                    {
                        case IObjectCreationOperation objectCreation:
                            Validator.Analyze(objectCreation);
                            break;
                            //objectCreation.Constructor.ty

                    }
                }, OperationKind.ConstructorBody);
            //context.RegisterCompilationStartAction(OnCompilationStart);
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Namespace, SymbolKind.NamedType);
            //context.re
            //private void OnCompilationStart(CompilationStartAnalysisContext compilationContext)
            //{

            //}
            #endregion


        }
    }
}
