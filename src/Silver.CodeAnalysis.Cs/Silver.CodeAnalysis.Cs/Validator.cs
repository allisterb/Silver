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

using Stratis.SmartContracts;
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
            if (ns != null && !WhitelistedNamespaces.Contains(ns.ToFullString()))
            {
                return CreateDiagnostic("SC0001", ns.GetLocation(), ns.ToFullString());
            }
            else
            {
                return NoDiagnostic;
            }
        }

        // Namespace declarations not allowed in smart contract code
        public static Diagnostic AnalyzeNamespaceDecl(NamespaceDeclarationSyntax node, SemanticModel model)
        {
            var ns = node.DescendantNodes().First();
            return CreateDiagnostic("SC0002", ns.GetLocation(), ns.ToFullString());
        }

        // Declared classes must inherit from Stratis.SmartContracts.SmartContract
        public static Diagnostic AnalyzeClassDecl(ClassDeclarationSyntax node, SemanticModel model)
        {
            var classSymbol = model.GetDeclaredSymbol(node) as ITypeSymbol;
            if (classSymbol.BaseType is null || classSymbol.BaseType.ToDisplayString() != "Stratis.SmartContracts.SmartContract")
            {
                return CreateDiagnostic("SC0003", node.ChildTokens().First(t => t.IsKind(SyntaxKind.IdentifierToken)).GetLocation(), classSymbol.Name);
            }
            else
            {
                return NoDiagnostic;
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

            if (fp == null) return NoDiagnostic;

            var fpt = fp.Type;
            
            var fpn = fp
                .ChildTokens()
                .First(t => t.IsKind(SyntaxKind.IdentifierToken));
            
            var classSymbol = model.GetSymbolInfo(fpt).Symbol as ITypeSymbol;
            if (classSymbol.ToDisplayString() != "Stratis.SmartContracts.ISmartContractState")
            {
                return CreateDiagnostic("SC0004", fpn.GetLocation(), classSymbol.Name);
            }
            else
            {
                return NoDiagnostic;
            }
        }

        // New object creation not allowed except for structs and arrays of primitives types
        public static Diagnostic AnalyzeObjectCreation(IObjectCreationOperation objectCreation)
        {            
            var t = objectCreation.Type;
            if (t.IsValueType || PrimitiveArrayTypeNames.Contains(t.Name))
            {
                return NoDiagnostic;
            }
            else
            {
                return CreateDiagnostic("SC0005", objectCreation.Syntax.GetLocation(), t.ToDisplayString());
            }
            
        }

        // Field declarations not allowed in smart contract classes
        public static Diagnostic AnalyzeFieldDecl(FieldDeclarationSyntax node, SemanticModel model)
        {
            return CreateDiagnostic("SC0006", node.GetLocation());
        }

        // Only primitive types or smart contract types or arrays of these kinds of types can be used as variables in a method
        public static Diagnostic AnalyzeLocalDecl(LocalDeclarationStatementSyntax node, SemanticModel model)
        {
            var t = model.GetSymbolInfo(node.Declaration.Type).Symbol.ToDisplayString();

            if (PrimitiveTypeNames.Contains(t) || SmartContractTypeNames.Contains(t) || PrimitiveArrayTypeNames.Contains(t) || SmartContractArrayTypeNames.Contains(t))
            {
                return NoDiagnostic;
            }
            else
            {
                return CreateDiagnostic("SC0007", node.GetLocation(), t);

            }
        }

        // Only members on primitive types and smart contract types and some whitelisted array members can be accessed
        public static Diagnostic AnalyzeMemberAccess(MemberAccessExpressionSyntax node, SemanticModel model)
        {
            switch(node.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.NumericLiteralExpression:
                    return NoDiagnostic;
                default:
                    var si = model.GetSymbolInfo(node.Expression).Symbol;
                    if (si is null || si.Kind != SymbolKind.Local) return NoDiagnostic;
                    var symbol = (ILocalSymbol)model.GetSymbolInfo(node.Expression).Symbol;
                    var type = symbol.Type.ToDisplayString();
                    var member = node.Name.ToFullString();
                    if (PrimitiveTypeNames.Contains(type) || SmartContractTypeNames.Contains(type))
                    {
                        return NoDiagnostic;
                    }
                    else if ((PrimitiveArrayTypeNames.Contains(type) || SmartContractArrayTypeNames.Contains(type)) && WhiteListedArrayMemberNames.Contains(member))
                    {
                        return NoDiagnostic;
                    }
                    else
                    {
                        return CreateDiagnostic("SC0008", node.GetLocation(), member, type);
                    }
            }
        }

        // Only methods on primitive and smart contract types and some whitelisted System.Object methods can be invoked.
        public static Diagnostic AnalyzeInvocation(InvocationExpressionSyntax node, SemanticModel model)
        {
            var symbol = model.GetSymbolInfo(node.Expression).Symbol as IMethodSymbol;
            var method = symbol.Name;
            var type = symbol.ContainingType.ToDisplayString();
            if (PrimitiveTypeNames.Contains(type) || SmartContractTypeNames.Contains(type))
            {
                return NoDiagnostic;
            }
            else if (WhitelistedMethodNames.ContainsKey(type) && WhitelistedMethodNames[type].Contains(method))
            {
                return NoDiagnostic;
            }
            else
            {
                return CreateDiagnostic("SC0009", node.GetLocation(), method, type);
            }

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

        public static Diagnostic AnalyzeFieldDecl(FieldDeclarationSyntax node, SyntaxNodeAnalysisContext ctx) =>
           AnalyzeFieldDecl(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeLocalDecl(LocalDeclarationStatementSyntax node, SyntaxNodeAnalysisContext ctx) =>
           AnalyzeLocalDecl(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeInvocation(InvocationExpressionSyntax node, SyntaxNodeAnalysisContext ctx) =>
           AnalyzeInvocation(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeMemberAccess(MemberAccessExpressionSyntax node, SyntaxNodeAnalysisContext ctx) =>
           AnalyzeMemberAccess(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeObjectCreation(IObjectCreationOperation objectCreation, OperationAnalysisContext ctx) =>
            AnalyzeObjectCreation(objectCreation).Report(ctx);
        
        #endregion

        public static DiagnosticDescriptor GetErrorDescriptor(string id) =>
            new DiagnosticDescriptor(id, RM.GetString($"{id}_Title"), RM.GetString($"{id}_MessageFormat"), Category,
                DiagnosticSeverity.Error, true, RM.GetString($"{id}_Description"));
        
        public static Diagnostic CreateDiagnostic(string id, Location location, params object[] args) =>
            Diagnostic.Create(GetErrorDescriptor(id), location, args);
        #endregion

        #region Fields
        internal static string[] DiagnosticIds = { "SC0001", "SC0002", "SC0003", "SC0004", "SC0005", "SC0006", "SC0007", "SC0008", "SC0009" };
        internal static Diagnostic NoDiagnostic = null;
        internal static ImmutableArray<DiagnosticDescriptor> Errors;
        internal static string Category = "Smart Contract";
        internal static System.Resources.ResourceManager RM = Resources.ResourceManager;
        
        public static Type[] BoxedPrimitiveTypes =
        {
            typeof(void),
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(string),
            typeof(UInt128),
            typeof(UInt256)
        };

        public static Type[] BoxedPrimitiveArrayTypes =
       {
            typeof(bool[]),
            typeof(byte[]),
            typeof(sbyte[]),
            typeof(char[]),
            typeof(int[]),
            typeof(uint[]),
            typeof(long[]),
            typeof(ulong[]),
            typeof(string[]),
            typeof(UInt128[]),
            typeof(UInt256[])
        };

        public static Type[] SmartContractTypes =
        {
            typeof(Address),
            typeof(Block),
            typeof(IBlock),
            typeof(IContractLogger),
            typeof(ICreateResult),
            typeof(IMessage),
            typeof(IPersistentState),
            typeof(ISmartContractState),
            typeof(ISerializer),
            typeof(ITransferResult),
            typeof(Message),
            typeof(SmartContract)
        };

        public static Type[] SmartContractArrayTypes =
        {
            typeof(Address[]),
            typeof(Block[]),
            typeof(IBlock[]),
            typeof(IContractLogger[]),
            typeof(ICreateResult[]),
            typeof(IMessage[]),
            typeof(IPersistentState[]),
            typeof(ISmartContractState[]),
            typeof(ITransferResult[]),
            typeof(Message[]),
        };

        public static Type[] SmartContractAttributeTypes =
        {
            typeof(DeployAttribute),
            typeof(IndexAttribute)
        };

        public static string[] UnboxedPrimitiveTypeNames =
        {
            "void",
            "bool",
            "byte",
            "sbyte",
            "char",
            "int",
            "uint",
            "long",
            "ulong",
            "string",
            "String"
        };

        public static string[] PrimitiveTypeNames = UnboxedPrimitiveTypeNames.Concat(BoxedPrimitiveTypes.Select(t => t.FullName)).ToArray();

        public static string[] PrimitiveArrayTypeNames = UnboxedPrimitiveTypeNames.Select(t => t + "[]").Concat(BoxedPrimitiveArrayTypes.Select(t => t.FullName)).ToArray();

        public static string[] SmartContractTypeNames = SmartContractTypes.Select(t => t.FullName).ToArray();

        public static string[] SmartContractArrayTypeNames = SmartContractArrayTypes.Select(t => t.FullName).ToArray();

        public static string[] WhiteListedArrayMemberNames = { "GetLength", "Copy", "GetValue", "SetValue", "ReSize" };

        public static Dictionary<string, string[]> WhitelistedMethodNames = new Dictionary<string, string[]>() 
        {
            { "object", new string [] {"ToString" } }
        };
        public static string[] WhitelistedNamespaces = { "System", "Stratis.SmartContracts", "Stratis.SmartContracts.Standards" };
        #endregion
    }
}
