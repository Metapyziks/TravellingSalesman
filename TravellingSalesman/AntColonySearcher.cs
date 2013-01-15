using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TravellingSalesman
{
    public class AntStepEventArgs : EventArgs
    {
        public readonly int[,] Paths;

        public AntStepEventArgs(int[,] paths)
        {
            Paths = paths;
        }
    }

    public class AntColonySearcher<T> : ISearcher
        where T : Ant
    {
        public int AntCount { get; set; }
        public int StepCount { get; set; }

        public event EventHandler<BetterRouteFoundEventArgs> BetterRouteFound;
        public event EventHandler<AntStepEventArgs> AntStep;

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

            int minimum = 0;
            for (int i = 0; i < graph.Count; ++i) {
                int vmin = -1;
                for (int j = 0; j < graph.Count; ++j) {
                    if (i == j) continue;
                    if (graph[i, j] < vmin || vmin == -1)
                        vmin = graph[i, j];
                }
                minimum += vmin;
            }

            var ants = new T[AntCount];

            var paths = new int[AntCount, 2];

            var args = new Object[] { graph, 0 };
            var phms = new double[graph.Count, graph.Count];
            Route best = null;
            int bestLength = 0, tours = 0;
            for (int step = 0; step < StepCount; ++step) {
                for (int a = AntCount - 1; a >= 0; --a) {
                    T ant = ants[a];
                    if (ant == null) {
                        args[1] = a % graph.Count;
                        ants[a] = (T) ctor.Invoke(args);
                        break;
                    }

                    paths[a, 0] = ant.CurrentVertex;
                    int cost = ant.Step(phms, tours);
                    paths[a, 1] = ant.CurrentVertex;
                    if (cost > -1) {
                        ant.FortifyLastRoute(phms, minimum, tours);
                        if (best == null || cost < bestLength) {
                            best = new Route(graph, ant.History, graph.Count);
                            bestLength = cost;
                            if (BetterRouteFound != null)
                                BetterRouteFound(this, new BetterRouteFoundEventArgs(best));
                            step = -1;
                            ++tours;
                        }
                    }
                }

                if (AntStep != null)
                    AntStep(this, new AntStepEventArgs(paths));

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
