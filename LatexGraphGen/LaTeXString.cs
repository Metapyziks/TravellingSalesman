using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTeXGraphGen
{
    class LaTeXString : LaTeXElement
    {
        public String String { get; private set; }

        public LaTeXString(String str)
        {
            String = str;
        }

        public override void WriteLaTeX(System.IO.TextWriter writer)
        {
            writer.Write(String);
        }
    }
}
