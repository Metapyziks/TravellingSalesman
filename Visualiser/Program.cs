using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualiser
{
    class Program
    {
        static void Main( string[] args )
        {
            VisualiserWindow window = new VisualiserWindow( 800, 600 );
            window.Run();
            window.Dispose();
        }
    }
}
