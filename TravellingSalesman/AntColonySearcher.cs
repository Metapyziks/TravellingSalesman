using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TravellingSalesman
{
    public class AntColonySearcher<T> : ISearcher
        where T : Ant
    {
        public int AntCount { get; set; }
        public int StepCount { get; set; }

        public event EventHandler<BetterRouteFoundEventArgs> BetterRouteFound;

        public AntColonySearcher()
        {
            AntCount = 1024;
            StepCount = 65536;
        }

        public Route Search(Graph graph, bool printProgress = false)
        {
            var ctor = typeof(T).GetConstructor(new Type[] { typeof(Graph), typeof(int) });

            if (ctor == null) {
                throw new Exception("Ant class must have constructor of form new(Graph, int)");
            }

            if (printProgress) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("# Starting a new {0}", GetType().Name);
                Console.Write("Progress: {0}/{1} - 0", 0, StepCount);
                Console.ForegroundColor = ConsoleColor.White;
            }

            var ants = new T[AntCount];

            var args = new Object[] { graph, 0 };
            for (int i = 0; i < AntCount; ++i) {
                args[1] = i % graph.Count;
                ants[i] = (T) ctor.Invoke(args);
            }

            var phms = new double[graph.Count, graph.Count];
            Route best = null;
            int bestLength = 0, tours = 0;
            for (int step = 0; step < StepCount; ++step) {
                for (int a = AntCount - 1; a >= 0; --a) {
                    int cost = ants[a].Step(phms, tours);
                    if (cost > -1 && (best == null || cost < bestLength)) {
                        best = new Route(graph, ants[a].History, graph.Count);
                        bestLength = cost;
                        BetterRouteFound(this, new BetterRouteFoundEventArgs(best));
                        ++tours;
                    }
                }

                if (printProgress) {
                    Console.CursorLeft = 10;
                    Console.Write("{0}/{1} - {2}    ", step + 1, StepCount, bestLength);
                }
            }

            if (printProgress) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n{0} search complete", GetType().Name);
                Console.ForegroundColor = ConsoleColor.White;
            }

            return best;
        }
    }
}
