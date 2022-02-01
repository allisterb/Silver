namespace Silver.CodeAnalysis.IL;

using CSharpSourceEmitter;
using System.Drawing;
public interface IColorfulSourceEmitterOutput : ISourceEmitterOutput
{
    void WriteLine(string str, bool fIndent, Color color);
    void WriteLine(string str, Color color);
    void Write(string str, bool fIndent, Color color);
    void Write(string str, Color color);
}

