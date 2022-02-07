#region Attribution
// Based on PeToText.CSharpSourceEmitter
// Pe2Text.CSharpSourceEmitter has the following copyright notice 
//
//-----------------------------------------------------------------------------
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//-----------------------------------------------------------------------------
#endregion

namespace Silver.CodeAnalysis.IL;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using CSharpSourceEmitter;
using Microsoft.Cci;
using Microsoft.Cci.MetadataReader;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.Contracts;

public class ColorfulSourceEmitter : SourceEmitter
{
    #region Constructor
    public ColorfulSourceEmitter(IColorfulSourceEmitterOutput sourceEmitterOutput, IMetadataHost host, PdbReader? pdbReader, bool printCompilerGeneratedMembers, bool noIL, string? classPattern, string? methodPattern)
      : base(sourceEmitterOutput)
    {
        this.host = host;
        this.pdbReader = pdbReader;
        this.noIL = noIL;
        this.printCompilerGeneratedMembers = printCompilerGeneratedMembers;
        this.csourceEmitterOutput = sourceEmitterOutput;
        if (classPattern is not null) this.classPattern = new Regex(classPattern, RegexOptions.Singleline | RegexOptions.Compiled);
        if (methodPattern is not null) this.methodPattern = new Regex(methodPattern, RegexOptions.Singleline | RegexOptions.Compiled);
    }
    #endregion

    #region Methods

    #region Traversers

    public override void TraverseChildren(INamespaceTypeDefinition namespaceTypeDefinition)
    {
        //var td = (ITypeDefinition)namespaceTypeDefinition;
        if (namespaceTypeDefinition.IsClass && classPattern is not null && classPattern.IsMatch(namespaceTypeDefinition.GetName()))
        {
            base.TraverseChildren(namespaceTypeDefinition);
        }
        else if (namespaceTypeDefinition.IsClass && classPattern is not null && !classPattern.IsMatch(namespaceTypeDefinition.GetName()))
        {
            Runtime.Debug("Not traversing class {0} that does not match pattern {1}.", namespaceTypeDefinition.GetName(), classPattern);
            return;
        }
        else
        {
            base.TraverseChildren(namespaceTypeDefinition);
        }
    }

    public override void TraverseChildren(IMethodDefinition method)
    {
        if (methodPattern is not null && methodPattern.IsMatch(method.Name.Value))
        {
            base.TraverseChildren(method);
        }
        else if (methodPattern is not null && !methodPattern.IsMatch(method.Name.Value))
        {
            Runtime.Debug("Not traversing method {0} that does not match pattern {1}.", method.Name, methodPattern);
            return;
        }
        else
        {
            base.TraverseChildren(method);
        }
    }

    public override void TraverseChildren(IPropertyDefinition prop)
    {
        if (methodPattern is not null && methodPattern.IsMatch(prop.Name.Value))
        {
            base.TraverseChildren(prop);
        }
        else if (methodPattern is not null && !methodPattern.IsMatch(prop.Name.Value))
        {
            Runtime.Debug("Not traversing property {0} that does not match pattern {1}.", prop.Name, methodPattern);
            return;
        }
        else
        {
            base.TraverseChildren(prop);
        }
    }

    public override void Traverse(IMethodBody methodBody)
    {
        PrintToken(CSharpToken.LeftCurly);
        ISourceMethodBody? sourceMethodBody = (ISourceMethodBody)methodBody;
        if (sourceMethodBody == null)
        {
            var options = DecompilerOptions.Loops;
            if (!this.printCompilerGeneratedMembers)
                options |= (DecompilerOptions.AnonymousDelegates | DecompilerOptions.Iterators);
            sourceMethodBody = new SourceMethodBody(methodBody, this.host, this.pdbReader, this.pdbReader, options);
        }
        if (this.noIL)
            this.Traverse(sourceMethodBody.Block.Statements);
        else
        {
            //this.Traverse(sourceMethodBody.Block);
            //PrintToken(CSharpToken.NewLine);

            //if (this.pdbReader != null)
            //     PrintScopes(methodBody);
            //else
            //    PrintLocals(methodBody.LocalVariables);
            int currentIndex = -1; // a number no index matches
            int gasCost = 0;
            foreach (IOperation operation in methodBody.Operations)
            {
                if (this.pdbReader != null)
                {
                    
                    foreach (IPrimarySourceLocation psloc in this.pdbReader.GetPrimarySourceLocationsFor(operation.Location))
                    {
                        if (psloc.StartIndex != currentIndex)
                        {
                            PrintSourceLocation(psloc);
                            currentIndex = psloc.StartIndex;
                        }
                    } 
                }
                PrintOperation(operation);
                if (operation.OperationCode == OperationCode.Call || operation.OperationCode == OperationCode.Callvirt)
                {
                    gasCost += 5;
                }
                else
                {
                    gasCost += 1;
                }
            }
            PrintInstructionCount(methodBody.Operations);
            PrintTotalGasCost(gasCost);
        }
        PrintToken(CSharpToken.RightCurly);
    }
    #endregion

