using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.CodeAnalysis.IL;

public enum DisassemblerMode
{
    IL,
    TAC,
    CS,
    BOOGIE
}
public class DisassemblerOptions
{
    public DisassemblerMode Mode { get; set; }

    public string? ClassNamePattern { get; set; }
    
    public string? MethodNamePattern { get; set; }
}

