using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TravellingSalesman
{
    public class Ant
    {
        private int _stepNo;
        private int _cost;
        private bool[] _visited;

        private static readonly Random _sRand = new Random();

        public Graph Graph { get; private set; }
        public int CurrentVertex { get; private set; }
        public int[] History { get; private set; }

        private Random _rand;

        public Ant(Graph graph, int startVertex)
        {
            Graph = graph;
            CurrentVertex = startVertex;
            History = new int[graph.Count];

            _stepNo = 0;
            _cost = 0;
            _visited = new bool[graph.Count];

            _rand = new Random(_sRand.Next());
        }

        private void EndCurrentTour(double[,] pheromones)
        {
            _stepNo = 0;
        }

        private void StartNewTour()
        {
            History[0] = CurrentVertex;

            for (int i = Graph.Count - 1; i >= 0; --i) {
                _visited[i] = false;
            }

            _visited[CurrentVertex] = true;
            _cost = 0;
            _stepNo = 1;
        }

        public int Step(double[,] pheromones, int tours)
        {
            if (_stepNo == 0) {
                StartNewTour();
            }

            int next = ChooseNext(pheromones, tours);
            Debug.Assert(!HasVisited(next), "Can't revisit a vertex in the current tour");
            _visited[next] = true;
            // pheromones[CurrentVertex, next] += 1d / Graph[CurrentVertex, next];
            // pheromones[next, CurrentVertex] += 1d / Graph[CurrentVertex, next];
            History[_stepNo++] = CurrentVertex = next;

            if (_stepNo > 1) {
                _cost += Graph[History[_stepNo - 2], CurrentVertex];

                if (_stepNo == Graph.Count) {
                    _cost += Graph[CurrentVertex, History[0]];
                    EndCurrentTour(pheromones);
                    return _cost;
                }
            }

            return -1;
        }

        public void FortifyLastRoute(double[,] pheromones, int min, int max)
        {
            double add = (double) Graph.Count / (_cost - min);
            int last = 0;
            for (int i = Graph.Count - 1; i >= 0; --i) {
                pheromones[History[i], History[last]] += add;

                if (pheromones[History[i], History[last]] > max)
                    pheromones[History[i], History[last]] = max;

                pheromones[History[last], History[i]] = pheromones[History[i], History[last]];

                last = i;
            }
        }

        protected bool HasVisited(int vertex)
        {
            return _visited[vertex];
        }

        protected virtual double FindScore(int vertex, double[,] pheromones, int tours, double phWeight)
        {
            return phWeight * (pheromones[CurrentVertex, vertex] + 1d) +
                (1d - phWeight ) * (tours + 1d) / (Graph[CurrentVertex, vertex]);
        }

        protected virtual int ChooseNext(double[,] pheromones, int tours)
        {
            double l = double.MaxValue;
            double phWeight = _rand.NextDouble(); // / (tours + 1);
            int b;
            do {
                b = -1;
                double s = 0;
                for (int i = Graph.Count - 1; i >= 0; --i) {
                    if (HasVisited(i)) continue;
                    double score = FindScore(i, pheromones, tours, phWeight);
                    if (b == -1 || (score > s && score < l)) {
                        b = i;
                        s = score;
                    }
                }
                l = s;
            } while (_rand.NextDouble() < 1d / Graph.Count);
            return b;
        }
    }
}
