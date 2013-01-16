using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTeXGraphGen
{
    class LaTeXTable : LaTeXElement
    {
        public enum Justification : byte
        {
            L = 1, C = 2, R = 3,
            P = 4 | L,
            M = 4 | C,
            B = 4 | R
        }

        public class ColumnSpec : LaTeXElement
        {
            public bool LeftLine = false;
            public bool RightLine = false;

            public Justification Justification = Justification.C;

            public int ParagraphWidth = 0;

            public ColumnSpec() {}

            public ColumnSpec(Justification justification,
                int width = 0, bool leftLine = false, bool rightLine = false)
            {
                LeftLine = leftLine;
                RightLine = rightLine;

                Justification = justification;
                ParagraphWidth = width;
            }

            public override void WriteLaTeX(System.IO.TextWriter writer)
            {
                if (LeftLine) writer.Write("|");
                writer.Write(" ");
                writer.Write(Justification.ToString().ToLower());
                if (((byte) Justification & 4) != 0) {
                    writer.Write("{'" + ParagraphWidth + "mm'}");
                }
                writer.Write(" ");
                if (RightLine) writer.Write("|");
            }
        }

        private ColumnSpec[] _colSpecs;
        private List<List<LaTeXElement>> _rows;
        private List<int> _hlines;

        public int Columns { get { return _colSpecs.Length; } }
        public int Rows { get { return _rows.Count; } }

        public LaTeXTable(params ColumnSpec[] columns)
        {
            _colSpecs = columns;
            _rows = new List<List<LaTeXElement>>();
            _hlines = new List<int>();
        }

        public int AddHLine()
        {
            _hlines.Add(_rows.Count);
            return _rows.Count;
        }

        public int AddRow(params LaTeXElement[] columns)
        {
            _rows.Add(columns.ToList());
            return _rows.Count - 1;
        }

        public int AddRow(params String[] columns)
        {
            _rows.Add(columns.Select(x => (LaTeXElement) new LaTeXString(x)).ToList());
            return _rows.Count - 1;
        }

        public override void WriteLaTeX(System.IO.TextWriter writer)
        {
            writer.Write("\\begin{tabular}{");
            foreach (var col in _colSpecs)
                col.WriteLaTeX(writer);
            writer.WriteLine("}");
            int hi = 0, l = 0;
            Action<int> checkLine = line => {
                while (hi < _hlines.Count && _hlines[hi] == line) {
                    writer.WriteLine("\\hline");
                    ++hi;
                }
            };

            checkLine(l);
            foreach (var row in _rows) {
                foreach (var col in row) {
                    col.WriteLaTeX(writer);
                    if (col != row.Last()) writer.Write(" & ");
                }
                writer.WriteLine(" \\\\");
                checkLine(++l);
            }
            writer.WriteLine("\\end{tabular}");
        }
    }
}
