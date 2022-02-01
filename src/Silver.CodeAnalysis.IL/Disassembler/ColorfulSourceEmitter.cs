// Based on PeToText.CSharpSourceEmitter
// Pe2Text.CSharpSourceEmitter has the following copyright notice 
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

namespace Silver.CodeAnalysis.IL;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics.Contracts;
using System.IO;
using CSharpSourceEmitter;
using Microsoft.Cci;
using Microsoft.Cci.MetadataReader;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.Contracts;

public class ColorfulSourceEmitter : SourceEmitter
{
    #region Constructor
    public ColorfulSourceEmitter(IColorfulSourceEmitterOutput sourceEmitterOutput, IMetadataHost host, PdbReader/*?*/ pdbReader, bool noIL, bool printCompilerGeneratedMembers)
      : base(sourceEmitterOutput)
    {
        this.host = host;
        this.pdbReader = pdbReader;
        this.noIL = noIL;
        this.printCompilerGeneratedMembers = printCompilerGeneratedMembers;
        this.csourceEmitterOutput = sourceEmitterOutput;
    }
    #endregion

    #region Overriden members
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
                //PrintScopes(methodBody);
            //else
                //PrintLocals(methodBody.LocalVariables);

            int currentIndex = -1; // a number no index matches
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
            }
        }

        PrintToken(CSharpToken.RightCurly);
    }
    #endregion

    #region Methods

    #region Printers
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
                sourceEmitterOutput.Write("public ");
                break;
            case CSharpToken.Private:
                sourceEmitterOutput.Write("private ");
                break;
            case CSharpToken.Internal:
                sourceEmitterOutput.Write("internal ");
                break;
            case CSharpToken.Protected:
                sourceEmitterOutput.Write("protected ");
                break;
            case CSharpToken.Static:
                sourceEmitterOutput.Write("static ");
                break;
            case CSharpToken.Abstract:
                sourceEmitterOutput.Write("abstract ");
                break;
            case CSharpToken.Extern:
                sourceEmitterOutput.Write("extern ");
                break;
            case CSharpToken.Unsafe:
                sourceEmitterOutput.Write("unsafe ");
                break;
            case CSharpToken.ReadOnly:
                sourceEmitterOutput.Write("readonly ");
                break;
            case CSharpToken.Fixed:
                sourceEmitterOutput.Write("fixed ");
                break;
            case CSharpToken.New:
                sourceEmitterOutput.Write("new ");
                break;
            case CSharpToken.Sealed:
                sourceEmitterOutput.Write("sealed ");
                break;
            case CSharpToken.Virtual:
                sourceEmitterOutput.Write("virtual ");
                break;
            case CSharpToken.Override:
                sourceEmitterOutput.Write("override ");
                break;
            case CSharpToken.Class:
                sourceEmitterOutput.Write("class ");
                break;
            case CSharpToken.Interface:
                sourceEmitterOutput.Write("interface ");
                break;
            case CSharpToken.Struct:
                sourceEmitterOutput.Write("struct ");
                break;
            case CSharpToken.Enum:
                sourceEmitterOutput.Write("enum ");
                break;
            case CSharpToken.Delegate:
                sourceEmitterOutput.Write("delegate ");
                break;
            case CSharpToken.Event:
                sourceEmitterOutput.Write("event ");
                break;
            case CSharpToken.Namespace:
                sourceEmitterOutput.Write("namespace ");
                break;
            case CSharpToken.Null:
                sourceEmitterOutput.Write("null");
                break;
            case CSharpToken.In:
                sourceEmitterOutput.Write("in ");
                break;
            case CSharpToken.Out:
                sourceEmitterOutput.Write("out ");
                break;
            case CSharpToken.Ref:
                sourceEmitterOutput.Write("ref ");
                break;
            #endregion

            #region Directives
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
                csourceEmitterOutput.Write("set");
                break;
            case CSharpToken.Add:
                csourceEmitterOutput.Write("add");
                break;
            case CSharpToken.Remove:
                csourceEmitterOutput.Write("remove");
                break;
            case CSharpToken.Return:
                csourceEmitterOutput.Write("return");
                break;
            case CSharpToken.This:
                csourceEmitterOutput.Write("this");
                break;
            case CSharpToken.Throw:
                csourceEmitterOutput.Write("throw");
                break;
            case CSharpToken.Try:
                csourceEmitterOutput.Write("try");
                break;
            case CSharpToken.YieldReturn:
                csourceEmitterOutput.Write("yield return");
                break;
            case CSharpToken.YieldBreak:
                csourceEmitterOutput.Write("yield break");
                break;
            
                break;
            case CSharpToken.TypeOf:
                csourceEmitterOutput.Write("typeof");
                break;
            default:
                csourceEmitterOutput.Write("Unknown-token");
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
        csourceEmitterOutput.Write(typeName, Color.Green);
    }
    private void PrintScopes(IMethodBody methodBody)
    {
        foreach (ILocalScope scope in this.pdbReader.GetLocalScopes(methodBody))
            PrintScopes(scope);
    }

    private void PrintScopes(ILocalScope scope)
    {
        sourceEmitterOutput.Write(string.Format("IL_{0} ... IL_{1} ", scope.Offset.ToString("x4"), (scope.Offset + scope.Length).ToString("x4")), true);
        sourceEmitterOutput.WriteLine("{");
        sourceEmitterOutput.IncreaseIndent();
        PrintConstants(this.pdbReader.GetConstantsInScope(scope));
        PrintLocals(this.pdbReader.GetVariablesInScope(scope));
        sourceEmitterOutput.DecreaseIndent();
        sourceEmitterOutput.WriteLine("}", true);
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

    private void PrintOperation(IOperation operation)
    {
        csourceEmitterOutput.Write("IL_" + operation.Offset.ToString("x4") + ": ", true, Color.Blue);
        csourceEmitterOutput.Write(operation.OperationCode.ToString(), Color.Magenta);
        if (operation is ILocalDefinition ld)
            csourceEmitterOutput.Write(" " + this.GetLocalName(ld), Color.Red);
        else if (operation.Value is string)
            sourceEmitterOutput.Write(" \"" + operation.Value + "\"");
        else if (operation.Value != null)
        {
            if (OperationCode.Br_S <= operation.OperationCode && operation.OperationCode <= OperationCode.Blt_Un)
                sourceEmitterOutput.Write(" IL_" + ((uint)operation.Value).ToString("x4"));
            else if (operation.OperationCode == OperationCode.Switch)
            {
                foreach (uint i in (uint[])operation.Value)
                    sourceEmitterOutput.Write(" IL_" + i.ToString("x4"));
            }
            else
                sourceEmitterOutput.Write(" " + operation.Value);
        }
        sourceEmitterOutput.WriteLine("", false);
    }

    private void PrintSourceLocation(IPrimarySourceLocation psloc)
    {
        csourceEmitterOutput.WriteLine("");
        csourceEmitterOutput.Write(psloc.Document.Name.Value + "(" + psloc.StartLine + ":" + psloc.StartColumn + ")-(" + psloc.EndLine + ":" + psloc.EndColumn + "): ", true, Color.Red);
        csourceEmitterOutput.WriteLine(psloc.Source);
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
    PdbReader/*?*/ pdbReader;
    bool noIL;
    IColorfulSourceEmitterOutput csourceEmitterOutput;
    #endregion
}
