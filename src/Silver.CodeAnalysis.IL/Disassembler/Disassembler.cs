namespace Silver.CodeAnalysis.IL;


using CSharpSourceEmitter;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MetadataReader;

public class Disassembler : Runtime
{
    public static string? Print(string fileName)
    {
        using var host = new PeReader.DefaultHost();
        
        IModule? module = host.LoadUnitFrom(FailIfFileNotFound(fileName)) as IModule;
        if (module is null || module is Dummy)
        {
            Error( "{0} is not a PE file containing a CLR module or assembly.", fileName);
            return null;
        }

        string pdbFile = Path.ChangeExtension(module.Location, "pdb");       
        using var pdbReader = new PdbReader(fileName, pdbFile, host, true);
        
        var options = DecompilerOptions.AnonymousDelegates | DecompilerOptions.Iterators | DecompilerOptions.Loops;
        options |= DecompilerOptions.Unstack;
        module = Decompiler.GetCodeModelFromMetadataModel(host, module, pdbReader, options);
        SourceEmitterOutputString sourceEmitterOutput = new SourceEmitterOutputString();
        SourceEmitter csSourceEmitter = new PeToText.SourceEmitter(sourceEmitterOutput, host, pdbReader, false, printCompilerGeneratedMembers: true);
        csSourceEmitter.Traverse(module);
        return sourceEmitterOutput.Data;
    }
}

