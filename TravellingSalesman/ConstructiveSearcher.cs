using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    public abstract class ConstructiveSearcher : ISearcher
    {
        public HillClimbSearcher Improver { get; private set; }

        public ConstructiveSearcher( HillClimbSearcher improver = null )
        {
            Improver = improver;
        }

        public Route Search( Graph graph, bool printProgress = false )
        {
            Route route = new Route( graph );

            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine( "Starting a new {0} search", GetType().Name );
                if ( Improver != null )
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine( "Search will use {0} to "
                    + "improve each iteration", Improver.GetType().Name );
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write( "Progress: 0/{0} - 0", graph.Count );
                Console.ForegroundColor = ConsoleColor.White;
            }

            int lastCount = route.Count;
            while ( route.Count < graph.Count )
            {
                int vIndex = ChooseNext( route );
                route.Insert( vIndex, ChooseIndex( route, vIndex ) );

                lastCount = route.Count;

                if ( Improver != null )
                    Improver.Improve( route, false );

                if ( printProgress )
                {
                    Console.CursorLeft = 10;
                    Console.Write( "{0}/{1} - {2}    ",
                        route.Count, graph.Count, route.Length );
                }
            }

            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine( "\n{0} search complete", GetType().Name );
                Console.ForegroundColor = ConsoleColor.White;
            }

            return route;
        }

        protected abstract int ChooseNext( Route route );
        protected abstract int ChooseIndex( Route route, int vIndex );
    }
}
