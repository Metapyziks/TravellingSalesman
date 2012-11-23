using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    abstract class HillClimbSearcher : ISearcher
    {
        public Route Search( Graph graph, bool printProgress = false )
        {
            Route route = Route.CreateDefault( graph );
            Improve( route, printProgress );
            return route;
        }

        public void Improve( Route route, bool printProgress = false )
        {
            if ( printProgress )
            {
                Console.WriteLine( "# Starting a new {0} with route of length {1}", GetType().Name, route.Length );
                Console.Write( "Current best: {0}", route.Length );
            }

            if ( route.Count > 3 )
            {
                while ( Iterate( route ) )
                {
                    if ( printProgress )
                    {
                        Console.CursorLeft = 14;
                        Console.Write( "{0}    ", route.Length );
                    }
                }
            }

            if ( printProgress )
                Console.WriteLine( "\nPeak reached" );
        }

        protected abstract bool Iterate( Route route );
    }
}
