using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    abstract class CompositeSearcher : ISearcher
    {
        public ISearcher[] Searchers { get; private set; }

        public CompositeSearcher( params ISearcher[] searchers )
        {
            if ( searchers.Length == 0 )
                throw new Exception( "At least one searcher required in a composite search" );

            Searchers = searchers;
        }

        public abstract Route Search( Graph graph, bool printProgress = false );
    }
}
