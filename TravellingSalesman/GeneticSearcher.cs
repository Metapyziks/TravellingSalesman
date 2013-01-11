using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class GeneticSearcher : HillClimbSearcher
    {
        private Random _rand;

        private GeneticRoute[] _selected;
        private GeneticRoute[] _genePool;

        public int GenePoolCount { get; set; }
        public int SelectionCount { get; set; }
        public int OffspringCount { get; set; }
        public int GenerationLimit { get; set; }

        public double SelectionQuality { get; set; }
        public double CrossoverSwapProbability { get; set; }
        public double MutationChance { get; set; }
        public double BitFlipChance { get; set; }

        public GeneticSearcher()
            : this(0x743bc365) { }

        public GeneticSearcher(int seed)
        {
            _rand = new Random(seed);

            GenePoolCount = 512;
            SelectionCount = 2;
            OffspringCount = 2;
            GenerationLimit = int.MaxValue;

            SelectionQuality = 4.0 / 5.0;
            CrossoverSwapProbability = 1.0 / 2.0;
            MutationChance = 4.0 / 5.0;
            BitFlipChance = 1.0 / 2.0;
        }

        private bool IsSelected(int i, int j)
        {
            Route route = _genePool[j];
            while (--i >= 0) {
                if (_selected[i] == route) return true;
            }

            return false;
        }

        public override void Improve(Route route, bool printProgress = false)
        {
            _genePool = new GeneticRoute[GenePoolCount];
            _selected = new GeneticRoute[SelectionCount];

            if (printProgress) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Starting a new {0} search", GetType().Name);
                Console.WriteLine("Creating initial population...");
                Console.Write("Progress: 0/{0}", GenePoolCount);
                Console.ForegroundColor = ConsoleColor.White;
            }

            ReversingSearcher improver = new ReversingSearcher();

            _genePool[0] = new GeneticRoute(route);
            _genePool[0].Fitness = route.Length;
            for (int i = 1; i < GenePoolCount; ++i) {
                _genePool[i] = new GeneticRoute(route.Graph, _rand);
                Route clone = new Route(_genePool[i]);
                improver.Improve(clone, false);
                _genePool[i].Fitness = clone.Length;
                if (printProgress) {
                    Console.CursorLeft = 10;
                    Console.Write("{0}/{1}", i + 1, GenePoolCount);
                }
            }

            if (printProgress) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("Initial population created");
                Console.WriteLine("Running generations...");
                Console.Write("Progress: 0/{0} - 0", GenerationLimit);
                Console.ForegroundColor = ConsoleColor.White;
            }

            for (int g = 0; g < GenerationLimit; ++g) {
                if (printProgress) {
                    Console.CursorLeft = 10;
                    Console.Write("{0}/{1} - {2}   ", g + 1, GenerationLimit, _genePool[0].Fitness);
                }

                for (int i = 0; i < SelectionCount; ++i) {
                    int j = 0;
                    while (IsSelected(i, j) || _rand.NextDouble() >= SelectionQuality) {
                        j = (j + 1) % GenePoolCount;
                    }
                    _selected[i] = _genePool[j];
                }

                for (int i = 0; i < OffspringCount; ++i) {
                    GeneticRoute parentA = _selected[0]; // TODO: fix for different 
                    GeneticRoute parentB = _selected[1]; //       selection counts
                    ushort[] genes = new ushort[route.Count];
                    Crossover(genes, parentA.Genes, parentB.Genes);
                    Mutate(genes);
                    GeneticRoute child = new GeneticRoute(route.Graph, genes);
                    Route clone = new Route(child);
                    improver.Improve(clone, false);
                    child.Fitness = clone.Length;

                    int l = -1;
                    for (int j = GenePoolCount - 1; j >= 0; --j) {
                        GeneticRoute other = _genePool[j];
                        if (other.Fitness > child.Fitness) {
                            _genePool[j] = child;

                            if(l > -1)
                                _genePool[l] = other;

                            if (j == 0)
                                g = 0;

                            l = j;
                        } else break;
                    }
                }
            }

            if (printProgress) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nGenetic search complete");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Route best = new Route(_genePool[0]);
            improver.Improve(best);

            route.Clear();
            for (int i = 0; i < best.Count; ++i)
                route.Insert(route.VIndexOf(best[i]), i);
        }

        public override bool Iterate(Route route)
        {
            throw new NotImplementedException();
        }

        protected virtual void Crossover(ushort[] dest, ushort[] parentA, ushort[] parentB)
        {
            bool readFromA = _rand.NextDouble() < 0.5;
            for (int i = 0; i < parentA.Length; ++i) {
                dest[i] = (readFromA ? parentA[i] : parentB[i]);

                if (_rand.NextDouble() < CrossoverSwapProbability / dest.Length)
                    readFromA = !readFromA;
            }
        }

        protected virtual void Mutate(ushort[] genes)
        {
            for (int i = 0; i < genes.Length; ++i) {
                ushort flip = 0;
                if (_rand.NextDouble() < MutationChance / genes.Length)
                    while (++flip < 65535 && _rand.NextDouble() < MutationChance);
                genes[i] ^= flip;
            }
        }

        protected virtual double FindFitness(Route route)
        {
            return route.Length * (double) Math.Abs(route.Count - route.Graph.Count) / route.Graph.Count;
        }
    }
}
