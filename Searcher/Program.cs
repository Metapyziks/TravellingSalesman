using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TravellingSalesman;

namespace Searcher
{
    public class Program
    {
        private static readonly char DSC = Path.DirectorySeparatorChar;
        private static readonly Stopwatch _stopwatch = new Stopwatch();

        public static void Main( string[] args )
        {
            String outDir = ( args.Length > 1 ? args[ 1 ] : null ) ?? "gvnj58";

            if ( !Directory.Exists( outDir ) )
                Directory.CreateDirectory( outDir );

            if ( !Directory.Exists( outDir + DSC + "TourfileA" ) )
                Directory.CreateDirectory( outDir + DSC + "TourfileA" );

            if ( !Directory.Exists( outDir + DSC + "TourfileB" ) )
                Directory.CreateDirectory( outDir + DSC + "TourfileB" );

#if DEBUG
            SearchSingle( args.Length > 0 ? args[0]
                : "cityfiles" + DSC + "SAfile535.txt", outDir );
#else
            SearchDirectory( args.Length > 0 ? args[0] : "cityfiles", outDir );
            Process.Start( "SAvalidtourcheck.py" );
#endif
            Console.WriteLine( "Press any key to exit..." );
            Console.ReadKey();
        }

        private static Route RunSearch( Graph graph, ISearcher searcher, Route route = null )
        {
            if ( searcher is HillClimbSearcher && route != null )
            {
                route = new Route( route );
                _stopwatch.Restart();
                ( (HillClimbSearcher) searcher ).Improve( route, true );
                _stopwatch.Stop();
            }
            else
            {
                _stopwatch.Restart();
                route = searcher.Search( graph, true );
                _stopwatch.Stop();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine( "Search time: {0}ms", _stopwatch.ElapsedMilliseconds );
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine( route.ToString() );
            return route;
        }

        public static void SearchSingle( String filePath, String outDir )
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine( "Loading file {0}", filePath );
            Graph graph = Graph.FromFile( filePath );
            Console.WriteLine( "Graph loaded: {0} has {1} vertices", graph.Name, graph.Count );
            
            String savePath = outDir + DSC + "TourfileA" + DSC + "tour" + graph.Name + ".txt";
            String datePath = Path.GetDirectoryName( savePath ) + Path.DirectorySeparatorChar;
            datePath += Path.GetFileNameWithoutExtension( savePath ) + ".";
            datePath += DateTime.Now.ToShortDateString().Replace( '/', '-' );
            datePath += Path.GetExtension( savePath );

            Route best = null;
            if ( File.Exists( savePath ) )
            {
                best = Route.FromFile( graph, savePath );
                Console.Write( "Record to beat: " );
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write( best.Length );
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine( " ({0})", File.GetLastWriteTime( savePath ).ToShortDateString() );
            }
            
            Route dayBest = null;
            if ( File.Exists( datePath ) )
            {
                dayBest = Route.FromFile( graph, datePath );
                Console.Write( "Today's record: " );
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write( best.Length );
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine( " ({0})", File.GetLastWriteTime( datePath ).ToShortTimeString() );
            }
#if DEBUG
            ISearcher searcher;
            Route route;
            
            searcher = new BestFirstSearcher( new ReversingSearcher() );
            _stopwatch.Restart();
            route = searcher.Search( graph, true );
            _stopwatch.Stop();
            Console.WriteLine( "Search time: {0}ms", _stopwatch.ElapsedMilliseconds );
            Console.WriteLine( route.ToString() );

            GeneticSearcher genSearcher = new GeneticSearcher();
            genSearcher.Improve( route, true );
#else
            bool record = false;
            bool dayRecord = false;

            StochasticHillClimbSearcher searcher = new StochasticHillClimbSearcher( new ReversingSearcher() )
            {
                Attempts = graph.Count < 17 ? 256 :
                    graph.Count < 50 ? 65536 : graph.Count < 100 ? 32768 : graph.Count < 500 ? 8192 : 4096,
                Threads = 4
            };

            searcher.BetterRouteFound += ( sender, e ) =>
            {
                if ( best == null || e.Route.Length < best.Length )
                {
                    record = dayRecord = true;

                    e.Route.Save( savePath );
                    e.Route.Save( datePath );
                }
                else if ( dayBest == null || e.Route.Length < dayBest.Length )
                {
                    dayRecord = true;
                    e.Route.Save( datePath );
                }
            };

            Route route = RunSearch( graph, searcher );

            if ( record || route.Save( savePath ) )
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine( "****************" );
                Console.WriteLine( "** NEW RECORD **" );
                Console.WriteLine( "****************" );
                route.Save( datePath );
            }
            else if ( dayRecord || route.Save( datePath ) )
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine( "================" );
                Console.WriteLine( "== DAY RECORD ==" );
                Console.WriteLine( "================" );
            }
#endif
        }

        public static void SearchDirectory( String directory, String outDir )
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine( "Loading directory {0}", directory );
            foreach( String filePath in Directory.EnumerateFiles( directory ) )
                SearchSingle( filePath, outDir );
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
