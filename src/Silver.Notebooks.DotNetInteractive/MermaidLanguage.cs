using System;
using System.IO;

namespace Silver.Notebooks
{
    public class MermaidLanguage
    {
        internal string Background { get; set; }
        internal string Width { get; set; }
        internal string Height { get; set; }
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
}
