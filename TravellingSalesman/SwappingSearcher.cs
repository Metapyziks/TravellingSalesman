using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    public class SwappingSearcher : HillClimbSearcher
    {
        public override bool Iterate(Route route)
        {
            int bestI = 0;
            int bestJ = 0;
            int bestScore = 0;

            for (int i = 0; i < route.Count; ++i) {
                int i0 = route[i - 1];
                int i1 = route[i];
                int i2 = route[i + 1];
                int iS = route.Graph[i0, i1] + route.Graph[i1, i2];
                for (int j = i + 1; j < route.Count; ++j) {
                    int j0 = route[j - 1];
                    int j1 = route[j];
                    int j2 = route[j + 1];
                    int jS = route.Graph[j0, j1] + route.Graph[j1, j2];

                    int orig = iS + jS;

                    if (j0 == i1) {
                        j0 = j1;
                        i2 = i1;
                    } else if (i0 == j1) {
                        i0 = i1;
                        j2 = j1;
                    } else {
                        i0 = route[i - 1];
                        i2 = route[i + 1];
                    }

                    int next = route.Graph[i0, j1] + route.Graph[j1, i2]
                             + route.Graph[j0, i1] + route.Graph[i1, j2];

                    int score = orig - next;

                    if (score > bestScore) {
                        bestI = i;
                        bestJ = j;
                        bestScore = score;
                    }
                }
            }

            if (bestScore > 0) {
                route.Swap(bestI, bestJ);
                return true;
            }

            return false;
        }
    }
}
