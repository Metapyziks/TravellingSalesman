using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    public class WorstFirstSearcher : ConstructiveSearcher
    {
        private int _lastIndex;

        public WorstFirstSearcher( HillClimbSearcher improver = null )
            : base( improver ) { }

        protected override int ChooseNext( Route route )
        {
            if ( route.Count == 0 )
                return 0;

            return route.SelectNextBest( route.Graph.Count - route.Count - 1 );

            /*
            int last = route[route.Count - 1];

            int best = -1;
            int bestVal = -1;
            int score = 0;
            for ( int i = route.Count; i < route.Graph.Count; ++i )
            {
                int val = route.GetFromSelectionBuffer( i );
                int dist = route.Graph[last, val];
                if ( dist > score || ( dist == score && val < bestVal ) )
                {
                    best = i;
                    bestVal = val;
                    score = dist;
                }
            }

            return best;
            */
        }

        protected override int ChooseIndex( Route route, int vIndex )
        {
            return _lastIndex;
        }
    }
}
