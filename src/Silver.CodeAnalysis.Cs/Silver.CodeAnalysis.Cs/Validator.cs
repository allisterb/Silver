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

        // Namespace declarations not allowed in smart contract code
        public static Diagnostic AnalyzeNamespaceDecl(NamespaceDeclarationSyntax node, SemanticModel model)
        {
            var ns = node.DescendantNodes().First();
            return CreateDiagnostic("SC0001", ns.GetLocation(), ns.ToFullString());
        }

        // Only allow using Stratis.SmartContract namespace in smart contract code
        public static Diagnostic AnalyzeUsingDirective(UsingDirectiveSyntax node, SemanticModel model)
        {
            var ns = node.DescendantNodes().OfType<NameSyntax>().FirstOrDefault();
            if (ns != null && !WhitelistedNamespaces.Contains(ns.ToFullString()))
            {
                return CreateDiagnostic("SC0002", ns.GetLocation(), ns.ToFullString());
            }
            else
            {
                return NoDiagnostic;
            }
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
            if (node.Parent is StructDeclarationSyntax) return NoDiagnostic;
            var parent = (ClassDeclarationSyntax) node.Parent;
            var parentSymbol = model.GetDeclaredSymbol(parent) as ITypeSymbol;

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
                return CreateDiagnostic("SC0004", fpn.GetLocation(), fp.Identifier.Text);
            }
            else
            {
                return NoDiagnostic;
            }
        }

        // Non-const field declarations outside structs not allowed in smart contract classes
        public static Diagnostic AnalyzeFieldDecl(FieldDeclarationSyntax node, SemanticModel model)
        {
            if (node.Parent.IsKind(SyntaxKind.StructDeclaration))
            {
                return NoDiagnostic;
            }
            else if (node.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword)))
            {

                return NoDiagnostic;
            }
            else
            {
                return CreateDiagnostic("SC0006", node.GetLocation());
            }
        }

        // New object creation not allowed except for structs and arrays of primitives types
        public static Diagnostic AnalyzeObjectCreation(IObjectCreationOperation objectCreation)
        {
            var type = objectCreation.Type;
            var typename = type.ToDisplayString();
            if (type.IsValueType || type.IsArray() || PrimitiveArrayTypeNames.Contains(typename))
            {
                return NoDiagnostic;
            }
            else
            {
                return CreateDiagnostic("SC0005", objectCreation.Syntax.GetLocation(), type.ToDisplayString());
            }
        }

        public static Diagnostic AnalyzePropertyReference(IPropertyReferenceOperation propReference)
        {
            var member = propReference.Member;
            string propname = member.Name;
            var type = member.ContainingType;
            var basetype = type.BaseType;
            var typename = type.ToDisplayString();
            var basetypename = basetype?.ToDisplayString() ?? "";
            if (type.IsValueType || type.IsEnum())
            {
                return NoDiagnostic;
            }
            if (PrimitiveTypeNames.Contains(typename) || SmartContractTypeNames.Contains(typename) || SmartContractTypeNames.Contains(basetypename))
            {
                return NoDiagnostic;
            }
            else if ((type.IsArray() || PrimitiveArrayTypeNames.Contains(typename) || SmartContractArrayTypeNames.Contains(typename)) && WhitelistedArrayPropertyNames.Contains(propname))
            {
                return NoDiagnostic;
            }
            else
            {
                return CreateDiagnostic("SC0008", propReference.Syntax.GetLocation(), propname, type);
            }
        }

        public static Diagnostic AnalyzeMethodInvocation(IInvocationOperation methodInvocation)
        {
            var node = methodInvocation.Syntax;
            var method = methodInvocation.TargetMethod;
            var type = method.ContainingType;
            var basetype = type.BaseType;
            var typename = type.ToDisplayString();
            var basetypename = basetype?.ToDisplayString() ?? string.Empty;
            if (PrimitiveTypeNames.Contains(typename) || SmartContractTypeNames.Contains(typename) || SmartContractTypeNames.Contains(basetypename))
            {
                return NoDiagnostic;
            }
            else if (WhitelistedMethodNames.ContainsKey(typename) && WhitelistedMethodNames[typename].Contains(method.Name))
            {
                return NoDiagnostic;
            }
            else
            {
                return CreateDiagnostic("SC0009", node.GetLocation(), method, typename);
            }
        }

        public static Diagnostic AnalyzeVariableDeclaration(IVariableDeclaratorOperation variableDeclarator)
        {
            var node = variableDeclarator.Syntax;
            var type = variableDeclarator.Symbol.Type;
            if (type.Kind == SymbolKind.ArrayType)
            {
                var ii = (IArrayTypeSymbol)type;
            }
            var typename = type.ToDisplayString();
            var basetype = type.BaseType;
            var basetypename = basetype?.ToDisplayString() ?? string.Empty;
            if (type.IsValueType || type.IsEnum() || PrimitiveTypeNames.Contains(typename) || SmartContractTypeNames.Contains(typename))
            {
                return NoDiagnostic;
            }
            else if ((type.IsArray() || PrimitiveArrayTypeNames.Contains(typename) || SmartContractArrayTypeNames.Contains(typename)))
            {
                return NoDiagnostic;
            }
            else
            {
                return CreateDiagnostic("SC0007", node.GetLocation(), typename);
            }
        }

        #region Overloads
        public static Diagnostic AnalyzeUsingDirective(UsingDirectiveSyntax node, SyntaxNodeAnalysisContext ctx) =>
           AnalyzeUsingDirective(node, ctx.SemanticModel)?.Report(ctx);
        public static Diagnostic AnalyzeNamespaceDecl(NamespaceDeclarationSyntax node, SyntaxNodeAnalysisContext ctx) =>
            AnalyzeNamespaceDecl(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeClassDecl(ClassDeclarationSyntax node, SyntaxNodeAnalysisContext ctx) =>
           AnalyzeClassDecl(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeConstructorDecl(ConstructorDeclarationSyntax node, SyntaxNodeAnalysisContext ctx) =>
            AnalyzeConstructorDecl(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeFieldDecl(FieldDeclarationSyntax node, SyntaxNodeAnalysisContext ctx) =>
           AnalyzeFieldDecl(node, ctx.SemanticModel)?.Report(ctx);

        public static Diagnostic AnalyzeObjectCreation(IObjectCreationOperation objectCreation, OperationAnalysisContext ctx) =>
            AnalyzeObjectCreation(objectCreation).Report(ctx);

        public static Diagnostic AnalyzePropertyReference(IPropertyReferenceOperation propertyReference, OperationAnalysisContext ctx) =>
            AnalyzePropertyReference(propertyReference).Report(ctx);

        public static Diagnostic AnalyzeMethodInvocation(IInvocationOperation methodInvocation, OperationAnalysisContext ctx) =>
            AnalyzeMethodInvocation(methodInvocation).Report(ctx);

        public static Diagnostic AnalyzeVariableDeclaration(IVariableDeclaratorOperation variableDeclarator, OperationAnalysisContext ctx) =>
            AnalyzeVariableDeclaration(variableDeclarator).Report(ctx);
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

        public static string[] WhitelistedArrayPropertyNames = { "Length" };

        public static string[] WhitelistedArrayMethodNames = { "GetLength", "Copy", "GetValue", "SetValue", "ReSize" };

        public static Dictionary<string, string[]> WhitelistedMethodNames = new Dictionary<string, string[]>() 
        {
            { "object", new string [] { "ToString" } },
            { "System.Object", new string [] { "ToString" } },
            { "System.Array", WhitelistedArrayMethodNames}
        };
        public static string[] WhitelistedNamespaces = { "System", "Stratis.SmartContracts", "Stratis.SmartContracts.Standards" };

        #endregion
    }
}
