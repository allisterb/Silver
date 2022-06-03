using System;
using System.IO;
namespace Silver.Notebooks
{
    public class DotLanguage
    {
        #region Constructors
        public DotLanguage(string code, string width = "100%", string height = "600px")
        {

            Code = code ?? throw new ArgumentNullException(nameof(code));
            Width = width;
            Height = height;

        }
        #endregion

        #region Properties
        internal string Code { get; set; }
        internal string Width { get; set; }
        internal string Height { get; set; }
        #endregion

        #region Methods
        public static DotLanguage LoadFrom(string f) => new DotLanguage(File.ReadAllText(f))!;
        #endregion
    
        #region Overriden members
        public override string ToString()
        {
            return Code;
        }
        #endregion
    }
}
