using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TravellingSalesman
{
    public class BetterRouteFoundEventArgs : EventArgs
    {
        public readonly Route Route;

        public BetterRouteFoundEventArgs( Route route )
        {
            Route = route;
        }
    }

    public class StochasticHillClimbSearcher : ISearcher
    {
        private Random _rand;

        public HillClimbSearcher Searcher { get; private set; }

        public int Attempts { get; set; }
        public int Threads { get; set; }

        public event EventHandler<BetterRouteFoundEventArgs> BetterRouteFound;

        public StochasticHillClimbSearcher( HillClimbSearcher searcher, int seed = 0 )
        {
            if ( seed == 0 )
                _rand = new Random();
            else
                _rand = new Random( seed );

            Searcher = searcher;

            Attempts = 256;
            Threads = 1;
        }

        public Route Search( Graph graph, bool printProgress = false )
        {
            if ( printProgress )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine( "Starting a new {0} search with {1} thread{2}", GetType().Name,
                    Threads, Threads != 1 ? "s" : "" );
                
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine( "Search will use {0} for each iteration", Searcher.GetType().Name );

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write( "Progress: 0/{0} - 0", Attempts );
                Console.ForegroundColor = ConsoleColor.White;
            }

            Route best = null;
            int attempt = 0;
            int exited = 0;
            
            Func<bool> next = () =>
            {
                lock ( this )
                {
                    if ( attempt < Attempts )
                    {
                        ++attempt;
                        return true;
                    }

                    return false;
                }
            };

            Func<bool> hasNext = () =>
            {
                lock ( this )
                    return attempt < Attempts;
            };

            Func<bool> ended = () =>
            {
                lock( this )
                    return exited == Threads;
            };

            Action exit = () =>
            {
                lock ( this )
                    ++ exited;
            };

            Action rejoin = () =>
            {
                lock ( this )
                    --exited;
            };

            Func<bool> waitForEnd = () =>
            {
                exit();

                while ( !hasNext() && !ended() )
                    Thread.Yield();

                if ( !ended() )
                {
                    rejoin();
                    return false;
                }
                else
                    return true;
            };

            Action<Route> compare = ( route ) =>
            {
                lock ( this )
                {
                    if ( best == null || route.Length < best.Length )
                    {
                        best = new Route( route );
                        attempt = 0;

                        if ( BetterRouteFound != null )
                            BetterRouteFound( this, new BetterRouteFoundEventArgs( route ) );
                    }
                    else if ( attempt % ( Attempts >> 4 ) == 0 )
                        _rand = new Random();

                    if ( printProgress )
                    {
                        Console.CursorLeft = 10;
                        Console.Write( "{0}/{1} - {2}    ", attempt, Attempts, best.Length );
                    }
                }
            };

            ThreadStart loop = () =>
            {
                Route route = new Route( graph );

                for (;;)
                {
                    while ( next() )
                    {
                        route.Clear();
                        for ( int i = 0; i < graph.Count; ++i )
                        {
                            route.AddEnd( _rand.Next( i, graph.Count ) );
                            Searcher.Improve( route );
                        }
                        compare( route );
                    }

                    if ( waitForEnd() )
                        break;
                }
            };

            Thread[] workers = new Thread[Threads - 1];
            for ( int i = 0; i < workers.Length; ++i )
            {
                workers[i] = new Thread( loop );
                workers[i].Start();
            }

            loop();

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
