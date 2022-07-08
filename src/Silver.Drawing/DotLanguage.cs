namespace Silver.Notebooks;

using System;
using System.IO;

public class DotLanguage
{
    #region Constructors
    public DotLanguage(string code, string width = "100%", string height = "600px")
    {
        Code = code;
        Width = width;
        Height = height;
    }
    #endregion

    #region Properties
    public string Code { get; set; }
    public string Width { get; set; }
    public string Height { get; set; }
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

