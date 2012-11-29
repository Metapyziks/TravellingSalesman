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

        private static String _ReadNext( String data, ref int index )
        {
            int next = data.IndexOf( ',', index );
            if ( next == -1 )
                next = data.Length;

            String str = data.Substring( index, next - index );
            index = next + 1;

            return str.Trim();
        }

        private static bool _IsKeyVal( String str )
        {
            return str.Contains( "=" );
        }

        private static KeyValuePair<String, String> _ParseKeyVal( String str )
        {
            int equIndex = str.IndexOf( '=' );
            return new KeyValuePair<string, string>(
                str.Substring( 0, equIndex ).ToUpper().Trim(),
                str.Substring( equIndex + 1 ).Trim() );
        }

        public readonly String Name;
        public readonly int Count;

        private int[,] _weights;

        public int this[int a, int b]
        {
            get { return _weights[a, b]; }
        }

        public Graph( String data )
        {
            int index = 0;

            String str;
            while ( _IsKeyVal( str = _ReadNext( data, ref index ) ) )
            {
                KeyValuePair<String, String> keyVal = _ParseKeyVal( str );
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

                    str = _ReadNext( data, ref index );
                }
            }
        }
    }
}
