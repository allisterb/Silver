namespace Silver.CLI.Core;

using System.Drawing;
using CSharpSourceEmitter;

using Silver.CodeAnalysis.IL;

public class ColorfulConsoleSourceEmitterOutput : IColorfulSourceEmitterOutput
{
    #region Constructors
    public ColorfulConsoleSourceEmitterOutput(int indentSize)
    {
        this.indentLevel = indentSize;
        this.strIndent = "";
        this.CurrentLineEmpty = true;
    }

    public ColorfulConsoleSourceEmitterOutput() : this(4) { }
    #endregion

    #region Methods
    public void WriteLine(string str, bool fIndent, Color color)
    {
        OutputBegin(fIndent);
        CC.WriteLine(str, color);
        //System.Console.WriteLine(str);
        this.CurrentLineEmpty = true;
    }

    public void WriteLine(string str, Color color)
    {
        this.WriteLine(str, false, color);
    }

    public void Write(string str, bool fIndent, Color color)
    {
        OutputBegin(fIndent);
        CC.Write(str, color);  
        //System.Console.Write(str);
        this.CurrentLineEmpty = false;
    }

    public void Write(string str, Color color)
    {
        this.Write(str, false, color);
    }

    public void WriteLine(string str, bool fIndent) => this.WriteLine(str, fIndent, Color.White);

    public void WriteLine(string str) => this.WriteLine(str, false);

    public void Write(string str, bool fIndent) => this.Write(str, fIndent, Color.White);

    public void Write(string str) => this.Write(str, false);

    public void IncreaseIndent()
    {
        int newIndent = strIndent.Length + indentLevel;
        strIndent = new String(' ', newIndent);
    }

    public void DecreaseIndent()
    {
        int newIndent = strIndent.Length - indentLevel;
        strIndent = new String(' ', newIndent);
    }

    protected void OutputBegin(bool fIndent)
    {
        if (fIndent)
        {
            LineStart?.Invoke(this);
            Con.Write(strIndent);
        }
    }
    #endregion

    #region Properties
    public bool CurrentLineEmpty
    {
        get;
        private set;
    }
    #endregion

    #region Events
    public event Action<ISourceEmitterOutput>? LineStart;
    #endregion

    #region Fields
    protected string strIndent;
    protected readonly int indentLevel;
    #endregion
}