    #region Printers
    private void PrintOperation(IOperation operation)
    {
        csourceEmitterOutput.Write("IL_" + operation.Offset.ToString("x4") + ": ", true, Color.Blue);
        csourceEmitterOutput.Write(operation.OperationCode.ToString(), Color.Magenta);
        if (operation is ILocalDefinition ld)
            csourceEmitterOutput.Write(" " + this.GetLocalName(ld), Color.Red);
        else if (operation.Value is string)
            csourceEmitterOutput.Write(" \"" + operation.Value + "\"", Color.Brown);
        else if (operation.Value is not null)
        {
            if (OperationCode.Br_S <= operation.OperationCode && operation.OperationCode <= OperationCode.Blt_Un)
                sourceEmitterOutput.Write(" IL_" + ((uint)operation.Value).ToString("x4"));
            else if (operation.OperationCode == OperationCode.Switch)
            {
                foreach (uint i in (uint[])operation.Value)
                    sourceEmitterOutput.Write(" IL_" + i.ToString("x4"));
            }
            else if (operation.Value is Microsoft.Cci.MutableCodeModel.MethodDefinition md)
            {
                string mdt = md.Name.Value.StartsWith("get_") || md.Name.Value.StartsWith("set_") ? (md.Name.Value.StartsWith("get_") ? "get" : "set") : "method";
                csourceEmitterOutput.Write($" [{mdt}] ", Color.Cyan);
                csourceEmitterOutput.Write(md.Type.ToString() + " ", Color.Cyan);
                csourceEmitterOutput.Write($"{md.ContainingTypeDefinition.GetName()}.", Color.Pink);
                csourceEmitterOutput.Write(md.Name.Value.Replace("get_", "").Replace("set_", ""), Color.Yellow);
                if (md.Parameters is not null && md.Parameters.Any())
                {
                    csourceEmitterOutput.Write("(" + md.Parameters.Select(p => p.Type.ToString() ?? "").JoinWith(",") + ")", Color.LimeGreen);
                }
                else
                {
                    csourceEmitterOutput.Write("()");
                }
            }
            else if (operation.Value is Microsoft.Cci.MutableCodeModel.MethodReference mr)
            {
                string mrt = mr.Name.Value.StartsWith("get_") || mr.Name.Value.StartsWith("set_") ? (mr.Name.Value.StartsWith("get_") ? "get" : "set") : "method";
                csourceEmitterOutput.Write($" [{mrt}] ", Color.Cyan);
                csourceEmitterOutput.Write(mr.Type.ToString() + " ", Color.Cyan);
                csourceEmitterOutput.Write($"{mr.ContainingType.ToString()}.", Color.Pink);
                csourceEmitterOutput.Write(mr.Name.Value.Replace("get_", "").Replace("set_", ""), Color.Yellow);
                if (mr.Parameters is not null && mr.Parameters.Any())
                {
                    csourceEmitterOutput.Write("(" + mr.Parameters.Select(p => p.Type.ToString()!).JoinWith(", ") + ")", Color.LimeGreen);
                }
                else
                {
                    csourceEmitterOutput.Write("()");
                }
            }
            else if (operation.Value is Microsoft.Cci.MutableCodeModel.FieldDefinition fd)
            {
                csourceEmitterOutput.Write(" [field] ", Color.Cyan);
                csourceEmitterOutput.Write(fd.Type.ToString() + " ", Color.Cyan);
                csourceEmitterOutput.Write(fd.ContainingTypeDefinition.GetName() + ".", Color.Pink);
                csourceEmitterOutput.Write(fd.Name.ToString()!, Color.Yellow);

            }
            else if (operation.Value is Microsoft.Cci.MutableCodeModel.FieldReference fr)
            {
                csourceEmitterOutput.Write(" [field] ", Color.Cyan);
                csourceEmitterOutput.Write(fr.Type.ToString() + " ", Color.Cyan);
                csourceEmitterOutput.Write(fr.ContainingType.ToString() + ".", Color.Pink);
                csourceEmitterOutput.Write(fr.Name.ToString()!, Color.Yellow);

            }
            else if (operation.Value is Microsoft.Cci.MutableCodeModel.ParameterDefinition pd)
            {
                csourceEmitterOutput.Write(" [param] ", Color.Cyan);
                csourceEmitterOutput.Write(pd.Type.ToString() + " ", Color.Pink);
                csourceEmitterOutput.Write(pd.Name.ToString()!, Color.Yellow);

            }
            else if (operation.Value is Microsoft.Cci.MutableCodeModel.LocalDefinition _ld)
            {
                csourceEmitterOutput.Write(" [var] ", Color.Cyan);
                csourceEmitterOutput.Write(_ld.Type.ToString() + " ", Color.Cyan);
                csourceEmitterOutput.Write(_ld.Name.ToString()!, Color.Yellow);
            }
            else if (operation.Value.GetType().FullName == "Microsoft.Cci.CodeModelToIL.TemporaryVariable")
            {
                
            }
            else
            {
                csourceEmitterOutput.Write(/*operation.Value.GetType().ToString() +*/ " " + operation.Value);
            }
        }
        sourceEmitterOutput.WriteLine("", false);
    }
    public override bool PrintToken(CSharpToken token)
    {
        switch (token)
        {
            #region Punctuation
            case CSharpToken.Assign:
                sourceEmitterOutput.Write("=");
                break;
            case CSharpToken.NewLine:
                sourceEmitterOutput.WriteLine("");
                break;
            case CSharpToken.Indent:
                sourceEmitterOutput.Write("", true);
                break;
            case CSharpToken.Space:
                sourceEmitterOutput.Write(" ");
                break;
            case CSharpToken.Dot:
                sourceEmitterOutput.Write(".");
                break;
            case CSharpToken.LeftCurly:
                if (this.LeftCurlyOnNewLine)
                {
                    if (!this.sourceEmitterOutput.CurrentLineEmpty)
                        PrintToken(CSharpToken.NewLine);
                }
                else
                {
                    PrintToken(CSharpToken.Space);
                }
                sourceEmitterOutput.WriteLine("{", this.LeftCurlyOnNewLine);
                sourceEmitterOutput.IncreaseIndent();
                break;
            case CSharpToken.RightCurly:
                sourceEmitterOutput.DecreaseIndent();
                sourceEmitterOutput.WriteLine("}", true);
                break;
            case CSharpToken.LeftParenthesis:
                sourceEmitterOutput.Write("(");
                break;
            case CSharpToken.RightParenthesis:
                sourceEmitterOutput.Write(")");
                break;
            case CSharpToken.LeftAngleBracket:
                sourceEmitterOutput.Write("<");
                break;
            case CSharpToken.RightAngleBracket:
                sourceEmitterOutput.Write(">");
                break;
            case CSharpToken.LeftSquareBracket:
                sourceEmitterOutput.Write("[");
                break;
            case CSharpToken.RightSquareBracket:
                sourceEmitterOutput.Write("]");
                break;
            case CSharpToken.Semicolon:
                sourceEmitterOutput.WriteLine(";");
                break;
            case CSharpToken.Colon:
                sourceEmitterOutput.Write(":");
                break;
            case CSharpToken.Comma:
                sourceEmitterOutput.Write(",");
                break;
            case CSharpToken.Tilde:
                sourceEmitterOutput.Write("~");
                break;
            #endregion

            #region Keywords
            case CSharpToken.Public:
                csourceEmitterOutput.Write("public ", Color.DarkBlue);
                break;
            case CSharpToken.Private:
                csourceEmitterOutput.Write("private ", Color.DarkBlue);
                break;
            case CSharpToken.Internal:
                csourceEmitterOutput.Write("internal ");
                break;
            case CSharpToken.Protected:
                csourceEmitterOutput.Write("protected ", Color.DarkBlue);
                break;
            case CSharpToken.Static:
                csourceEmitterOutput.Write("static ", Color.DarkBlue);
                break;
            case CSharpToken.Abstract:
                csourceEmitterOutput.Write("abstract ", Color.DarkBlue);
                break;
            case CSharpToken.Extern:
                csourceEmitterOutput.Write("extern ", Color.DarkBlue);
                break;
            case CSharpToken.Unsafe:
                csourceEmitterOutput.Write("unsafe ", Color.DarkBlue);
                break;
            case CSharpToken.ReadOnly:
                csourceEmitterOutput.Write("readonly ", Color.DarkBlue);
                break;
            case CSharpToken.Fixed:
                csourceEmitterOutput.Write("fixed ", Color.DarkBlue);
                break;
            case CSharpToken.New:
                csourceEmitterOutput.Write("new ", Color.DarkBlue);
                break;
            case CSharpToken.Sealed:
                csourceEmitterOutput.Write("sealed ", Color.DarkBlue);
                break;
            case CSharpToken.Virtual:
                csourceEmitterOutput.Write("virtual ", Color.DarkBlue);
                break;
            case CSharpToken.Override:
                csourceEmitterOutput.Write("override ", Color.DarkBlue);
                break;
            case CSharpToken.Class:
                csourceEmitterOutput.Write("class ", Color.DarkBlue);
                break;
            case CSharpToken.Interface:
                csourceEmitterOutput.Write("interface ", Color.DarkBlue);
                break;
            case CSharpToken.Struct:
                csourceEmitterOutput.Write("struct ", Color.DarkBlue);
                break;
            case CSharpToken.Enum:
                csourceEmitterOutput.Write("enum ", Color.DarkBlue);
                break;
            case CSharpToken.Delegate:
                csourceEmitterOutput.Write("delegate ", Color.DarkBlue);
                break;
            case CSharpToken.Event:
                csourceEmitterOutput.Write("event ", Color.DarkBlue);
                break;
            case CSharpToken.Namespace:
                csourceEmitterOutput.Write("namespace ", Color.DarkBlue);
                break;
            case CSharpToken.Null:
                csourceEmitterOutput.Write("null", Color.DarkBlue);
                break;
            case CSharpToken.In:
                csourceEmitterOutput.Write("in ", Color.DarkBlue);
                break;
            case CSharpToken.Out:
                csourceEmitterOutput.Write("out ", Color.DarkBlue);
                break;
            case CSharpToken.Ref:
                csourceEmitterOutput.Write("ref ", Color.DarkBlue);
                break;
            #endregion

            #region Primitives
            case CSharpToken.Boolean:
                csourceEmitterOutput.Write("boolean ", Color.DarkBlue);
                break;
            case CSharpToken.Byte:
                csourceEmitterOutput.Write("byte ", Color.DarkBlue);
                break;
            case CSharpToken.Char:
                csourceEmitterOutput.Write("char ", Color.DarkBlue);
                break;
            case CSharpToken.Double:
                csourceEmitterOutput.Write("double ", Color.DarkBlue);
                break;
            case CSharpToken.Short:
                csourceEmitterOutput.Write("short ", Color.DarkBlue);
                break;
            case CSharpToken.Int:
                csourceEmitterOutput.Write("int ", Color.DarkBlue);
                break;
            case CSharpToken.Long:
                csourceEmitterOutput.Write("long ", Color.DarkBlue);
                break;
            case CSharpToken.Object:
                csourceEmitterOutput.Write("object ", Color.DarkBlue);
                break;
            case CSharpToken.String:
                csourceEmitterOutput.Write("string ", Color.DarkBlue);
                break;
            case CSharpToken.UShort:
                csourceEmitterOutput.Write("ushort ", Color.DarkBlue);
                break;
            case CSharpToken.UInt:
                csourceEmitterOutput.Write("uint ", Color.DarkBlue);
                break;
            case CSharpToken.ULong:
                csourceEmitterOutput.Write("ulong ", Color.DarkBlue);
                break;
            #endregion

            #region Statements
            case CSharpToken.Get:
                csourceEmitterOutput.Write("get", Color.DarkBlue);
                break;
            case CSharpToken.Set:
                csourceEmitterOutput.Write("set", Color.DarkBlue);
                break;
            case CSharpToken.Add:
                csourceEmitterOutput.Write("add", Color.DarkBlue);
                break;
            case CSharpToken.Remove:
                csourceEmitterOutput.Write("remove", Color.DarkBlue);
                break;
            case CSharpToken.Return:
                csourceEmitterOutput.Write("return", Color.DarkBlue);
                break;
            case CSharpToken.This:
                csourceEmitterOutput.Write("this", Color.DarkBlue);
                break;
            case CSharpToken.Throw:
                csourceEmitterOutput.Write("throw", Color.DarkBlue);
                break;
            case CSharpToken.Try:
                csourceEmitterOutput.Write("try", Color.DarkBlue);
                break;
            case CSharpToken.YieldReturn:
                csourceEmitterOutput.Write("yield return", Color.DarkBlue);
                break;
            case CSharpToken.YieldBreak:
                csourceEmitterOutput.Write("yield break", Color.DarkBlue);
                break;
            case CSharpToken.TypeOf:
                csourceEmitterOutput.Write("typeof", Color.DarkBlue);
                break;
            default:
                csourceEmitterOutput.Write("Unknown-token", Color.DarkBlue);
                break;
            #endregion

            #region Constants
            case CSharpToken.True:
                csourceEmitterOutput.Write("true");
                break;
            case CSharpToken.False:
                csourceEmitterOutput.Write("false");
                break;
            #endregion
        }
        return true;
    }

