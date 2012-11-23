using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    abstract class ConstructiveSearcher : ISearcher
    {
        private HillClimbSearcher _improver;
        private bool[] _added;

        public ConstructiveSearcher( HillClimbSearcher improver = null )
        {
            _improver = improver;
        }

        public Route Search( Graph graph, bool printProgress = false )
        {
            Route route = new Route( graph, new int[ 0 ] );
            _added = new bool[ graph.Size ];

            if ( printProgress )
            {
                Console.WriteLine( "# Starting a new {0} search", GetType().Name );
                if ( _improver != null )
                    Console.WriteLine( "Search will use {0} to "
                    + "improve each iteration", _improver.GetType().Name );
                Console.Write( "Progress: 0/{0} - 0", graph.Size );
            }

            int lastCount = route.Count;
            while ( route.Count < graph.Size )
            {
                int vIndex = ChooseNext( route );
                route.Insert( vIndex, ChooseIndex( route, vIndex ) );
                _added[ vIndex ] = true;

                if ( printProgress )
                {
                    Console.CursorLeft = 10;
                    Console.Write( "{0}/{1} - {2}",
                        route.Count, graph.Size, route.Length );
                }

                lastCount = route.Count;

                if ( _improver != null )
                    _improver.Improve( route );
            }

            if ( printProgress )
                Console.WriteLine( "\nConstructive search complete" );

            return route;
        }

        protected bool IsAdded( int vIndex )
        {
            return _added[ vIndex ];
        }

        protected abstract int ChooseNext( Route route );
        protected abstract int ChooseIndex( Route route, int vIndex );
    }
}
