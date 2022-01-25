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
using Microsoft.CodeAnalysis.Text;

namespace Silver.CodeAnalysis.Cs
{
    public class Validator
    {
        #region Constructors
        static Validator()
        {
            Errors = ImmutableArray.Create(DiagnosticIds.Select(i => GetErrorDescriptor(i)).ToArray());
        }
        #endregion

        #region Methods

        // Only allow using Stratis.SmartContracts namespace in smart contract code
        public static Diagnostic AnalyzeUsingDirective(UsingDirectiveSyntax node, SemanticModel model)
        {
            var ns = node.DescendantNodes().OfType<NameSyntax>().FirstOrDefault();  
            if (ns.ToFullString() != "Stratis.SmartContracts")
            {
                return Diagnostic.Create(GetErrorDescriptor("SC0001"), ns.GetLocation(), ns.ToFullString());
            }
            else
            {
                return null;
            }
        }

        // Namespace declarations not allowed in smart contract code
        public static Diagnostic AnalyzeNamespaceDecl(NamespaceDeclarationSyntax node, SemanticModel model)
        {
            var ns = node.DescendantNodes().First();
            return Diagnostic.Create(GetErrorDescriptor("SC0002"), ns.GetLocation(), ns.ToFullString());
        }

        // Declared classes must inherit from Stratis.SmartContracts.SmartContract
        public static Diagnostic AnalyzeClassDecl(ClassDeclarationSyntax node, SemanticModel model)
        {
            var classSymbol = model.GetDeclaredSymbol(node) as ITypeSymbol;
            if (classSymbol.BaseType is null || classSymbol.BaseType.ToDisplayString() != "Stratis.SmartContracts.SmartContract")
            {
                return Diagnostic.Create(GetErrorDescriptor("SC0003"), node.ChildTokens().First(t => t.IsKind(SyntaxKind.IdentifierToken)).GetLocation(), classSymbol.Name);
            }
            else
            {
                return null;
            }
        }

        // Class constructor must have a ISmartContractState as first parameter.
        public static Diagnostic AnalyzeConstructorDecl(ConstructorDeclarationSyntax node, SemanticModel model)
        {
            var fp = node
                .DescendantNodes()
                .OfType<ParameterListSyntax>()
                .FirstOrDefault()?
                .DescendantNodes()
                .OfType<ParameterSyntax>()
                .FirstOrDefault();

            if (fp == null) return null;

            var fpt = fp.Type;
            
            var fpn = fp
                .ChildTokens()
                .First(t => t.IsKind(SyntaxKind.IdentifierToken));
            
            var classSymbol = model.GetSymbolInfo(fpt).Symbol as ITypeSymbol;
            if (classSymbol.ToDisplayString() != "Stratis.SmartContracts.ISmartContractState")
            {
                return Diagnostic.Create(GetErrorDescriptor("SC0004"), fpn.GetLocation(), classSymbol.Name);
            }
            else
            {
                return null;
            }
            
        }

        // New object creation not allowed
        public static Diagnostic AnalyzeObjectCreation(IObjectCreationOperation objectCreation)
        {
            var t = objectCreation.Type;
            if (t.IsValueType) return null;
            return Diagnostic.Create(GetErrorDescriptor("SC0005"), objectCreation.Syntax.GetLocation(), t.ToDisplayString());
        }

        #region Overloads
        public static Diagnostic AnalyzeUsingDirective(UsingDirectiveSyntax node, SyntaxNodeAnalysisContext ctx) =>
           AnalyzeUsingDirective(node, ctx.SemanticModel)?.Report(ctx);
        public static Diagnostic AnalyzeNamespaceDecl(NamespaceDeclarationSyntax node, SyntaxNodeAnalysisContext ctx) =>
            AnalyzeNamespaceDecl(node, ctx.SemanticModel)?.Report(ctx);
        
        public static Diagnostic AnalyzeClassDecl(ClassDeclarationSyntax node, SyntaxNodeAnalysisContext ctx) =>
           AnalyzeClassDecl(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeConstructor(ConstructorDeclarationSyntax node, SyntaxNodeAnalysisContext ctx) =>
            AnalyzeConstructorDecl(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeObjectCreation(IObjectCreationOperation objectCreation, OperationAnalysisContext ctx) =>
            AnalyzeObjectCreation(objectCreation).Report(ctx);
        #endregion

        public static DiagnosticDescriptor GetErrorDescriptor(string id) =>
            new DiagnosticDescriptor(id, RM.GetString($"{id}_Title"), RM.GetString($"{id}_MessageFormat"), Category,
                DiagnosticSeverity.Error, true, RM.GetString($"{id}_Description"));
        
        #endregion

        #region Fields
        internal static string[] DiagnosticIds = { "SC0001", "SC0002", "SC0003", "SC0004", "SC0005"};
        internal static ImmutableArray<DiagnosticDescriptor> Errors;
        internal static string Category = "Smart Contract";
        internal static System.Resources.ResourceManager RM = Resources.ResourceManager;


        public static Type[] WhitelistedTypes =
        {
            typeof(void),
            typeof(System.Boolean),
            typeof(System.Byte),
            typeof(System.Char),
            typeof(System.Int32),
            typeof(System.UInt32),
            typeof(System.Int64),
            typeof(System.UInt64),
            typeof(System.String),
            typeof(System.Boolean[]),
            typeof(System.Byte[]),
            typeof(System.Char[]),
            typeof(System.Int32[]),
            typeof(System.UInt32[]),
            typeof(System.Int64[]),
            typeof(System.UInt64[]),
            typeof(System.String[])
        };
        public static string[] WhiteListedTypeNames = WhitelistedTypes.Select(t => t.Name).ToArray();
        
        public static Dictionary<string, string[]> WhiteListedMemberNames = new Dictionary<string, string[]>
        {
            {typeof(System.Array).Name, new[] { "GetLength", "Copy", "GetValue", "SetValue", "ReSize" } },
            {typeof(string[]).Name, new[] { "GetLength", "Copy", "GetValue", "SetValue", "ReSize" } },
        };
        #endregion
    }
}