    public override void PrintIdentifier(IName name) => csourceEmitterOutput.Write(EscapeIdentifier(name.Value), Color.Yellow);

    public override void PrintTypeReferenceName(ITypeReference typeReference)
    {
        Contract.Requires(typeReference != null);

        var typeName = TypeHelper.GetTypeName(typeReference,
          NameFormattingOptions.ContractNullable | NameFormattingOptions.UseTypeKeywords |
          NameFormattingOptions.TypeParameters | NameFormattingOptions.EmptyTypeParameterList |
          NameFormattingOptions.OmitCustomModifiers);
        csourceEmitterOutput.Write(typeName, Color.Cyan);
    }
    private void PrintScopes(IMethodBody methodBody)
    {
        if (this.pdbReader is not null)
        {
            foreach (ILocalScope scope in this.pdbReader.GetLocalScopes(methodBody))
                PrintScopes(scope);
        }
    }

    private void PrintScopes(ILocalScope scope)
    {
        if (this.pdbReader is not null)
        {
            sourceEmitterOutput.Write(string.Format("IL_{0} ... IL_{1} ", scope.Offset.ToString("x4"), (scope.Offset + scope.Length).ToString("x4")), true);
            sourceEmitterOutput.WriteLine("{");
            sourceEmitterOutput.IncreaseIndent();
            PrintConstants(this.pdbReader.GetConstantsInScope(scope));
            PrintLocals(this.pdbReader.GetVariablesInScope(scope));
            sourceEmitterOutput.DecreaseIndent();
            sourceEmitterOutput.WriteLine("}", true);
        }
    }

