using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class GeneticSearcher : ISearcher
    {

        private Random _rand;

        public double CrossoverSwapProbability { get; set; }

        public GeneticSearcher()
        {
            _rand = new Random();
            CrossoverSwapProbability = 0.125;
        }

        public GeneticSearcher( int seed )
        {
            _rand = new Random( seed );
            CrossoverSwapProbability = 0.125;
        }

        public Route Search( Graph graph, bool printProgress = false )
        {
            GeneticRoute route = CreateInitialRoute( graph );

            for ( int i = 0; i < graph.Count; ++i )
            {
                int next = route.SelectNextBest( graph.Count - i - 1 );
                route.AddEnd( next );
            }

            return route;
        }

        protected virtual GeneticRoute CreateInitialRoute( Graph graph )
        {
            int[] indices = new int[graph.Count];
            for ( int i = 0; i < graph.Count; ++i )
                indices[i] = i;

            return new GeneticRoute( graph );
        }

        protected virtual GeneticRoute CreateFromGenes( Graph graph, byte[] genes )
        {
            return new GeneticRoute( graph, genes );
        }

        protected virtual byte[] Crossover( byte[] parentA, byte[] parentB )
        {
            byte[] child = new byte[parentA.Length];

            bool readFromA = _rand.NextDouble() < 0.5;
            for ( int i = 0; i < parentA.Length; ++i )
            {
                child[i] = ( readFromA ? parentA[i] : parentB[i] );

                if ( _rand.NextDouble() < CrossoverSwapProbability )
                    readFromA = !readFromA;
            }

            return child;
        }

        protected virtual void Mutate( byte[] genes )
        {
            return;
        }

        protected virtual double FindFitness( Route route )
        {
            return route.Length * (double) Math.Abs( route.Count - route.Graph.Count ) / route.Graph.Count;
        }
    }
}
