using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
