using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TravellingSalesman;
using System.Diagnostics;

namespace LaTeXGraphGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var table = new LaTeXTable(new LaTeXTable.ColumnSpec {
                LeftLine = true, RightLine = true,
                Justification = LaTeXTable.Justification.R
            }, new LaTeXTable.ColumnSpec {
                Justification = LaTeXTable.Justification.R
            }, new LaTeXTable.ColumnSpec {
                Justification = LaTeXTable.Justification.R
            }, new LaTeXTable.ColumnSpec {
                RightLine = true,
                Justification = LaTeXTable.Justification.R
            });

            table.AddHLine();
            table.AddRow("\\textbf{City File}", "\\textbf{Result A}",
                "\\textbf{Result B}", "\\textbf{Difference}");
            table.AddHLine(); table.AddHLine();
            foreach (var cityFile in Directory.EnumerateFiles("cityfiles")) {
                var graph = Graph.FromFile(cityFile);
                Console.WriteLine("Doing {0}...", graph.Name);
                Route routeA = null, routeB = null;
                var routeAPath = "gvnj58\\TourfileA\\tour" + graph.Name + ".txt";
                var routeBPath = "gvnj58\\TourfileB\\tour" + graph.Name + ".txt";
                if (File.Exists(routeAPath))
                    routeA = Route.FromFile(graph, routeAPath);
                if (File.Exists(routeBPath))
                    routeB = Route.FromFile(graph, routeBPath);
                int diff = routeB.Length - routeA.Length;
                table.AddRow(graph.Name, routeA.Length.ToString(), routeB.Length.ToString(),
                    diff.ToString() + " (" + (100d * diff / routeA.Length).ToString("F2") + "\\%)");
            }
            table.AddHLine();

            using (var stream = new FileStream("resultstable.tex", FileMode.Create, FileAccess.Write)) {
                var writer = new StreamWriter(stream);
                table.WriteLaTeX(writer);
                writer.Flush();
            }

            Console.WriteLine("Compiling...");
            Process.Start("pdflatex", "gvnj58report.tex");
        }
    }
}
