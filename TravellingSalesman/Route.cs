using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    public class Route
    {
        private class NextComparer : IComparer<int>
        {
            private readonly Route _route;

            public NextComparer( Route route )
            {
                _route = route;
            }

            public int Compare( int x, int y )
            {
                int last = _route[_route.Count - 1];
                return _route.Graph[last, x] - _route.Graph[last, y];
            }
        }

        public static Route CreateDefault( Graph graph )
        {
            int[] indices = new int[graph.Count];
            for ( int i = 0; i < graph.Count; ++i )
                indices[i] = i;

            return new Route( graph, indices, graph.Count );
        }

        public readonly Graph Graph;

        private int _count;
        private int _length;
        private bool[] _added;
        private int[] _indices;

        private IComparer<int> _nextComparer;

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
                        _length += Graph[_indices[i], _indices[( i + 1 ) % Graph.Count]];
                }

                return _length;
            }
        }

        public int this[int index]
        {
            get
            {
                while ( index < 0 )
                    index += _count;

                while ( index >= _count )
                    index -= _count;

                return _indices[index];
            }
        }

        public Route( Graph graph )
        {
            Graph = graph;

            _indices = new int[graph.Count];
            _added = new bool[graph.Count];
            _length = -1;

            _nextComparer = new NextComparer( this );

            for ( int i = 0; i < graph.Count; ++i )
                _indices[i] = i;
        }

        public Route( Route clone )
            : this( clone.Graph, clone._indices, clone.Count )
        {
            _length = clone._length;
        }

        public Route( Graph graph, int[] indices, int count )
            : this( graph )
        {
            _count = count;
            for ( int i = 0; i < graph.Count; ++i )
            {
                _indices[i] = indices[i];
                _added[indices[i]] = i < _count;
            }
        }

        public int GetFromSelectionBuffer( int index )
        {
            return _indices[index];
        }

        public int VIndexOf( int index )
        {
            for ( int i = Count; i < Graph.Count; ++i )
                if ( _indices[i] == index )
                    return i;

            return -1;
        }

        public virtual void Insert( int vIndex, int index )
        {
            if ( _count >= Graph.Count )
                throw new Exception( "Route is at maximum capacity" );

            if ( vIndex < Count )
                throw new IndexOutOfRangeException();

            int val = _indices[vIndex];
            _added[val] = true;

            _indices[vIndex] = _indices[_count];

            for ( int i = _count; i > index; --i )
                _indices[i] = _indices[i - 1];

            _indices[index] = val;

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

        public bool IsAdded( int index )
        {
            return _added[index];
        }

        public int SelectNextBest( int k = 0 )
        {
            if ( Count == 0 )
                return k;

            return _indices.SelectStatisticIndex( k, Count, Graph.Count - Count, _nextComparer );
        }

        public int FindStatistic( int vIndex )
        {
            if ( Count == 0 )
                return vIndex;

            return _indices.FindIndexStatistic( vIndex, Count, Graph.Count - Count, _nextComparer );
        }

        public virtual void Reverse()
        {
            Reverse( 0, Count );
        }

        public virtual void Reverse( int start, int count )
        {
            int mid = count / 2;
            for ( int i = 0; i < mid; ++i )
            {
                int indA = ( start + i ) % _count;
                int indB = ( start + count - i - 1 ) % _count;
                _indices.Swap( indA, indB );
            }

            _length = -1;
        }

        public virtual void Clear()
        {
            _count = 0;
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
                    builder.Append( _indices[i] + 1 );
                    builder.Append( ',' );
                }
            }

            return builder.ToString( 0, builder.Length - 1 );
        }

        public override bool Equals( object obj )
        {
            if ( obj is Route )
            {
                Route route = (Route) obj;

                if ( route.Count != Count )
                    return false;

                for ( int i = 0; i < Count; ++i )
                    if ( this[i] != route[i] )
                        return false;

                return true;
            }

            return false;
        }
    }
}
