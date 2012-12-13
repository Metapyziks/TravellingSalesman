using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class GeneticRoute : Route
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

        private int _nextGeneBit;

        public byte[] Genes { get; private set; }

        public GeneticRoute( Graph graph, int[] indices )
            : base( graph, indices )
        {
            _nextGeneBit = 0;
        }

        public GeneticRoute( Graph graph, byte[] genes )
            : this( graph, new int[0] )
        {
            if ( genes.Length != ( FindTotalBitCount( Graph.Count ) + 7 ) >> 3 )
                throw new Exception( "Incorrect gene count" );

            for ( int g = 0; g < graph.Count; ++g )
            {
                int start = _nextGeneBit;
                int count = FindBitCount( Graph.Count - Count );
                int end = ( _nextGeneBit += count );
                int val = 0;

                for ( int b = count, i = start >> 3; b > -8 && i < genes.Length; b -= 8, ++i )
                {
                    int shift = b - 8 + ( start & 0x7 );
                    byte byt = genes[i];

                    if ( b == count )
                        byt &= (byte) ( 0xff >> ( start & 0x7 ) );

                    val |= shift >= 0 ? byt << shift : byt >> -shift;
                }

                int k = ( 1 << count ) - 1 > 0 ?
                    (int) Math.Ceiling( (double) val * ( Graph.Count - Count - 1 )
                    / ( ( 1 << count ) - 1 ) ) : 0;

                int vIndex = k + Count; //this.SelectNextBest( k );
                base.Insert( vIndex, g );
            }

            Genes = (byte[]) genes.Clone();
        }

        public override void Insert( int vIndex, int index )
        {
            if ( vIndex < Count )
                throw new IndexOutOfRangeException();

            if ( index != Count )
                throw new NotSupportedException();

            if ( Genes == null )
                Genes = new byte[(FindTotalBitCount( Graph.Count ) + 7 ) >> 3];

            int start = _nextGeneBit;
            int count = FindBitCount( Graph.Count - Count );
            int end = ( _nextGeneBit += count );
            int val = ( Graph.Count - Count - 1 > 0 ) ?
                (int) ( (long) ( vIndex - Count ) * ( ( 1 << count ) - 1 )
                / ( Graph.Count - Count - 1 ) ) : 0;

            for ( int b = count, i = start >> 3; b > -8 && i < Genes.Length; b -= 8, ++i )
            {
                int shift = b - 8 + ( start & 0x7 );
                byte byt = (byte) ( ( shift >= 0 ? val >> shift : val << -shift ) & 0xff );

                Genes[i] |= byt;
            }

            base.Insert( vIndex, index );
        }

        public override void Reverse(int start, int count)
        {
            throw new NotSupportedException();
        }
    }
}
