using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    class TestConstrSearcher : ConstructiveSearcher
    {
        private int _lastIndex;

        public TestConstrSearcher( HillClimbSearcher improver = null )
            : base( improver ) { }

        protected override int ChooseNext( Route route )
        {
            int best = -1;
            int score = 0;
            _lastIndex = 0;

            for ( int i = 0; i < route.Graph.Size; ++i )
            {
                if ( IsAdded( i ) )
                    continue;

                int minDist = Int32.MaxValue;
                int minIndex = 0;
                for ( int j = 0; j < route.Count; ++j )
                {
                    int dist = route.Graph[ i, route[ j ] ]
                        + route.Graph[ i, route[ j + 1 ] ];

                    if ( dist < minDist )
                    {
                        minDist = dist;
                        minIndex = j;
                    }
                }

                if ( minDist > score )
                {
                    best = i;
                    score = minDist;
                    _lastIndex = minIndex;
                }
            }

            return best;
        }

        protected override int ChooseIndex( Route route, int vIndex )
        {
            return _lastIndex;
        }
    }
}
