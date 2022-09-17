namespace Silver.Drawing;

public class HighlightedCode
{
    #region Constructors
    public HighlightedCode(string lang, string code)
    {
        Lang = lang;
        Code = code;
    }
    #endregion

    #region Properties
    public string Lang { get; set; }
    public string Code { get; set; }
    #endregion

    #region Methods
    
    #endregion

    #region Overriden members
    public override string ToString()
    {
        return Code;
    }
    #endregion
}


public class TmHighlightedCode : HighlightedCode
{
    public TmHighlightedCode(string lang, string code, string grammarUrl) :base(lang, code)
    {
        GrammarUrl = grammarUrl;
    }

    #region Properties
    public string GrammarUrl { get; set; }
    #endregion
}

