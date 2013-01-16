using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TravellingSalesman;

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
                RightLine = true,
                Justification = LaTeXTable.Justification.R
            });

            table.AddHLine();
            table.AddRow("City File", "Algorithm A", "Algorithm B");
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
                table.AddRow(graph.Name, routeA.Length.ToString(), routeB.Length.ToString());
            }
            table.AddHLine();

            using (var stream = new FileStream("graphtest.tex", FileMode.Create, FileAccess.Write)) {
                var writer = new StreamWriter(stream);
                table.WriteLaTeX(writer);
                writer.Flush();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
