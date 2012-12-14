using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    public abstract class HillClimbSearcher : ISearcher
    {
        public virtual Route Search( Graph graph, bool printProgress = false )
        {
            Route route = Route.CreateDefault( graph );
            Improve( route, printProgress );
            return route;
        }

        public virtual void Improve( Route route, bool printProgress = false )
        {
            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
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
            {
                Console.WriteLine( "\nPeak reached" );
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        protected abstract bool Iterate( Route route );
    }
}
