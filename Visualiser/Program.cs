using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            if (args.Length == 0) {
                // Console.WriteLine("usage: Visualiser.exe <cityfile>");
                // return;

                args = new string[] {
                    "cityfiles" + DSC + "SAfile535.txt",
                    "gvnj58" + DSC + "TourfileA" + DSC + "tourSAfile535.txt"
                };
            }

            if (!File.Exists(args[0])) {
                Console.WriteLine("file \"{0}\" does not exist", args[0]);
                return;
            }

            VisualiserWindow window = new VisualiserWindow(800, 600);
            window.Graph = PositionalGraph.FromFile(args[0]);

            Thread thread = null;

            if (args.Length > 1 && File.Exists(args[1])) {
                window.Graph.CurrentRoute = Route.FromFile(window.Graph, args[1]);
                window.Graph.GuessStartPositions();
            } else {
                StochasticHillClimbSearcher searcher = new StochasticHillClimbSearcher(new ReversingSearcher());
                searcher.Attempts = Int32.MaxValue;
                searcher.Threads = 2;

                thread = new Thread(() => {
                    searcher.BetterRouteFound += (sender, e) => {
                        window.Graph.CurrentRoute = new Route(e.Route);
                        window.Graph.GuessStartPositions();
                    };
                    searcher.Search(window.Graph, false);
                });

                thread.Start();
            }

            window.Run();
            window.Dispose();

            if (thread != null) thread.Abort();
        }
    }
}
