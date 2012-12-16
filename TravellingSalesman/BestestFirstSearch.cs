using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    class BestestFirstSearch : ConstructiveSearcher
    {
        private Route _testRoute;

        public BestestFirstSearch( HillClimbSearcher improver = null )
            : base( improver )
        {
            _testRoute = null;
        }
        
        protected override int ChooseNext( Route route )
        {
            if ( route.Count == 0 )
                return 0;

            if ( _testRoute == null || _testRoute.Graph != route.Graph )
                _testRoute = new Route( route.Graph );

            int best = -1;
            int bestVal = -1;
            int score = Int32.MaxValue;
            for ( int i = route.Count; i < route.Graph.Count; ++i )
            {
                int val = route.GetFromSelectionBuffer( i );
                _testRoute.Clear();
                _testRoute.Copy( route );
                _testRoute.AddEnd( i );
                Improver.Improve( _testRoute, false );
                int dist = _testRoute.Length;
                if ( dist < score || ( dist == score && val < bestVal ) )
                {
                    best = i;
                    bestVal = val;
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
