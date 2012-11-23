using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    class Route
    {
        public static Route CreateDefault( Graph graph )
        {
            int[] indices = new int[ graph.Size ];
            for ( int i = 0; i < graph.Size; ++i )
                indices[ i ] = i;

            return new Route( graph, indices );
        }

        public readonly Graph Graph;

        private int _count;
        private int _length;
        private int[] _indices;

        public int Count
        {
            get { return _count; }
        }

        public int Length
        {
            get
            {
                if ( _length == -1 )
                {
                    _length = 0;
                    for ( int i = 0; i < Count; ++i )
                        _length += Graph[ _indices[ i ], _indices[ ( i + 1 ) % Graph.Size ] ];
                }
                
                return _length;
            }
        }

        public int this[ int index ]
        {
            get
            {
                while ( index < 0 )
                    index += _count;

                while ( index >= _count )
                    index -= _count;

                return _indices[ index ];
            }
        }

        public Route( Graph graph, int[] indices )
        {
            Graph = graph;

            _indices = new int[ graph.Size ];
            _count = indices.Length;
            _length = -1;

            for ( int i = 0; i < _count; ++i )
                _indices[ i ] = indices[ i ];
        }

        public void Insert( int vIndex, int index )
        {
            if ( _count >= Graph.Size )
                throw new Exception( "Route is at maximum capacity" );

            for ( int i = _count; i > index; --i )
                _indices[ i ] = _indices[ i - 1 ];

            _indices[ index ] = vIndex;
            ++_count;
            _length = -1;
        }

        public void AddEnd( int vIndex )
        {
            Insert( vIndex, _count );
        }

        public void AddStart( int vIndex )
        {
            Insert( vIndex, 0 );
        }

        public void Reverse( int start, int count )
        {
            int mid = count / 2;
            for( int i = 0; i < mid; ++ i )
            {
                int indA = ( start + i ) % _count;
                int indB = ( start + count - i - 1 ) % _count;
                _indices[ indA ] ^= _indices[ indB ];
                _indices[ indB ] ^= _indices[ indA ];
                _indices[ indA ] ^= _indices[ indB ];
            }

            _length = -1;
        }

        public override string ToString()
        {
            return ToString( false );
        }

        public string ToString( bool includePath )
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat( "NAME = {0},\r\n", Graph.Name );
            builder.AppendFormat( "TOURSIZE = {0},\r\n", Count );
            builder.AppendFormat( "LENGTH = {0},", Length );

            if ( includePath )
            {
                builder.Append( "\r\n" );
                for ( int i = 0; i < _count; ++i )
                {
                    builder.Append( _indices[ i ] + 1 );
                    builder.Append( ',' );
                }
            }

            return builder.ToString( 0, builder.Length - 1 );
        }
    }
}
