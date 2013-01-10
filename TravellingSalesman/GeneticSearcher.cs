using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class GeneticSearcher : HillClimbSearcher
    {
        private Random _rand;

        private GeneticRoute[] _genePool;

        public int GenePoolCount { get; set; }
        public int SelectionCount { get; set; }
        public int CorruptedCount { get; set; }
        public int GenerationLimit { get; set; }

        public double CrossoverSwapProbability { get; set; }
        public double BitFlipChance { get; set; }
        public double BadSelectionChance { get; set; }

        public GeneticSearcher()
            : this( 0x743bc365 ) { }

        public GeneticSearcher( int seed )
        {
            _rand = new Random( seed );

            GenePoolCount = 32;
            SelectionCount = 8;
            CorruptedCount = 2;
            GenerationLimit = 1024;

            CrossoverSwapProbability = 1.0 / 16.0;
            BitFlipChance = 1.0 / 2.0;
            BadSelectionChance = 1.0 / 32.0;
        }

        public override void Improve( Route route, bool printProgress = false )
        {
            _genePool = new GeneticRoute[GenePoolCount];

            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine( "Starting a new {0} search", GetType().Name );
                Console.Write( "Progress: 0/{0} - 0", GenerationLimit );
                Console.ForegroundColor = ConsoleColor.White;
            }

            _genePool[0] = new GeneticRoute( route );

            for ( int i = 1; i < GenePoolCount; ++i )
                _genePool[i] = new GeneticRoute( route.Graph, _rand );

            for ( int g = 0; g < GenerationLimit; ++g )
            {
                if ( printProgress )
                {
                    Console.CursorLeft = 10;
                    Console.Write( "{0}/{1} - {2}:{3}   ", g, GenerationLimit,
                        _genePool[0].Length, _genePool[SelectionCount - CorruptedCount - 1].Length );
                }

                for ( int i = SelectionCount; i < GenePoolCount; ++i )
                {
                    GeneticRoute child = _genePool[i];
                    GeneticRoute parentA = _genePool[_rand.Next( SelectionCount / 2 )];
                    GeneticRoute parentB = _genePool[_rand.Next( SelectionCount / 2, SelectionCount )];
                    Crossover( child.Genes, parentA.Genes, parentB.Genes );

                    Mutate( child.Genes );
                    child.UpdateIndicesFromGenes();
                }

                for ( int i = SelectionCount; i < GenePoolCount; ++i )
                {
                    GeneticRoute curnt = _genePool[i];
                    int l = i;
                    for ( int s = SelectionCount - 1; s >= 0; --s )
                    {
                        GeneticRoute other = _genePool[s];
                        if ( other.Length > curnt.Length
                            || ( s >= SelectionCount - CorruptedCount && _rand.NextDouble() < BadSelectionChance ) )
                        {
                            _genePool[s] = curnt;
                            _genePool[l] = other;

                            l = s;

                            if( s < SelectionCount - CorruptedCount )
                                g = 0;
                        }
                        else break;
                    }
                }

                for ( int i = SelectionCount - CorruptedCount / 2; i < SelectionCount; ++i )
                    _genePool[i] = new GeneticRoute( route.Graph, _rand );
            }

            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine( "\nGenetic search complete" );
                Console.ForegroundColor = ConsoleColor.White;
            }

            Route clone = new Route( _genePool[0] );

            new ReversingSearcher().Improve( clone, false );

            route.Clear();
            for ( int i = 0; i < clone.Count; ++i )
                route.Insert( route.VIndexOf( clone[i] ), i );
        }

        public override bool Iterate( Route route )
        {
            throw new NotImplementedException();
        }

        protected virtual void Crossover(ushort[] dest, ushort[] parentA, ushort[] parentB)
        {
            bool readFromA = _rand.NextDouble() < 0.5;
            for ( int i = 0; i < parentA.Length; ++i )
            {
                dest[i] = ( readFromA ? parentA[i] : parentB[i] );

                if ( _rand.NextDouble() < CrossoverSwapProbability )
                    readFromA = !readFromA;
            }
        }

        protected virtual void Mutate(ushort[] genes)
        {
            double flipChance = BitFlipChance / genes.Length;

            for (int i = 0; i < genes.Length; ++i) {
                ushort flip = 0;
                while (_rand.NextDouble() < flipChance && ++flip < 65535) // just in case... :P
                    ++flip;
                genes[i] ^= flip;
            }
        }

        protected virtual double FindFitness( Route route )
        {
            return route.Length * (double) Math.Abs( route.Count - route.Graph.Count ) / route.Graph.Count;
        }
    }
}
