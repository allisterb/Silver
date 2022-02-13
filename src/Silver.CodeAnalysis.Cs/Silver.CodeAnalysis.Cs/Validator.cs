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

        public static Diagnostic AnalyzePropertyReference(IPropertyReferenceOperation propReference)
        {
            string propname = propReference.Property.Name;
            var type = propReference.Type;
            var basetype = type.BaseType;
            var typename = type.ToDisplayString();
            var basetypename = basetype?.ToDisplayString() ?? "";
            if (type.IsValueType || (basetypename == "System.Enum"))
            {
                return NoDiagnostic;
            }
            if (PrimitiveTypeNames.Contains(typename) || SmartContractTypeNames.Contains(typename) || (basetype != null && SmartContractTypeNames.Contains(basetypename)))
            {
                return NoDiagnostic;
            }
            else if ((PrimitiveArrayTypeNames.Contains(typename) || SmartContractArrayTypeNames.Contains(typename)) && WhiteListedArrayMemberNames.Contains(propname))
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
            //if (WhitelistedOperators.Contains(node.Expression.ToFullString())) return NoDiagnostic;
            var node = methodInvocation.Syntax;
            var method = methodInvocation.TargetMethod;
            var type = method.ContainingType.ToDisplayString();
            var basetype = method.ContainingType.BaseType?.ToDisplayString();
            if (PrimitiveTypeNames.Contains(type) || SmartContractTypeNames.Contains(type) || (basetype != null && SmartContractTypeNames.Contains(basetype)))
            {
                return NoDiagnostic;
            }
            else if (WhitelistedMethodNames.ContainsKey(type) && WhitelistedMethodNames[type].Contains(method.Name))
            {
                return NoDiagnostic;
            }
            else
            {
                return CreateDiagnostic("SC0009", node.GetLocation(), method, type);
            }
        }

        public static Diagnostic AnalyzeLocalReference(ILocalReferenceOperation localReference)
        {
            var type = localReference.Type.ToDisplayString();
            var baseType = localReference.Type.BaseType?.ToDisplayString();
            if (PrimitiveTypeNames.Contains(type) || SmartContractTypeNames.Contains(type))
            {
                return NoDiagnostic;
            }
            else if ((PrimitiveArrayTypeNames.Contains(type) || SmartContractArrayTypeNames.Contains(type)) && WhiteListedArrayMemberNames.Contains(type))
            {
                return NoDiagnostic;
            }
            else
            {
                return NoDiagnostic;
                //return CreateDiagnostic("SC0008", node.GetLocation(), member, type);

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

        public static Diagnostic AnalyzeLocalReference(ILocalReferenceOperation localReference, OperationAnalysisContext ctx) =>
            AnalyzeLocalReference(localReference).Report(ctx);
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

        public static string[] WhitelistedOperators = { "nameof" };
        #endregion
    }
}
