namespace Silver.Notebooks;

using System;

public class Resources
{
    public static bool JSTreeCssLoaded { get; set; } = false;

    public static bool ShikiLibLoaded { get; set; } = false;

    public static bool ShikiRendererLibLoaded { get; set; } = false;

    public static bool ShikiLanguagesLoaded { get; set; } = false;

    public static bool PrismLibLoaded { get; set; } = false;

    public static bool PrismLanguagesLoaded { get; set; } = false;
    
    public static bool PrismCssLoaded { get; set; } = false;

    public static string GenerateScriptElement(string uri, string tag)
    {
        var id = Guid.NewGuid().ToString("N");
        return $@"<script>
    //if (!!document.getElementById('{tag}')) {{
        let require_script_{id} = document.createElement('script');
        require_script_{id}.setAttribute('src', '{uri}');
        require_script_{id}.setAttribute('type', 'text/javascript');
        document.getElementsByTagName('head')[0].appendChild(require_script_{id});
        let tag_{id} = document.createElement('span');
        tag_{id}.setAttribute('id', '{tag}');
        document.getElementsByTagName('body')[0].appendChild(tag_{id});
    //}}
    </script>
    ";
    }
}

