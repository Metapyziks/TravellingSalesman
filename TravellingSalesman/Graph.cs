using System;
using System.Collections.Generic;
using System.IO;

namespace TravellingSalesman
{
    public class Graph
    {
        public static Graph FromFile( String path )
        {
            return new Graph( File.ReadAllText( path ) );
        }

        public readonly String Name;
        public readonly int Count;

        private int[,] _weights;

        public int this[int a, int b]
        {
            get { return _weights[a, b]; }
        }

        private Graph( String data )
        {
            int index = 0;

            String str;
            while ( ( str = data.ReadNext( ref index ) ).IsKeyVal() )
            {
                KeyValuePair<String, String> keyVal = str.ParseKeyVal();
                switch ( keyVal.Key )
                {
                    case "NAME":
                        Name = keyVal.Value; break;
                    case "SIZE":
                        Count = Int32.Parse( keyVal.Value ); break;
                }
            }

            if ( Count <= 0 )
                throw new Exception( "Invalid graph size given" );

            _weights = new int[Count, Count];

            for ( int i = 0; i < Count; ++i )
            {
                for ( int j = i + 1; j < Count; ++j )
                {
                    _weights[i, j] = _weights[j, i] = Int32.Parse( str );

                    if ( index >= data.Length )
                        break;

                    str = data.ReadNext( ref index );
                }
            }
        }
    }
}
