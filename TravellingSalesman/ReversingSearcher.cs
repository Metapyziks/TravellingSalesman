using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    class ReversingSearcher : HillClimbSearcher
    {
        protected override bool Iterate( Route route )
        {
            int last = route.Length;
            int bestStart = 0;
            int bestCount = 0;
            int bestScore = 0;

            int mid = route.Count / 2;
            for ( int i = 0; i < route.Count; ++i )
            {
                int s0 = route[ i ];
                int s1 = route[ i - 1 ];
                for ( int c = 2; c < mid; ++c )
                {
                    int e0 = route[ i + c - 1 ];
                    int e1 = route[ i + c ];

                    int orig = route.Graph[ s0, s1 ] + route.Graph[ e0, e1 ];
                    int next = route.Graph[ s0, e1 ] + route.Graph[ e0, s1 ];
                    int score = orig - next;

                    if ( score > bestScore )
                    {
                        bestStart = i;
                        bestCount = c;
                        bestScore = score;
                    }
                }
            }

            if ( bestCount > 0 )
            {
                route.Reverse( bestStart, bestCount );
                return true;
            }

            return false;
        }
    }
}
