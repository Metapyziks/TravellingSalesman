using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class GeneticSearcher : ISearcher
    {
        private Random _rand;

        private GeneticRoute[] _genePool;

        public int GenePoolCount { get; set; }
        public int SelectionCount { get; set; }
        public int GenerationLimit { get; set; }

        public double CrossoverSwapProbability { get; set; }
        public double BitFlipChance { get; set; }

        public GeneticSearcher()
            : this( 0x743bc365 ) { }

        public GeneticSearcher( int seed )
        {
            _rand = new Random( seed );

            GenePoolCount = 64;
            SelectionCount = 8;
            GenerationLimit = 1024;

            CrossoverSwapProbability = 1.0 / 8.0;
            BitFlipChance = 1.0;
        }

        public Route Search( Graph graph, bool printProgress = false )
        {
            _genePool = new GeneticRoute[GenePoolCount];

            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine( "Starting a new {0} search", GetType().Name );
                Console.Write( "Progress: 0/{0} - 0", GenerationLimit );
                Console.ForegroundColor = ConsoleColor.White;
            }

            for ( int i = 0; i < GenePoolCount; ++i )
                _genePool[i] = new GeneticRoute( graph, _rand );

            for ( int g = 0; g < GenerationLimit; ++g )
            {
                for ( int i = SelectionCount; i < GenePoolCount; ++ i )
                {
                    GeneticRoute child = _genePool[i];
                    GeneticRoute parentA = _genePool[_rand.Next( SelectionCount / 2 )];
                    GeneticRoute parentB = _genePool[_rand.Next( SelectionCount / 2, SelectionCount )];
                    Crossover( child.Genes, parentA.Genes, parentB.Genes );

                    Mutate( child.Genes );
                    child.UpdateFromGenes();
                }

                for ( int i = SelectionCount; i < GenePoolCount; ++i )
                {
                    GeneticRoute route = _genePool[i];
                    int l = i;
                    for ( int s = SelectionCount - 1; s >= 0; --s )
                    {
                        GeneticRoute other = _genePool[s];
                        if ( other.Length > route.Length )
                        {
                            _genePool[s] = route;
                            _genePool[l] = other;
                        }
                        else break;
                        l = s;
                        g = 0;
                    }
                }

                if ( printProgress )
                {
                    Console.CursorLeft = 10;
                    Console.Write( "{0}/{1} - {2}    ", g, GenerationLimit, _genePool[0].Length );
                }
            }

            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine( "\nGenetic search complete" );
                Console.ForegroundColor = ConsoleColor.White;
            }

            Route clone = new Route( _genePool[0] );

            new ReversingSearcher().Improve( clone, false );

            return clone;
        }

        protected virtual void Crossover( byte[] dest, byte[] parentA, byte[] parentB )
        {
            bool readFromA = _rand.NextDouble() < 0.5;
            for ( int i = 0; i < parentA.Length; ++i )
            {
                dest[i] = ( readFromA ? parentA[i] : parentB[i] );

                if ( _rand.NextDouble() < CrossoverSwapProbability )
                    readFromA = !readFromA;
            }
        }

        protected virtual void Mutate( byte[] genes )
        {
            double flipChance = BitFlipChance / genes.Length;

            for ( int i = 0; i < genes.Length; ++i ) for ( int b = 0; b < 8; ++b )
                if ( _rand.NextDouble() < flipChance )
                    genes[i] ^= (byte) ( 1 << b );
        }

        protected virtual double FindFitness( Route route )
        {
            return route.Length * (double) Math.Abs( route.Count - route.Graph.Count ) / route.Graph.Count;
        }
    }
}
