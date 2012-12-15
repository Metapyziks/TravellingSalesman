using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    class EitherOrSearcher : CompositeSearcher
    {
        public EitherOrSearcher( params ISearcher[] searchers )
            : base( searchers ) { }

        public override Route Search( Graph graph, bool printProgress = false )
        {
            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine( "Selecting the best result from {0} different searchers...", Searchers.Length );
                Console.ForegroundColor = ConsoleColor.White;
            }

            ISearcher bestSearcher = Searchers[0];
            Route best = Searchers[0].Search( graph, printProgress );

            for ( int i = 1; i < Searchers.Length; ++i )
            {
                Route next = Searchers[i].Search( graph, printProgress );
                if ( next.Length < best.Length )
                {
                    bestSearcher = Searchers[i];
                    best = next;
                }
            }

            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine( "And the winner is: {0}!", bestSearcher.GetType().Name );
                Console.ForegroundColor = ConsoleColor.White;
            }

            return best;
        }
    }
}
