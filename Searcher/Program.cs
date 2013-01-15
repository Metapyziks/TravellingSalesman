using System;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TravellingSalesman;

namespace Searcher
{
    public class Program
    {
        private static readonly char DSC = Path.DirectorySeparatorChar;
        private static readonly Stopwatch _stopwatch = new Stopwatch();

        private static int _threads;

        public static void Main( string[] args )
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-GB" );

            String outDir = ( args.Length > 1 ? args[ 1 ] : null ) ?? "gvnj58";

            if ( !Directory.Exists( outDir ) )
                Directory.CreateDirectory( outDir );

            if ( !Directory.Exists( outDir + DSC + "TourfileA" ) )
                Directory.CreateDirectory( outDir + DSC + "TourfileA" );

            if ( !Directory.Exists( outDir + DSC + "TourfileB" ) )
                Directory.CreateDirectory( outDir + DSC + "TourfileB" );

            bool quiet = args.Length > 2 && args[2] == "quiet";

            _threads = args.Length > 3 ? int.Parse(args[3]) : 4;

#if DEBUG
            SearchSingle( args.Length > 0 ? args[0]
                : "cityfiles" + DSC + "SAfile175.txt", outDir );
#else
            SearchDirectory( args.Length > 0 ? args[0] : "cityfiles", outDir, quiet );
            // Process.Start( "SAvalidtourcheck.py" );
#endif
            if ( !quiet )
            {
                Console.WriteLine( "Press any key to exit..." );
                Console.ReadKey();
            }
        }

        private static Route RunSearch( Graph graph, ISearcher searcher, Route route = null, bool quiet = false )
        {
            if ( searcher is HillClimbSearcher && route != null )
            {
                route = new Route( route );
                _stopwatch.Restart();
                ( (HillClimbSearcher) searcher ).Improve( route, !quiet );
                _stopwatch.Stop();
            }
            else
            {
                _stopwatch.Restart();
                route = searcher.Search( graph, !quiet );
                _stopwatch.Stop();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine( "Search time: {0}ms", _stopwatch.ElapsedMilliseconds );
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine( route.ToString() );
            return route;
        }

        public static void SearchSingle( String filePath, String outDir, bool quiet = false )
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine( "Loading file {0}", filePath );
            Graph graph = Graph.FromFile( filePath );
            Console.WriteLine( "Graph loaded: {0} has {1} vertices", graph.Name, graph.Count );
            
            String savePath = outDir + DSC + "TourfileB" + DSC + "tour" + graph.Name + ".txt";
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
                Console.Write( dayBest.Length );
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine( " ({0})", File.GetLastWriteTime( datePath ).ToShortTimeString() );
            }
            bool record = false;
            bool dayRecord = false;

            var searcher = new AntColonySearcher<Ant> {
                StepCount = graph.Count < 17 ? 256 :
                    graph.Count < 50 ? 65536 : graph.Count < 100 ? 32768 : graph.Count < 500 ? 8192 : 4096
            };

            searcher.BetterRouteFound += ( sender, e ) =>
            {
                if ( best == null || e.Route.Length < best.Length )
                {
                    record = dayRecord = true;

                    e.Route.Save( savePath );
                    e.Route.Save( datePath );
/*
/*
                    try {
                        Process.Start("git", string.Format("add {0}", savePath));
                        Process.Start("git", string.Format("commit -m \"[AUTO] New all time record for {0}\"", graph.Name));
                        Process.Start("git", "push origin linux");
                    } catch {

                    }
*/
                }
                else if ( dayBest == null || e.Route.Length < dayBest.Length )
                {
                    dayRecord = true;
                    e.Route.Save( datePath );
                }
            };

            Route route = RunSearch( graph, searcher, null, quiet );

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
        }

        public static void SearchDirectory( String directory, String outDir, bool quiet = false )
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine( "Loading directory {0}", directory );
            do
            {
                if ( quiet )
                    Console.WriteLine("Starting batch at {0}", DateTime.Now.ToString());

                foreach( String filePath in Directory.EnumerateFiles( directory ) )
                    SearchSingle( filePath, outDir, quiet );
                
                if ( quiet )
                    Thread.Sleep( 60000 );
            }
            while( quiet );
        }
    }
}
