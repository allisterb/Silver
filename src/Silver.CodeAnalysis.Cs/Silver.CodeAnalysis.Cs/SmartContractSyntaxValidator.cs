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
    public class SmartContractSyntaxValidator : DiagnosticAnalyzer
    {
        #region Constructors
        static SmartContractSyntaxValidator()
        {
            Errors = ImmutableArray.Create(DiagnosticIds.Select(i => GetErrorDescriptor(i)).ToArray());

        }
        #endregion
        #region Overriden members
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = Errors;

        public override void Initialize(AnalysisContext context)
        {
            if (!System.Diagnostics.Debugger.IsAttached) context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(ctx => AnalyzeUsingDirective((UsingDirectiveSyntax) ctx.Node, ctx), 
                SyntaxKind.UsingDirective);
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Namespace, SymbolKind.NamedType);
        }
        #endregion

        #region Methods
        public static DiagnosticDescriptor GetErrorDescriptor(string id) =>
            new DiagnosticDescriptor(id, RM.GetString($"{id}_Title"), RM.GetString($"{id}_MessageFormat"), Category,
                DiagnosticSeverity.Error, true, RM.GetString($"{id}_Description"));
        #endregion

        #region Fields
        static string[] DiagnosticIds = { "SC0001" };

        static ImmutableArray<DiagnosticDescriptor> Errors;

        private const string Category = "Smart Contract";
        private static System.Resources.ResourceManager RM = Resources.ResourceManager;


        #endregion
    }
}
