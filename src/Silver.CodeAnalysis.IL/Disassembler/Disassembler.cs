namespace Silver.CodeAnalysis.IL;

using CSharpSourceEmitter;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MetadataReader;

public class Disassembler : Runtime
{
    public static void Run(string fileName, ISourceEmitterOutput output, bool noIL = false, bool noStack = true, bool colorful = false)
    {
        using var host = new PeReader.DefaultHost();
        IModule? module = host.LoadUnitFrom(FailIfFileNotFound(fileName)) as IModule;
        if (module is null || module is Dummy)
        {
            Error( "{0} is not a PE file containing a CLR module or assembly.", fileName);
            return;
        }
        string pdbFile = Path.ChangeExtension(module.Location, "pdb");       
        using var pdbReader = new PdbReader(fileName, pdbFile, host, true);
        var options = DecompilerOptions.AnonymousDelegates | DecompilerOptions.Iterators | DecompilerOptions.Loops;
        if (noStack)
        {
            options |= DecompilerOptions.Unstack;
        }
        module = Decompiler.GetCodeModelFromMetadataModel(host, module, pdbReader, options);

        if (!colorful)
        {
            var sourceEmitter = new PeToText.SourceEmitter(output, host, pdbReader, noIL, printCompilerGeneratedMembers: true);
            sourceEmitter.Traverse(module);
        }
        else
        {
            var sourceEmitter = new ColorfulSourceEmitter((IColorfulSourceEmitterOutput) output, host, pdbReader, noIL, printCompilerGeneratedMembers: true);
            sourceEmitter.Traverse(module);
        }
    }
}

