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

        protected static readonly Random Rand  = new Random();

        public Graph Graph { get; private set; }
        public int CurrentVertex { get; private set; }
        public int[] History { get; private set; }

        public Ant(Graph graph, int startVertex)
        {
            Graph = graph;
            CurrentVertex = startVertex;
            History = new int[graph.Count];

            _stepNo = 0;
            _cost = 0;
            _visited = new bool[graph.Count];
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
            History[_stepNo++] = CurrentVertex = next;
            pheromones[CurrentVertex, next] += 1d / Graph[CurrentVertex, next];
            pheromones[next, CurrentVertex] += 1d;

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

        public void FortifyLastRoute(double[,] pheromones)
        {
            double add = (double) Graph.Count / _cost;
            int last = 0;
            for (int i = Graph.Count - 1; i >= 0; --i) {
                pheromones[History[i], History[last]] += add;
                pheromones[History[last], History[i]] += add;
                last = i;
            }
        }

        protected bool HasVisited(int vertex)
        {
            return _visited[vertex];
        }

        protected virtual double FindScore(int vertex, double[,] pheromones, int tours, double phWeight)
        {
            return (pheromones[CurrentVertex, vertex] / (tours + 1) * phWeight + 1d)
                / (Graph[CurrentVertex, vertex]);
        }

        protected virtual int ChooseNext(double[,] pheromones, int tours)
        {
            int b = -1;
            double s = 0;
            double phWeight = Rand.NextDouble(); // / (tours + 1);
            for (int i = Graph.Count - 1; i >= 0; --i) {
                if (HasVisited(i)) continue;
                double score = FindScore(i, pheromones, tours, phWeight);
                if (b == -1 || score > s) {
                    b = i;
                    s = score;
                }
            }
            return b;
        }
    }
}
