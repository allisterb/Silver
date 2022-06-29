namespace Silver.Drawing;

using System;
using System.IO;

public class MermaidLanguage
{
    public string Background { get; set; }
    public string Width { get; set; }
    public string Height { get; set; }
    public override string ToString()
    {
        return _value;
    }

    private readonly string _value;

    public MermaidLanguage(string value)
    {
        Background = "white";
        Width = string.Empty;
        Height = string.Empty;
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static MermaidLanguage LoadFrom(string f) => new MermaidLanguage(File.ReadAllText(f))!;
}

