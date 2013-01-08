using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TravellingSalesman;

namespace Visualiser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) {
                // Console.WriteLine("usage: Visualiser.exe <cityfile>");
                // return;

                args = new string[] {
                    "cityfiles\\SAfile535.txt",
                    "gvnj58\\TourfileA\\tourSAfile535.txt"
                };
            }

            if (!File.Exists(args[0])) {
                Console.WriteLine("file \"{0}\" does not exist", args[0]);
                return;
            }

            VisualiserWindow window = new VisualiserWindow(800, 600);
            window.Graph = PositionalGraph.FromFile(args[0]);

            if (args.Length > 1 && File.Exists(args[1])) {
                window.Graph.CurrentRoute = Route.FromFile(window.Graph, args[1]);
                window.Graph.GuessStartPositions();
            }

            window.Run();
            window.Dispose();
        }
    }
}
