using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class BestFirstSearcher : ConstructiveSearcher
    {
        public BestFirstSearcher( HillClimbSearcher improver = null )
            : base( improver ) { }

        protected override int ChooseNext( Route route )
        {
            if ( route.Count == 0 )
                return 0;

            int last = route[route.Count - 1];

            int best = -1;
            int bestVal = -1;
            int score = Int32.MaxValue;
            for ( int i = route.Count; i < route.Graph.Count; ++i )
            {
                int val = route.GetFromSelectionBuffer( i );
                int dist = route.Graph[last, val];
                if ( dist < score || ( dist == score && val < bestVal ) )
                {
                    best = i;
                    bestVal = val;
                    score = dist;
                }
            }

            // New method which happens to be worse for the biggest graph:
            // best = route.SelectNextBest();

            return best;
        }

        protected override int ChooseIndex( Route route, int vIndex )
        {
            return route.Count;
        }
    }
}
