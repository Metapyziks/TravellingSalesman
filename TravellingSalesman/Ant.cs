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
            for (int i = _stepNo - 1; i >= 0; --i) {
                pheromones[History[i], History[(i + 1) % _stepNo]] += 1d / _cost;
            }
            _stepNo = 0;
        }
         
        private void StartNewTour()
        {
            History[0] = CurrentVertex;

            for (int i = Graph.Count - 1; i >= 0; --i) {
                _visited[i] = false;
            }

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

        protected bool HasVisited(int vertex)
        {
            return _visited[vertex];
        }

        protected virtual double FindScore(int vertex, double[,] pheromones, int tours, double phWeight)
        {
            return Math.Sqrt(pheromones[CurrentVertex, vertex] / tours * phWeight + 1d)
                / (Graph[CurrentVertex, vertex]);
        }

        protected virtual int ChooseNext(double[,] pheromones, int tours)
        {
            int b = -1;
            double s = 0;
            double phWeight = Rand.NextDouble();
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
