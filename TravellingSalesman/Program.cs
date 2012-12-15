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
            String outDir = ( args.Length > 1 ? args[ 1 ] : null );

            if ( outDir != null )
            {
                if ( !Directory.Exists( outDir ) )
                    Directory.CreateDirectory( outDir );

                if ( !Directory.Exists( outDir + DSC + "TourfileA" ) )
                    Directory.CreateDirectory( outDir + DSC + "TourfileA" );

                if ( !Directory.Exists( outDir + DSC + "TourfileB" ) )
                    Directory.CreateDirectory( outDir + DSC + "TourfileB" );
            }

#if DEBUG
            SearchSingle( args.Length > 0 ? args[0]
                : "cityfiles" + DSC + "SAfile535.txt", outDir );
            Console.ReadKey();
#else
            SearchDirectory( args.Length > 0 ? args[0] : "cityfiles", outDir );
            Console.ReadKey();
#endif
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

        public static void SearchSingle( String filePath, String outDir = null )
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
                new BestFirstSearcher( new ReversingSearcher() ) );

            Route route = RunSearch( graph, searcher );

            if ( outDir != null )
                File.WriteAllText( outDir + DSC + "TourfileA" + DSC
                    + "tour" + graph.Name + ".txt", route.ToString( true ) );
#endif
        }

        public static void SearchDirectory( String directory, String outDir = null )
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine( "Loading directory {0}", directory );
            foreach( String filePath in Directory.EnumerateFiles( directory ) )
                SearchSingle( filePath, outDir );
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
