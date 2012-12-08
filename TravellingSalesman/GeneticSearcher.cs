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
            Route route = new Route( graph, new int[0] );

            for ( int i = 0; i < graph.Count; ++i )
            {
                int next = route.SelectNextBest( graph.Count - i - 1 );
                route.AddEnd( next );

                FindGenes( route );
            }

            return route;
        }

        protected virtual byte[] FindGenes( Route route )
        {

            return null;
            /*
            byte[] genes = new byte[FindTotalBitCount( route.Count )];
            for ( int i = 0, j = 0; i < route.Count; ++i )
            {
                int bits = FindBitCount( route.Count );

                for ( int k = j & 0x7; k < bits; k += 8 )
                    genes[k] |= (byte) ( ( ( ( route[i] >> ( k << 8 ) ) << ( j & 0x7 ) ) & 0xff ) );

                j += bits;
            }

            return genes;
            */
        }

        protected virtual Route FindRoute( Graph graph, byte[] genes )
        {
            int bitCount = graph.Count;
            return Route.CreateDefault( graph );
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
