using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
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
#endif
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
            EitherOrSearcher searcher = new EitherOrSearcher(
                new WorstFirstSearcher( new ReversingSearcher() ),
                new BestFirstSearcher( new ReversingSearcher() ),
                new AltBestFirstSearcher( new ReversingSearcher() ),
                new StochasticHillClimbSearcher( new ReversingSearcher() ) { Attempts = 1024 } );

            Route route = RunSearch( graph, searcher );

            String savePath = outDir + DSC + "TourfileA" + DSC + "tour" + graph.Name + ".txt";
            String datePath = Path.GetDirectoryName( savePath ) + Path.DirectorySeparatorChar;
            datePath += Path.GetFileNameWithoutExtension( savePath ) + ".";
            datePath += DateTime.Now.ToShortDateString().Replace( '/', '-' );
            datePath += Path.GetExtension( savePath );

            if ( route.Save( savePath ) )
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine( "****************" );
                Console.WriteLine( "** NEW RECORD **" );
                Console.WriteLine( "****************" );
                route.Save( datePath );
            }
            else if ( route.Save( datePath ) )
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
