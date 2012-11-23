using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    class Program
    {
        private static readonly char DSC = Path.DirectorySeparatorChar;

        static void Main( string[] args )
        {
            String outDir = ( args.Length > 1 ? args[ 1 ] : null );

            if( outDir != null && !Directory.Exists( outDir ) )
                Directory.CreateDirectory( outDir );

#if DEBUG
            SearchSingle( args.Length > 0 ? args[0]
                : "cityfiles" + DSC + "SAtestcase.txt", outdir );
            Console.ReadKey();
#else
            SearchDirectory( args.Length > 0 ? args[0] : "cityfiles", outDir );
#endif
        }

        static void SearchSingle( String filePath, String outDir = null )
        {
            Console.WriteLine( "Loading file {0}", filePath );
            Graph graph = Graph.FromFile( filePath );
            Console.WriteLine( "Graph loaded: {0} has {1} vertices", graph.Name, graph.Size );

            ISearcher searcher;
            Route route;
            Stopwatch stopwatch = new Stopwatch();

            searcher = new TestConstrSearcher( new ReversingSearcher() );
            stopwatch.Start();
            route = searcher.Search( graph, true );
            stopwatch.Stop();
            
            Console.WriteLine( "Search time: {0}ms", stopwatch.ElapsedMilliseconds );
            Console.WriteLine( route.ToString() );

            if( outDir != null )
                File.WriteAllText( outDir + DSC + "tour" + graph.Name + ".txt", route.ToString( true ) );
        }

        static void SearchDirectory( String directory, String outDir = null )
        {
            Console.WriteLine( "Loading directory {0}", directory );
            foreach( String filePath in Directory.EnumerateFiles( directory ) )
                SearchSingle( filePath, outDir );
        }
    }
}
