using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class ChainedHillClimbSearcher : HillClimbSearcher
    {
        public HillClimbSearcher[] Searchers { get; private set; }

        public ChainedHillClimbSearcher(params HillClimbSearcher[] searchers)
        {
            Searchers = searchers;
        }

        public override bool Iterate(Route route)
        {
            bool improved = false;
            bool cont;
            do {
                cont = false;
                foreach (HillClimbSearcher searcher in Searchers) {
                    bool repeat;
                    do {
                        repeat = searcher.Iterate(route);
                        cont |= repeat;
                    } while (repeat);
                }
                improved |= cont;
            } while (cont);

            return improved;
        }
    }
}
