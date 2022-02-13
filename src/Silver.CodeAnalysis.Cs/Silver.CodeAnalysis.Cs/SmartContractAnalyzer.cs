namespace Silver.CodeAnalysis.Cs
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    
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
            if (!Debugger.IsAttached) context.EnableConcurrentExecution();
            
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(ctx => Validator.AnalyzeUsingDirective((UsingDirectiveSyntax)ctx.Node, ctx), SyntaxKind.UsingDirective);
            context.RegisterSyntaxNodeAction(ctx => Validator.AnalyzeNamespaceDecl((NamespaceDeclarationSyntax)ctx.Node, ctx), SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(ctx => Validator.AnalyzeClassDecl((ClassDeclarationSyntax)ctx.Node, ctx), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(ctx => Validator.AnalyzeConstructorDecl((ConstructorDeclarationSyntax)ctx.Node, ctx), SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(ctx => Validator.AnalyzeFieldDecl((FieldDeclarationSyntax)ctx.Node, ctx), SyntaxKind.FieldDeclaration);
            
            context.RegisterOperationAction(ctx =>
                {
                    switch (ctx.Operation)
                    {
                        case IObjectCreationOperation objectCreation:
                            Validator.AnalyzeObjectCreation(objectCreation, ctx);
                            break;

                        case IPropertyReferenceOperation propReference:
                            Validator.AnalyzePropertyReference(propReference, ctx);
                            break;

                        case IInvocationOperation methodInvocation:
                            Validator.AnalyzeMethodInvocation(methodInvocation, ctx);
                            break;
                        
                        case ILocalReferenceOperation localReference:
                            Validator.AnalyzeLocalReference(localReference, ctx);
                            break;
                    }
                }, OperationKind.ObjectCreation, OperationKind.Invocation, OperationKind.PropertyReference, OperationKind.LocalReference);
            //context.RegisterCompilationStartAction(OnCompilationStart);
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Namespace, SymbolKind.NamedType);
            #endregion

        }
    }
}
