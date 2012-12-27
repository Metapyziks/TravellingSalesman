using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualiser
{
    class Program
    {
        static void Main( string[] args )
        {
            if ( args.Length == 0 )
            {
                Console.WriteLine( "usage: Visualiser.exe <cityfile>" );
                return;
            }

            if ( !File.Exists( args[0] ) )
            {
                Console.WriteLine( "file \"{0}\" does not exist", args[0] );
                return;
            }

            VisualiserWindow window = new VisualiserWindow( 800, 600 );
            window.Graph = PositionalGraph.FromFile( args[0] );
            window.Run();
            window.Dispose();
        }
    }
}
