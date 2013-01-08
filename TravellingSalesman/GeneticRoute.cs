using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class GeneticRoute : Route
    {
        private static List<int> _sBitCountCache = new List<int>();

        private static int FindBitCount( int size )
        {
            return (int) Math.Ceiling( Math.Log( size, 2 ) );
        }

        private static int FindTotalBitCount( int size )
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

        private static int FindTotalByteCount( int size )
        {
            return ( FindTotalBitCount( size ) + 7 ) >> 3;
        }

        private int _nextGeneBit;

        public byte[] Genes { get; private set; }

        public GeneticRoute( Graph graph, Random rand )
            : base( graph )
        {
            _nextGeneBit = 0;
            Genes = new byte[FindTotalByteCount( graph.Count )];
            for ( int i = 0; i < Genes.Length; ++i )
                Genes[i] = (byte) rand.Next( 1 );

            UpdateIndicesFromGenes();
        }

        public GeneticRoute( Route route )
            : base( route )
        {
            _nextGeneBit = 0;
            Genes = new byte[FindTotalByteCount( Graph.Count )];

            UpdateGenesFromIndices();
        }

        public GeneticRoute( Graph graph, byte[] genes )
            : base( graph )
        {
            if ( genes.Length != FindTotalByteCount( graph.Count ) )
                throw new Exception( "Incorrect gene count" );

            Genes = genes;
            UpdateIndicesFromGenes();
        }

        public void UpdateIndicesFromGenes()
        {
            Clear();
            for ( int g = 0; g < Graph.Count; ++g )
            {
                int start = _nextGeneBit;
                int count = FindBitCount( Graph.Count - Count );
                _nextGeneBit += count;
                int val = 0;

                for ( int b = count, i = start >> 3; b > -8 && i < Genes.Length; b -= 8, ++i )
                {
                    int shift = b - 8 + ( start & 0x7 );
                    byte byt = Genes[i];

                    if ( b == count )
                        byt &= (byte) ( 0xff >> ( start & 0x7 ) );

                    val |= shift >= 0 ? byt << shift : byt >> -shift;
                }

                int k = ( 1 << count ) - 1 > 0 ?
                    (int) Math.Ceiling( (double) val * ( Graph.Count - Count - 1 )
                    / ( ( 1 << count ) - 1 ) ) : 0;

                int vIndex = this.SelectNextBest( k );
                base.Insert( vIndex, g );
            }
        }

        private void AddGene( int vIndex )
        {
            int start = _nextGeneBit;
            int count = FindBitCount( Graph.Count - Count );
            _nextGeneBit += count;
            int val = ( Graph.Count - Count - 1 > 0 ) ?
                (int) ( (long) ( vIndex - Count ) * ( ( 1 << count ) - 1 )
                / ( Graph.Count - Count - 1 ) ) : 0;

            for ( int b = count, i = start >> 3; b > -8 && i < Genes.Length; b -= 8, ++i )
            {
                int shift = b - 8 + ( start & 0x7 );
                byte byt = (byte) ( ( shift >= 0 ? val >> shift : val << -shift ) & 0xff );

                Genes[i] |= byt;
            }
        }

        public void UpdateGenesFromIndices()
        {
            int count = Count;
            Clear();
            while( Count < count )
            {
                AddGene( Count + FindStatistic( Count ) );
                base.Insert( Count, Count );
            }
        }

        public override void Insert( int vIndex, int index )
        {
            if ( vIndex < Count )
                throw new IndexOutOfRangeException();

            if ( index != Count )
                throw new NotSupportedException();

            AddGene( index );

            base.Insert( vIndex, index );
        }

        public override void Clear()
        {
            base.Clear();

            _nextGeneBit = 0;
        }

        public override void Reverse(int start, int count)
        {
            throw new NotSupportedException();
        }
    }
}
