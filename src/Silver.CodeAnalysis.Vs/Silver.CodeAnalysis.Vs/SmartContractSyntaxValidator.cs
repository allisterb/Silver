using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Silver.CodeAnalysis.Vs
{
    
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
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(ctx => {

               var g = (UsingDirectiveSyntax)ctx.Node;
               var xx = g.DescendantNodes().First();        
               if (xx.ToFullString() != "Stratis.SmartContracts")
                {
                    var diagnostic = Diagnostic.Create(GetErrorDescriptor("SC0001"), g.GetLocation(), xx.ToFullString());

                    ctx.ReportDiagnostic(diagnostic);
                }
            }, SyntaxKind.UsingDirective);
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Namespace, SymbolKind.NamedType);
        }
        #endregion

        #region Methods
        static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            if (true)
            {
                /*
                if (ns.Name != "Stratis.SmartContracts")
                {
                    var diagnostic = Diagnostic.Create(Rule, ns.Locations[0], ns.Name);

                    context.ReportDiagnostic(diagnostic);
                }
                */
            }

            // Find just those named type symbols with names containing lowercase letters.
            //if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            //{
                // For all such symbols, produce a diagnostic.
            //    var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                //context.ReportDiagnostic(diagnostic);
            //}
        }

        static DiagnosticDescriptor GetErrorDescriptor(string id) =>
            new DiagnosticDescriptor(id, RM.GetString($"{id}_Title"), RM.GetString($"{id}_MessageFormat"), Category, 
                DiagnosticSeverity.Error, true, RM.GetString($"{id}_Description"));
        #endregion

        #region Fields
        static string[] DiagnosticIds = { "SC0001" };

        static ImmutableArray<DiagnosticDescriptor> Errors;

        private const string Category = "Smart Contract";
        private static System.Resources.ResourceManager RM = Resources.ResourceManager;

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.SC0001_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.SC001_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.SC0001_Description), Resources.ResourceManager, typeof(Resources));
        
        //private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
        #endregion
    }
}
