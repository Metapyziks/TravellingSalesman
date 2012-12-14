using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public static class Tools
    {
        public static int Partition( this int[] arr, int pivot )
        {
            return arr.Partition( 0, arr.Length - 1, pivot, Comparer<int>.Default );
        }

        public static int Partition( this int[] arr, int pivot,
            IComparer<int> comparer )
        {
            return arr.Partition( 0, arr.Length - 1, pivot, comparer );
        }

        public static int Partition( this int[] arr, int start, int end, int pivot )
        {
            return arr.Partition( start, end, pivot, Comparer<int>.Default );
        }

        public static int Partition( this int[] arr, int start, int end, int pivot,
            IComparer<int> comparer )
        {
            if ( pivot < start || pivot > end )
                throw new IndexOutOfRangeException( "Pivot was out of range" );

            if ( end <= start )
                return pivot;
            
            int pVal = arr[pivot];
            arr.Swap( end, pivot );

            int i = start, j = end - 1;
            while ( i < j )
            {
                while ( i < j && comparer.Compare( arr[i], pVal ) <= 0 )
                    ++i;

                while ( i < j && comparer.Compare( arr[j], pVal ) > 0 )
                    --j;

                if ( i < j )
                    arr.Swap( i, j );
            }

            if ( comparer.Compare( arr[i], pVal ) <= 0 )
                ++i;

            arr[end] = arr[i];
            arr[i] = pVal;
            return i;
        }

        public static void Swap( this int[] arr, int a, int b )
        {
            arr[a] ^= arr[b];
            arr[b] ^= arr[a];
            arr[a] ^= arr[b];
        }

        public static int SelectStatistic( this int[] buffer, int k )
        {
            return buffer[buffer.SelectStatisticIndex( k, Comparer<int>.Default )];
        }

        public static int SelectStatistic( this int[] buffer, int k, IComparer<int> comparer )
        {
            return buffer[buffer.SelectStatisticIndex( k, comparer )];
        }

        public static int SelectStatisticIndex( this int[] buffer, int k )
        {
            return buffer.SelectStatisticIndex( k, 0, buffer.Length, Comparer<int>.Default );
        }

        public static int SelectStatisticIndex( this int[] buffer, int k, IComparer<int> comparer )
        {
            return buffer.SelectStatisticIndex( k, 0, buffer.Length, comparer );
        }

        public static int SelectStatisticIndex( this int[] buffer, int k, int start, int count )
        {
            return buffer.SelectStatisticIndex( k, start, count, Comparer<int>.Default );
        }

        public static int SelectStatisticIndex( this int[] buffer, int k, int start, int count,
            IComparer<int> comparer )
        {
            if ( k < 0 || k >= count )
                throw new IndexOutOfRangeException();

            if ( count <= 5 )
            {
                Array.Sort( buffer, start, count, comparer );
                return start + k;
            }

            int subCount = count / 5;
            for ( int s = 0; s < subCount; ++s )
            {
                int sMedian = buffer.SelectStatisticIndex( 2, start + s * 5, 5, comparer );
                buffer.Swap( start + s, sMedian );
            }

            int medianIndex = buffer.SelectStatisticIndex( subCount >> 1, start, subCount, comparer );
            medianIndex = buffer.Partition( start, start + count - 1, medianIndex, comparer );

            if ( medianIndex == k + start )
                return k + start;

            if ( medianIndex > k + start )
                return buffer.SelectStatisticIndex( k, start, medianIndex - start, comparer );

            return buffer.SelectStatisticIndex( k + start - medianIndex - 1, medianIndex + 1,
                count + start - medianIndex - 1, comparer );
        }

        public static int FindIndexStatistic( this int[] buffer, int index )
        {
            return buffer.FindIndexStatistic( index, 0, buffer.Length, Comparer<int>.Default );
        }

        public static int FindIndexStatistic( this int[] buffer, int index, IComparer<int> comparer )
        {
            return buffer.FindIndexStatistic( index, 0, buffer.Length, comparer );
        }

        public static int FindIndexStatistic( this int[] buffer, int index,
            int start, int count )
        {
            return buffer.FindIndexStatistic( index, start, count, Comparer<int>.Default );
        }

        public static int FindIndexStatistic( this int[] buffer, int index,
            int start, int count, IComparer<int> comparer )
        {
            if ( index < start || index >= start + count )
                throw new IndexOutOfRangeException();

            int statistic = 0;
            for ( int i = start; i < start + count; ++i )
                if ( comparer.Compare( buffer[i], buffer[index] ) < 0 )
                    ++statistic;

            return statistic;
        }
    }
}
