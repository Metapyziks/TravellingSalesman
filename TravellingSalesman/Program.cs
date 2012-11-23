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
        static void Main( string[] args )
        {
            String filePath = args.Length > 0 ? args[ 0 ]
                : "TestFiles" + Path.DirectorySeparatorChar + "SAtestcase.txt";

            Console.WriteLine( "Loading file {0}", filePath );
            Graph graph = Graph.FromFile( filePath );
            Console.WriteLine( "Graph loaded: {0} has {1} vertices", graph.Name, graph.Size );

            ISearcher searcher;
            Route route;
            Stopwatch stopwatch = new Stopwatch();

            searcher = new TestConstrSearcher();
            stopwatch.Start();
            route = searcher.Search( graph, true );
            stopwatch.Stop();
            Console.WriteLine( "Search time: {0}ms", stopwatch.ElapsedMilliseconds );
            Console.WriteLine( route.ToString() );

            searcher = new ReversingSearcher();
            stopwatch.Start();
            route = searcher.Search( graph, true );
            stopwatch.Stop();
            Console.WriteLine( "Search time: {0}ms", stopwatch.ElapsedMilliseconds );
            Console.WriteLine( route.ToString() );

            searcher = new TestConstrSearcher( new ReversingSearcher() );
            stopwatch.Start();
            route = searcher.Search( graph, true );
            stopwatch.Stop();
            Console.WriteLine( "Search time: {0}ms", stopwatch.ElapsedMilliseconds );
            Console.WriteLine( route.ToString() );

            Console.ReadKey();
        }
    }
}
