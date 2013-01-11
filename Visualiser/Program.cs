using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TravellingSalesman;

namespace Visualiser
{
    class Program
    {
        static readonly char DSC = Path.DirectorySeparatorChar;

        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            if (args.Length == 0) {
                // Console.WriteLine("usage: Visualiser.exe <cityfile>");
                // return;

                args = new string[] {
                    "gvnj58" + DSC + "TourfileA" + DSC + "tourSAfile535.txt"
                };
            }

            if (!File.Exists(args[0])) {
                Console.WriteLine("file \"{0}\" does not exist", args[0]);
                return;
            }

            Match match = Regex.Match(args[0], "SAfile[0-9]{3}");

            if (!match.Success) {
                Console.WriteLine("invalid tour filename", args[0]);
                return;
            }

            String graphPath = "cityfiles" + DSC + match.Value + ".txt";

            var graph = PositionalGraph.FromFile(graphPath);
            var route = Route.FromFile(graph, args[0]);


            var window = new VisualiserWindow(800, 600);
            window.Graph = graph;

            window.Graph.CurrentRoute = route;
            window.Graph.GuessStartPositions();
            
            var searcher = new AntColonySearcher<Ant>();
            var thread = new Thread(() => {
                searcher.BetterRouteFound += (sender, e) => {
                    graph.CurrentRoute = e.Route;
                    window.UpdateTitle();
                };
                searcher.AntStep += (sender, e) => {
                    graph.HighlightedEdges = e.Paths;
                };
                searcher.Search(graph, false);
            });

            thread.Start();

            window.Run();
            window.Dispose();

            thread.Abort();
        }
    }
}
