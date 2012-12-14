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

            if( outDir != null && !Directory.Exists( outDir ) )
                Directory.CreateDirectory( outDir );

#if DEBUG
            SearchSingle( args.Length > 0 ? args[0]
                : "cityfiles" + DSC + "SAfile535.txt", outDir );
            Console.ReadKey();
#else
            SearchDirectory( args.Length > 0 ? args[0] : "cityfiles", outDir );
            Console.ReadKey();
#endif
        }

        private static Route RunSearch( Graph graph, ISearcher searcher )
        {
            _stopwatch.Restart();
            Route route = searcher.Search( graph, true );
            _stopwatch.Stop();
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
            
            searcher = new WorstFirstSearcher();
            _stopwatch.Restart();
            route = searcher.Search( graph, true );
            _stopwatch.Stop();
            Console.WriteLine( "Search time: {0}ms", _stopwatch.ElapsedMilliseconds );
            Console.WriteLine( route.ToString() );

            searcher = new BestFirstSearcher();
            _stopwatch.Start();
            route = searcher.Search( graph, true );
            _stopwatch.Restart();
            Console.WriteLine( "Search time: {0}ms", _stopwatch.ElapsedMilliseconds );
            Console.WriteLine( route.ToString() );
#endif
            Route wfs = RunSearch( graph, new WorstFirstSearcher( new ReversingSearcher() ) );
            Route bfs = RunSearch( graph, new BestFirstSearcher( new ReversingSearcher() ) );

            if ( wfs.Length < bfs.Length )
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine( "WorstFirstSearcher was better!" );

                if ( outDir != null )
                    File.WriteAllText( outDir + DSC + "tour" + graph.Name + ".txt", wfs.ToString( true ) );
            }

            if ( bfs.Length <= wfs.Length )
            {
                if ( bfs.Length < wfs.Length )
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine( "BestFirstSearcher was better!" );
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine( "Both routes had equal length!" );
                }

                if ( outDir != null )
                    File.WriteAllText( outDir + DSC + "tour" + graph.Name + ".txt", bfs.ToString( true ) );
            }
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
