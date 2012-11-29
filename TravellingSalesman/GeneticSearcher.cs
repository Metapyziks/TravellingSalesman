using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class GeneticSearcher : ISearcher
    {
        private static List<int> _sBitCountCache = new List<int>();

        protected static int FindBitCount( int size )
        {
            if ( size < 0 )
                return 0;

            if ( _sBitCountCache.Count <= size )
            {
                int curr = ( ( size < 2 ) ? 0 :
                    (int) Math.Ceiling( Math.Log( size, 2 ) ) )
                    + FindBitCount( size - 1 );

                _sBitCountCache.Add( curr );
            }

            return _sBitCountCache[size];
        }

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
                int nextVal = route.GetFromSelectionBuffer( next );
                if( i > 0 )
                    Console.WriteLine( "{0}: {1} ({2})", i, nextVal, route.Graph[route[route.Count - 1], nextVal] );
                else
                    Console.WriteLine( "{0}: {1} ({2})", i, nextVal, 0 );
                route.AddEnd( next );
            }

            return route;
        }

        protected virtual byte[] FindGenes( Route route )
        {
            byte[] genes = new byte[FindBitCount( route.Count )];
            for ( int i = 0; i < route.Count; ++i )
            {

            }

            return new byte[0];
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
