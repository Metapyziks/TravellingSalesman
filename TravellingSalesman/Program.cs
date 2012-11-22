using System;
using System.Collections.Generic;
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
            
            do
            {
                Route def = Route.CreateDefault( graph );
                Console.WriteLine( "Starting with graph of length {0}", def.Length );

                Console.Write( "Current best: {0}", def.Length );

                Random rand = new Random();

                int best = def.Length;
                int tries = 0;
                while ( tries++ < 65536 )
                {
                    int start = rand.Next( def.Count );
                    int count = rand.Next( 2, def.Count / 2 );

                    def.Reverse( start, count );

                    if ( def.Length < best )
                    {
                        best = def.Length;
                        tries = 0;
                        Console.CursorLeft = 14;
                        Console.Write( best );
                    }
                    else
                        def.Reverse( start, count );
                }

                Console.WriteLine( "\nNo better route found after {0} tries", tries - 1 );

                Console.WriteLine( def.ToString() );
            }
            while ( Console.ReadKey().Key != ConsoleKey.Q );
        }
    }
}
