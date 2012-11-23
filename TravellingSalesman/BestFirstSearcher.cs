using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    class BestFirstSearcher : ConstructiveSearcher
    {
        public BestFirstSearcher( HillClimbSearcher improver = null )
            : base( improver ) { }

        protected override int ChooseNext( Route route )
        {
            if ( route.Count == 0 )
                return 0;

            int last = route[ route.Count - 1 ];

            int best = -1;
            int score = Int32.MaxValue;
            for ( int i = 0; i < route.Graph.Size; ++i )
            {
                if ( IsAdded( i ) )
                    continue;

                int dist = route.Graph[ last, i ];
                if ( dist < score )
                {
                    best = i;
                    score = dist;
                }
            }

            return best;
        }

        protected override int ChooseIndex( Route route, int vIndex )
        {
            return route.Count;
        }
    }
}
