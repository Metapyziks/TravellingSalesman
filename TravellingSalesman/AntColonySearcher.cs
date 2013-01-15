using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

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
        public int Threads { get; set; }

        public event EventHandler<BetterRouteFoundEventArgs> BetterRouteFound;
        public event EventHandler<AntStepEventArgs> AntStep;

        public AntColonySearcher()
        {
            AntCount = 1024;
            StepCount = 65536;
            Threads = 1;
        }

        public Route Search(Graph graph, bool printProgress = false)
        {
            var ctor = typeof(T).GetConstructor(new Type[] { typeof(Graph), typeof(int) });

            if (ctor == null) {
                throw new Exception("Ant class must have constructor of form new(Graph, int)");
            }

            if (printProgress) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("# Starting a new {0} search with {1} thread{2}", GetType().Name,
                    Threads, Threads != 1 ? "s" : "");
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

            var args = new Object[] { graph, 0 };
            Route best = null;
            int bestLength = 0, tours = 0;
            int step = 0;
            int exited = 0;

            Func<bool> next = () => {
                lock (this) {
                    if (step < StepCount) {
                        ++step;
                        return true;
                    }

                    return false;
                }
            };

            Func<bool> hasNext = () => {
                lock (this)
                    return step < StepCount;
            };

            Func<bool> ended = () => {
                lock (this)
                    return exited == Threads;
            };

            Action exit = () => {
                lock (this)
                    ++exited;
            };

            Action rejoin = () => {
                lock (this)
                    --exited;
            };

            Func<bool> waitForEnd = () => {
                exit();

                while (!hasNext() && !ended())
                    Thread.Yield();

                if (!ended()) {
                    rejoin();
                    return false;
                } else
                    return true;
            };

            Action<T, int> compare = (ant, cost) => {
                lock (this) {
                    if (best == null || cost < bestLength) {
                        best = new Route(graph, ant.History, graph.Count);
                        bestLength = cost;
                        step = 0;
                        ++tours;

                        if (BetterRouteFound != null)
                            BetterRouteFound(this, new BetterRouteFoundEventArgs(best));
                    }
                }
            };

            Thread mainThread = Thread.CurrentThread;

            ThreadStart loop = () => {
                var ants = new T[AntCount / Threads];
                var phms = new double[graph.Count, graph.Count];

                for (; ; ) {
                    while (next()) {
                        for (int a = ants.Length - 1; a >= 0; --a) {
                            T ant = ants[a];
                            if (ant == null) {
                                args[1] = a % graph.Count;
                                ants[a] = (T) ctor.Invoke(args);
                                break;
                            }

                            int cost = ant.Step(phms, tours);
                            if (cost > -1) {
                                lock (phms) {
                                    ant.FortifyLastRoute(phms, minimum, tours + 1);
                                }
                                compare(ant, cost);
                            }
                        }

                        if (Thread.CurrentThread == mainThread && printProgress) {
                            Console.CursorLeft = 10;
                            Console.Write("{0}/{1} - {2}    ", step + 1, StepCount, bestLength);
                        }
                    }

                    if (waitForEnd())
                        break;
                }
            };
            
            Thread[] workers = new Thread[Threads - 1];
            for (int i = 0; i < workers.Length; ++i) {
                workers[i] = new Thread(loop);
                workers[i].Start();
            }

            loop();

            if (printProgress) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n{0} search complete", GetType().Name);
                Console.ForegroundColor = ConsoleColor.White;
            }

            return best;
        }
    }
}
