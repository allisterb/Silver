using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.Projects
{
    public readonly record struct CompilerError(string File, string Code, string Msg);

    public readonly record struct CompilerWarning(string File, string Code, string Msg);
    public class SilverProjectCompilation
    {
        #region Constructor
        public SilverProjectCompilation(SilverProject proj, bool succeded, bool verify, List<CompilerError> errors, List<CompilerWarning> warnings)
        {
            Project = proj;
            Succeded = succeded;
            Verify = verify;
            Errors = errors;
        }
        #endregion
        #region Properties
        public SilverProject Project { get; init; }
        
        public bool Succeded { get; init; }

        public bool Verify { get; init; }

        public List<CompilerError> Errors {get; init; }
        #endregion
    }
}
