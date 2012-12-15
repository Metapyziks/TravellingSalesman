using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    class StochasticHillClimbSearcher : ISearcher
    {
        private Random _rand;

        public HillClimbSearcher Searcher { get; private set; }

        public int Attempts { get; set; }

        public StochasticHillClimbSearcher( HillClimbSearcher searcher, int seed = 0 )
        {
            if ( seed == 0 )
                _rand = new Random();
            else
                _rand = new Random( seed );

            Searcher = searcher;

            Attempts = 256;
        }

        public Route Search( Graph graph, bool printProgress = false )
        {
            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine( "Starting a new {0} search", GetType().Name );
                
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine( "Search will use {0} for each iteration", Searcher.GetType().Name );

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write( "Progress: 0/{0} - 0", Attempts );
                Console.ForegroundColor = ConsoleColor.White;
            }

            Route best = null;
            for ( int a = 0; a < Attempts; ++a )
            {
                Route route = Route.CreateRandom( graph, _rand );
                Searcher.Improve( route );
                if ( best == null || route.Length < best.Length )
                {
                    best = route;
                    a = 0;
                }

                if ( printProgress )
                {
                    Console.CursorLeft = 10;
                    Console.Write( "{0}/{1} - {2}    ", a, Attempts, best.Length );
                }
            }

            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine( "\n{0} search complete", GetType().Name );
                Console.ForegroundColor = ConsoleColor.White;
            }

            return best;
        }
    }
}
