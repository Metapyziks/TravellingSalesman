using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    public class WorstFirstSearcher : ConstructiveSearcher
    {
        public WorstFirstSearcher( HillClimbSearcher improver = null )
            : base( improver ) { }

        protected override int ChooseNext( Route route )
        {
            if ( route.Count == 0 )
                return 0;

            return route.SelectNextBest( route.Graph.Count - route.Count - 1 );
        }

        protected override int ChooseIndex( Route route, int vIndex )
        {
            return 0;
        }
    }
}