    private void PrintConstants(IEnumerable<ILocalDefinition> locals)
    {
        foreach (ILocalDefinition local in locals)
        {
            sourceEmitterOutput.Write("const ", true);
            PrintTypeReference(local.Type);
            sourceEmitterOutput.WriteLine(" " + this.GetLocalName(local));
        }
    }

    private void PrintLocals(IEnumerable<ILocalDefinition> locals)
    {
        foreach (ILocalDefinition local in locals)
        {
            sourceEmitterOutput.Write("", true);
            PrintTypeReference(local.Type);
            sourceEmitterOutput.WriteLine(" " + this.GetLocalName(local));
        }
    }

    public override void PrintLocalName(ILocalDefinition local)
    {
        this.sourceEmitterOutput.Write(this.GetLocalName(local));
    }

    private void PrintSourceLocation(IPrimarySourceLocation psloc)
    {
        var source = psloc.Source.Trim() != "{" ? " " + psloc.Source.TrimEnd(';') : "";
        csourceEmitterOutput.WriteLine(psloc.Document.Name.Value + "(" + psloc.StartLine + ":" + psloc.StartColumn + ")-(" + psloc.EndLine + ":" + psloc.EndColumn + ")" + source + ":", true, Color.Red);
        //if (psloc.Source.Length > 1)
        //{
        //csourceEmitterOutput.WriteLine(psloc.Source, true);
        //}
        //else
        //{
        //    csourceEmitterOutput.WriteLine("");
        //}
    }

    private void PrintInstructionCount(IEnumerable<IOperation> operations)
    {
        csourceEmitterOutput.Write("Total instructions in method: ",true, Color.Red);
        csourceEmitterOutput.WriteLine(operations.Count().ToString(), Color.Magenta);
    }

    private void PrintTotalGasCost(int gasCount)
    {
        csourceEmitterOutput.Write($"Total gas cost: ", true, Color.Red);
        csourceEmitterOutput.WriteLine(gasCount.ToString(), Color.Magenta);
    }
    #endregion

    private string GetLocalName(ILocalDefinition local)
    {
        string localName = local.Name.Value;
        if (this.pdbReader != null)
        {
            foreach (IPrimarySourceLocation psloc in this.pdbReader.GetPrimarySourceLocationsForDefinitionOf(local))
            {
                if (psloc.Source.Length > 0)
                {
                    localName = psloc.Source;
                    break;
                }
            }
        }
        return localName;
    }
    #endregion

    #region Fields
    IMetadataHost host;
    PdbReader? pdbReader;
    bool noIL;
    IColorfulSourceEmitterOutput csourceEmitterOutput;
    Regex? classPattern;
    Regex? methodPattern;
    #endregion
}
