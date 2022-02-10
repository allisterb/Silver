namespace Silver.Compiler
{
    public readonly record struct SscCompilerError(string File, string Code, string Msg);

    public readonly record struct SscCompilerWarning(string File, string Code, string Msg);
    public class SscCompilation
    {
        #region Constructor
        public SscCompilation(SilverProject proj, bool succeded, bool verify, List<SscCompilerError> errors, List<SscCompilerWarning> warnings)
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

        public List<SscCompilerError> Errors {get; init; }
        #endregion
    }
}
