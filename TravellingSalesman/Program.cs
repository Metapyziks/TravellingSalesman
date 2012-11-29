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

        public static void Main( string[] args )
        {
            String outDir = ( args.Length > 1 ? args[ 1 ] : null );

            if( outDir != null && !Directory.Exists( outDir ) )
                Directory.CreateDirectory( outDir );

#if DEBUG
            SearchSingle( args.Length > 0 ? args[0]
                : "cityfiles" + DSC + "SAtestcase.txt", outDir );
            Console.ReadKey();
#else
            SearchDirectory( args.Length > 0 ? args[0] : "cityfiles", outDir );
            Console.ReadKey();
#endif
        }

        public static void SearchSingle( String filePath, String outDir = null )
        {
            Console.WriteLine( "Loading file {0}", filePath );
            Graph graph = Graph.FromFile( filePath );
            Console.WriteLine( "Graph loaded: {0} has {1} vertices", graph.Name, graph.Count );

            ISearcher searcher;
            Stopwatch stopwatch = new Stopwatch();
#if DEBUG
            Route route;

            searcher = new WorstFirstSearcher();
            stopwatch.Restart();
            route = searcher.Search( graph, true );
            stopwatch.Stop();
            Console.WriteLine( "Search time: {0}ms", stopwatch.ElapsedMilliseconds );
            Console.WriteLine( route.ToString() );

            searcher = new BestFirstSearcher();
            stopwatch.Start();
            route = searcher.Search( graph, true );
            stopwatch.Restart();
            Console.WriteLine( "Search time: {0}ms", stopwatch.ElapsedMilliseconds );
            Console.WriteLine( route.ToString() );
#endif
            searcher = new WorstFirstSearcher( new ReversingSearcher() );
            stopwatch.Restart();
            Route wfs = searcher.Search( graph, true );
            stopwatch.Stop();
            Console.WriteLine( "Search time: {0}ms", stopwatch.ElapsedMilliseconds );
            Console.WriteLine( wfs.ToString() );

            searcher = new BestFirstSearcher( new ReversingSearcher() );
            stopwatch.Restart();
            Route bfs = searcher.Search( graph, true );
            stopwatch.Stop();
            Console.WriteLine( "Search time: {0}ms", stopwatch.ElapsedMilliseconds );
            Console.WriteLine( bfs.ToString() );

            if ( wfs.Length < bfs.Length )
            {
                Console.WriteLine( "WorstFirstSearcher was better!" );

                if ( outDir != null )
                    File.WriteAllText( outDir + DSC + "tour" + graph.Name + ".txt", wfs.ToString( true ) );
            }

            if ( bfs.Length <= wfs.Length )
            {
                Console.WriteLine( "BestFirstSearcher was better!" );

                if ( outDir != null )
                    File.WriteAllText( outDir + DSC + "tour" + graph.Name + ".txt", bfs.ToString( true ) );
            }
        }

        public static void SearchDirectory( String directory, String outDir = null )
        {
            Console.WriteLine( "Loading directory {0}", directory );
            foreach( String filePath in Directory.EnumerateFiles( directory ) )
                SearchSingle( filePath, outDir );
        }
    }
}
