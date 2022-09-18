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


public class TmHighlightedCode : Runtime
{
    #region Constructors
    public TmHighlightedCode(string lang, string code)
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
    public string DrawWithShiki(string id)
    {
        var html = new StringBuilder();
        html.AppendLine($"<div id=\"{id}\">");
        html.AppendLine("</div>");
        html.AppendLine($"<script>$('#{id}').html(highlighter.codeToHtml(\"{this.Code}\", \"{this.Lang}\", \"nord\"));</script>");
        return html.ToString();
    }
    #endregion
}

