﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    class EitherOrSearcher : ISearcher
    {
        public ISearcher[] Searchers { get; private set; }

        public EitherOrSearcher( params ISearcher[] searchers )
        {
            Searchers = searchers;
        }

        public Route Search( Graph graph, bool printProgress = false )
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
                Console.WriteLine( "And the winner is: {0} ({1})!", bestSearcher.GetType().Name, best.Length );
                Console.ForegroundColor = ConsoleColor.White;
            }

            return best;
        }
    }
}
