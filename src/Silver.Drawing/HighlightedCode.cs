namespace Silver.Drawing;

public abstract class HighlightedCode
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
    public abstract string Draw(string id);
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
    #region Constructors
    public TmHighlightedCode(string lang, string code) : base(lang, code) 
    {
        
        
    }
    #endregion

    #region Methods
    public override string Draw(string id)
    {
        var html = new StringBuilder();
        html.AppendLine($"<div id=\"{id}\">");
        html.AppendLine("</div>");
        html.AppendLine($"<script>if (highlighter === undefined) {{console.error('Shiki not loaded yet!. Run the notebook init script.')}} else {{$('#{id}').html(highlighter.codeToHtml(\"{System.Web.HttpUtility.JavaScriptStringEncode(this.Code)}\", \"{this.Lang}\", \"light-plus\"));}}</script>");
        return html.ToString();
    }
    #endregion
}

public class PrismHighlightedCode : HighlightedCode
{
    #region Constructors
    public PrismHighlightedCode(string lang, string code) : base(lang, code)
    {

    }
    #endregion

    #region Methods
    public override string Draw(string id)
    {
        var html = new StringBuilder();
        html.AppendLine($"<code id=\"{id}\" class=\"language-{Lang}\">");
        html.AppendLine(Code);
        html.Append("</code>");
        return html.ToString();
    }
    #endregion
}

