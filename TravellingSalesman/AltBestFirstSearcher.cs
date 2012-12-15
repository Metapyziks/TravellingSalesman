using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class AltBestFirstSearcher : ConstructiveSearcher
    {
        private int _lastIndex;

        public AltBestFirstSearcher( HillClimbSearcher improver = null )
            : base( improver ) { }

        protected override int ChooseNext( Route route )
        {
            if ( route.Count == 0 )
                return 0;

            int best = -1;
            int score = 0;
            _lastIndex = 0;

            for ( int i = route.Count; i < route.Graph.Count; ++i )
            {
                int val = route.GetFromSelectionBuffer( i );

                int minDist = Int32.MaxValue;
                int minIndex = 0;
                for ( int j = 0; j < route.Count; ++j )
                {
                    int dist = route.Graph[val, route[j]]
                        + route.Graph[val, route[j + 1]];

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
