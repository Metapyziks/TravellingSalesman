using System;
using System.IO;

namespace LaTeXGraphGen
{
    abstract class LaTeXElement
    {
        public abstract void WriteLaTeX(TextWriter writer);

        public String ToLaTeX()
        {
            using(var stream = new MemoryStream()){
                var writer = new StreamWriter(stream);
                WriteLaTeX(writer);
                writer.Flush();
                return BitConverter.ToString(stream.GetBuffer());
            }
        }
    }
}
