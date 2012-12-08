using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    class GeneticRoute : Route
    {
        private static List<int> _sBitCountCache = new List<int>();

        protected static int FindBitCount( int size )
        {
            return (int) Math.Ceiling( Math.Log( size, 2 ) );
        }

        protected static int FindTotalBitCount( int size )
        {
            if ( size < 0 )
                return 0;

            if ( _sBitCountCache.Count <= size )
            {
                int curr = ( ( size < 2 ) ? 0 : FindBitCount( size ) ) + FindTotalBitCount( size - 1 );

                _sBitCountCache.Add( curr );
            }

            return _sBitCountCache[size];
        }

        private byte[] _genes;

        public GeneticRoute( Graph graph, int[] indices )
            : base( graph, indices ) { }

        public override void Insert( int vIndex, int index )
        {
            if ( index != Count )
                throw new NotSupportedException();

            if ( _genes == null )
                _genes = new byte[FindTotalBitCount( Graph.Count ) >> 3];

            int offset = FindTotalBitCount( index - 1 );


            base.Insert( vIndex, index );
        }

        public override void Reverse(int start, int count)
        {
            throw new NotSupportedException();
        }
    }
}
